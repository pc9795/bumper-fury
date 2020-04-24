using UnityEngine;

//REF: https://www.youtube.com/watch?v=o1XOUkYUDZU&list=PLB9LefPJI-5wH5VdLFPkWfnPjeI6OSys1
//I referenced about AI movements from the above mentioned youtube tutorial on Car AI.
public class AICar : MonoBehaviour
{
    //Public fields
    public GameObject modelPrefab;
    public Vector3 centreOfMass = new Vector3(0, 0.3f, 0);
    public int topSpeed = 20;

    // Private fields
    //Many of the configurations can be moved to AI manager.
    private StatsController stats;
    private SimpleCarController carController;
    private ProjectileShooter projectileShooter;
    private bool initialized;
    private GameObject modelInstance;
    private float horizontalInput;
    private float verticalInput;
    private AIManager.WayPoint currWaypoint;
    new private Rigidbody rigidbody;
    private bool reversing;
    private Vector3 reverseDirection;
    private float lastSpeedCheckTime;
    private float speedCheckTimeIntervalInSecs = 2;
    private float stuckThreshold = 2f;
    private float wayPointDistanceThreshold = 1;
    private bool flipped;
    private bool wasOutsideSmartBoundary;
    private Scoreboard scoreboard;
    private bool obstacleFound;
    private float obstacleAvoidanceInput;

    // Unity methods
    void Start()
    {
        //Get this object AI tag.
        gameObject.tag = GameManager.Tag.AI;
    }

    void LateUpdate()
    {
        //No updates if not initialized.
        if (!initialized)
        {
            return;
        }
        //No updates if dead.
        if (!stats.IsAlive())
        {
            return;
        }
        //If out of level or health drops below zero make it dead.
        if (stats.isOutOflevel || stats.health <= 0)
        {
            GameManager.INSTANCE.PushNotification(stats.displayName + " Eliminated!");
            Destroy(this.gameObject, GameManager.INSTANCE.deathTimer);
            stats.Die();
        }
        carController.ReleaseHandBrake();
        //AI Behaviors
        AI();
        //Move the car
        carController.Steer(horizontalInput);
        carController.Move(verticalInput);
        carController.UpdateWheelPoses();
        //Collect any score done by the current projectile if exists.
        float damageDoneWithProjectile = projectileShooter.CollectDamageDone();
        stats.AddScore(GameManager.INSTANCE.GetScoreFromDamage(damageDoneWithProjectile));
    }

    void OnTriggerEnter(Collider collider)
    {
        if (CheckItemTrigger(collider))
        {
            return;
        }
    }

    void OnTriggerStay(Collider collider)
    {
        Storm storm = collider.GetComponent<Storm>();
        if (storm != null)
        {
            float force = 2500;
            rigidbody.AddForce(storm.transform.forward * force);

            if (rigidbody.velocity.magnitude > AIManager.INSTANCE.stormSpeed)
            {
                carController.ApplyHandBrake();
            }
            return;
        }

        if (CheckLavaTrigger(collider))
        {
            return;
        }
        if (CheckWaterTrigger(collider))
        {
            return;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (CheckPlayerCollision(collision))
        {
            return;
        }
        if (CheckAICollision(collision))
        {
            return;
        }
        if (CheckBarelCollision(collision))
        {
            return;
        }
    }

    //Custom methods

    //Called by game manager to initialize.
    public void Init()
    {
        //Configure centre of mass.
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.centerOfMass = centreOfMass;
        //Initialize the projectile shooter.
        projectileShooter = GetComponent<ProjectileShooter>();
        //Initialize the stats controller.
        stats = GetComponent<StatsController>();
        //Create the car model.
        modelInstance = Instantiate(modelPrefab, transform.position, Quaternion.identity, transform);
        //Initialize the car controller.
        carController = modelInstance.GetComponent<SimpleCarController>();
        carController.maxSteerAngle = AIManager.INSTANCE.maxSteerAngle;
        carController.motorForce = AIManager.INSTANCE.motorForce;
        //Initialize the scoreboard.
        scoreboard = FindObjectOfType<Scoreboard>();

        lastSpeedCheckTime = Time.time;
        //Let the script now that updates can be applied on this object as it is initialized.
        initialized = true;
    }

    private void Reset()
    {
        Vector3 rotation = transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(new Vector3(0, rotation.y, 0));
        horizontalInput = 0;
        verticalInput = 0;
        flipped = false;
    }


    private bool CheckItemTrigger(Collider collider)
    {
        Item item = collider.GetComponent<Item>();
        if (item == null)
        {
            return false;
        }
        switch (item.type)
        {
            case Item.ItemType.ENERGY_BOOST:
                if (stats.IsEnergyFull())
                {
                    return false;
                }
                stats.CollectEnergy(item.value);
                Destroy(item.gameObject);
                break;
            case Item.ItemType.HEALTH_BOOST:
                if (stats.IsHealthFull())
                {
                    return false;
                }
                stats.CollectHealth(item.value);
                Destroy(item.gameObject);
                break;
            case Item.ItemType.SPEED_BOOST:
                carController.NitroBoost(item.value, item.duration);
                Destroy(item.gameObject);
                break;
        }
        return true;
    }

    private bool CheckLavaTrigger(Collider collider)
    {
        Lava lava = collider.GetComponent<Lava>();
        if (lava == null)
        {
            return false;
        }
        stats.DamageHealth(lava.baseDamage);
        return true;
    }

    private bool CheckWaterTrigger(Collider collider)
    {
        Water water = collider.GetComponent<Water>();
        if (water == null)
        {
            return false;
        }
        stats.DamageHealth(water.baseDamage);
        return true;
    }

    private bool CheckPlayerCollision(Collision collision)
    {
        //Models will be collided so we have to look for behaviors in parent.
        PlayerCar playerCar = collision.collider.GetComponentInParent<PlayerCar>();
        if (playerCar == null)
        {
            return false;
        }
        StatsController otherStats = playerCar.GetComponent<StatsController>();
        DamageCalcWithOtherCars(otherStats, collision);
        return true;
    }

    private bool CheckAICollision(Collision collision)
    {
        //Models will be collided so we have to look for behaviors in parent.
        AICar aiCar = collision.collider.GetComponentInParent<AICar>();
        if (aiCar == null)
        {
            return false;
        }
        StatsController otherStats = aiCar.GetComponent<StatsController>();
        DamageCalcWithOtherCars(otherStats, collision);
        return true;
    }

    private void DamageCalcWithOtherCars(StatsController otherStats, Collision collision)
    {
        //Early exist if it is not alive without any damage calculation.
        if (!otherStats.IsAlive())
        {
            return;
        }
        if (collision.relativeVelocity.magnitude <= PhysicsManager.INSTANCE.collisionRelativeVelocityThreshold)
        {
            return;
        }
        bool performedBySelf = false;
        foreach (ContactPoint point in collision.contacts)
        {
            Vector3 relative = transform.InverseTransformPoint(point.point);
            if (relative.z >= PhysicsManager.INSTANCE.collisionRelativeVectorZThreshold)
            {
                performedBySelf = true;
                break;
            }
        }

        int damage = GameManager.INSTANCE.GetDamageFromCollisonRelativeVelcoity(collision.relativeVelocity.magnitude);
        if (performedBySelf)
        {
            stats.AddScore(GameManager.INSTANCE.GetScoreFromDamage(damage));
            otherStats.DamageHealth(damage);
        }
        else
        {
            stats.CollectEnergy(GameManager.INSTANCE.GetEnergyFromDamage(damage));
        }
    }

    private bool CheckBarelCollision(Collision collision)
    {
        Barel barel = collision.collider.GetComponent<Barel>();
        if (barel == null)
        {
            return false;
        }
        AudioManager.INSTANCE.Play(AudioManager.AudioTrack.BAREL_EXPLODE);
        barel.Explode();
        return true;
    }

    private void AI()
    {
        StuckingCheck();
        if (flipped)
        {
            Reset();
        }
        else if (reversing)
        {
            Reverse();
        }
        else
        {
            if (IsReachedWayPoint())
            {
                DecideWayPoint();
            }
            MoveToWayPoint();
            //We are not checking smart boundary for cases when car is reversing. It will fall.
            SmartBoundaryDetection();
            SenseEnv();
            if (obstacleFound)
            {
                horizontalInput = obstacleAvoidanceInput;
            }
        }
    }

    private void SenseEnv()
    {
        float sensorLength = AIManager.INSTANCE.sensorLength;
        float sensorAngle = AIManager.INSTANCE.sensorAngle;
        Vector3 sensorPos = transform.position;
        sensorPos += transform.forward * carController.carDimensions.z / 2;
        sensorPos += transform.up * carController.carDimensions.y / 2;
        //Clear if there is some input from the previous sense iteration.
        obstacleFound = false;
        obstacleAvoidanceInput = 0f;
        RaycastHit hit;

        //Shotting is enabled only for Right, Left and Centre sensors not for Angluar ones.

        //Check right obstacles.
        //Front right sensor
        sensorPos += transform.right * carController.carDimensions.x / 2;
        if (Physics.Raycast(sensorPos, transform.forward, out hit, sensorLength))
        {
            if (AIManager.INSTANCE.IsObstacle(hit.collider))
            {
                obstacleFound = true;
                obstacleAvoidanceInput -= 1f;
                Debug.DrawLine(sensorPos, hit.point, Color.yellow);
            }
            else if (AIManager.INSTANCE.IsShootable(hit.collider) && projectileShooter.CanShoot() && stats.IsEnergyFull())
            {
                projectileShooter.Shoot();
            }
        }
        //Front right angle sensor
        else if (Physics.Raycast(sensorPos, Quaternion.AngleAxis(sensorAngle, transform.up) * transform.forward, out hit, sensorLength))
        {
            if (AIManager.INSTANCE.IsObstacle(hit.collider))
            {
                obstacleFound = true;
                obstacleAvoidanceInput -= 0.5f;
                Debug.DrawLine(sensorPos, hit.point, Color.yellow);
            }
        }

        //Checking Left Obstacles
        //Front left sensor
        //Multiplying by 2 because to place at right we already added an extra half from centre position.
        sensorPos -= 2 * transform.right * (carController.carDimensions.x / 2);
        if (Physics.Raycast(sensorPos, transform.forward, out hit, sensorLength))
        {
            if (AIManager.INSTANCE.IsObstacle(hit.collider))
            {
                obstacleFound = true;
                obstacleAvoidanceInput += 1f;
                Debug.DrawLine(sensorPos, hit.point, Color.yellow);
            }
            else if (AIManager.INSTANCE.IsShootable(hit.collider) && projectileShooter.CanShoot() && stats.IsEnergyFull())
            {
                projectileShooter.Shoot();
            }
        }
        //Front left angle sensor
        if (Physics.Raycast(sensorPos, Quaternion.AngleAxis(-sensorAngle, transform.up) * transform.forward, out hit, sensorLength))
        {
            if (AIManager.INSTANCE.IsObstacle(hit.collider))
            {
                obstacleFound = true;
                obstacleAvoidanceInput += 0.5f;
                Debug.DrawLine(sensorPos, hit.point, Color.yellow);
            }
        }

        //If we have a narrow obstacle(missed by left and right sensor) or obstacle on both sides(components from left and right
        //sensors got cancelled.). Use front sensor in this situation.
        //Front sensor
        if (obstacleAvoidanceInput == 0 && Physics.Raycast(sensorPos, transform.forward, out hit, sensorLength))
        {
            if (AIManager.INSTANCE.IsObstacle(hit.collider))
            {
                obstacleFound = true;
                Debug.DrawLine(sensorPos, hit.point, Color.yellow);
                if (hit.normal.x < 0)
                {
                    obstacleAvoidanceInput = -1f;
                }
                else
                {
                    obstacleAvoidanceInput = 1f;
                }
            }
            else if (AIManager.INSTANCE.IsShootable(hit.collider) && projectileShooter.CanShoot() && stats.IsEnergyFull())
            {
                projectileShooter.Shoot();
            }
        }
    }

    private void DebugWaypointSelection()
    {
        Color color = Color.blue;
        switch (currWaypoint.wayPointType)
        {
            case AIManager.WayPointType.SAFTEY:
            case AIManager.WayPointType.HEALTH:
            case AIManager.WayPointType.ENERGY:
            case AIManager.WayPointType.NITRO:
                color = Color.green;
                break;
            case AIManager.WayPointType.AI_CAR:
                color = Color.black;
                break;
            case AIManager.WayPointType.PLAYER:
                color = Color.magenta;
                break;
        }
        if (currWaypoint.containsTransform)
        {
            Debug.DrawLine(transform.position, currWaypoint.transform.position, color, 10);
        }
        else
        {
            Debug.DrawLine(transform.position, currWaypoint.vector, color, 10);
        }
    }

    private void SmartBoundaryDetection()
    {
        bool outsideSmartBoundary = IsOutsideSmartBoundary();
        if (outsideSmartBoundary)
        {
            if (!wasOutsideSmartBoundary)
            {
                reversing = true;
                reverseDirection = -transform.forward;
            }
            if (rigidbody.velocity.magnitude > AIManager.INSTANCE.smartBoundarySpeed)
            {
                carController.ApplyHandBrake();
            }
        }
        wasOutsideSmartBoundary = outsideSmartBoundary;
    }

    private bool IsOutsideSmartBoundary()
    {
        Bounds smartBounds = AIManager.INSTANCE.smartLevelBounds;
        Bounds bounds = stats.GetMaxBounds();
        if (!(smartBounds.Contains(bounds.min) && smartBounds.Contains(bounds.max)))
        {
            return true;
        }
        return false;
    }

    private void StuckingCheck()
    {
        float now = Time.time;
        //Do checks at regular intervals not every frame.
        if (now - lastSpeedCheckTime <= speedCheckTimeIntervalInSecs)
        {
            return;
        }
        lastSpeedCheckTime = now;
        float currSpeed = rigidbody.velocity.magnitude;
        //Check vehicle is stucked or not.
        if (currSpeed >= stuckThreshold)
        {
            return;
        }
        //Check vehicle is flipped or not.
        float flipIndicator = Vector3.Dot(transform.up, Vector3.down);
        if (flipIndicator >= PhysicsManager.INSTANCE.flipThreshold)
        {
            flipped = true;
            return;
        }
        
        if (reversing)
        {
            //If already reversing and stuck
            reversing = false;
        }
        else
        {
            //If vechile is stucked and not flipped then reverse.
            reverseDirection = -transform.forward;
            reversing = true;
        }
    }

    private bool IsReachedWayPoint()
    {
        //Reached at the waypoint
        if (currWaypoint == null)
        {
            return true;
        }
        if (currWaypoint.containsTransform && currWaypoint.transform == null)
        {
            return true;
        }
        return Vector3.Distance(
                transform.position, currWaypoint.containsTransform ? currWaypoint.transform.position : currWaypoint.vector
                ) <= wayPointDistanceThreshold;
    }

    //Right now I am not adding them to look for energy and nitro as it can cause weired race between AIs. Can add this in future.
    private void DecideWayPoint()
    {
        //Fill health
        if (stats.IsHealthCritical() && TrySelectingHealthWaypoint())
        {
            return;
        }
        //Try to attack the player.
        if (TrySelectingPlayerWaypoint())
        {
            return;
        }
        //Try to attack other AIs.
        if (TrySelectingOtherAIWaypoint())
        {
            return;
        }
        currWaypoint = AIManager.INSTANCE.GetRandWayPoint(stats.displayName);
        DebugWaypointSelection();
    }

    //Right now this mehtod is not used so can look to remove it in future.
    private void MoveTowardsSafteyPoint()
    {
        //Try to collect health;
        if (!stats.IsHealthFull() && TrySelectingHealthWaypoint())
        {
            return;
        }
        //Try to collect energy
        if (!stats.IsEnergyFull() && TrySelectingEnergyWaypoint())
        {
            return;
        }
        //Try selecting Nitro.
        if (TrySelectingNitroWaypoint())
        {
            return;
        }
        //Move to a safe place decided by AI manager
        currWaypoint = AIManager.INSTANCE.GetSafteyPoint(stats.displayName);
        DebugWaypointSelection();
    }

    private bool TrySelectingHealthWaypoint()
    {
        AIManager.WayPoint wayPoint = AIManager.INSTANCE.GetHealthLocation(stats.displayName);
        if (wayPoint == null)
        {
            return false;
        }
        currWaypoint = wayPoint;
        DebugWaypointSelection();
        return true;
    }

    private bool TrySelectingEnergyWaypoint()
    {
        AIManager.WayPoint wayPoint = AIManager.INSTANCE.GetEnergyLocation(stats.displayName);
        if (wayPoint == null)
        {
            return false;
        }
        currWaypoint = wayPoint;
        DebugWaypointSelection();
        return true;
    }

    private bool TrySelectingNitroWaypoint()
    {
        AIManager.WayPoint wayPoint = AIManager.INSTANCE.GetNitroLocation(stats.displayName);
        if (wayPoint == null)
        {
            return false;
        }
        currWaypoint = wayPoint;
        DebugWaypointSelection();
        return true;
    }

    private bool TrySelectingPlayerWaypoint()
    {
        int chances = (int)(1 / AIManager.INSTANCE.playerAttackingProb);
        if (Random.Range(0, chances) != 0)
        {
            return false;
        }
        AIManager.WayPoint playerLoc = AIManager.INSTANCE.GetPlayerLocation(stats.displayName);
        if (playerLoc == null)
        {
            return false;
        }
        currWaypoint = playerLoc;
        DebugWaypointSelection();
        return true;
    }

    private bool TrySelectingOtherAIWaypoint()
    {
        int chances = (int)(1 / AIManager.INSTANCE.otherAIAttackcingProb);
        if (Random.Range(0, chances) == 0)
        {
            return false;
        }
        AIManager.WayPoint otherAILoc = AIManager.INSTANCE.GetRandomAICarLocation(stats.displayName);
        if (otherAILoc == null)
        {
            return false;
        }
        currWaypoint = otherAILoc;
        DebugWaypointSelection();
        return true;
    }

    private void MoveToWayPoint()
    {
        Vector3 relative = transform.InverseTransformPoint(
            currWaypoint.containsTransform ? currWaypoint.transform.position : currWaypoint.vector
            );
        horizontalInput = relative.x / relative.magnitude;
        verticalInput = rigidbody.velocity.magnitude > topSpeed ? 0 : 1;
    }

    private void Reverse()
    {
        //We don't want the height component.
        Vector2 reversedDirection2D = new Vector2(reverseDirection.x, reverseDirection.z);
        Vector2 forward2D = new Vector2(transform.forward.x, transform.forward.z);

        if (Vector2.Dot(reversedDirection2D, forward2D) > PhysicsManager.INSTANCE.reversingThreshold)
        {
            DecideWayPoint();
            reversing = false;
            return;
        }
        //Turn with maximum steer angle.
        horizontalInput = 1;
        //Move back with maximum moter force.
        verticalInput = rigidbody.velocity.magnitude > topSpeed ? 0 : -1;
    }
}

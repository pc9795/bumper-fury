using UnityEngine;

//REF: https://www.youtube.com/watch?v=o1XOUkYUDZU&list=PLB9LefPJI-5wH5VdLFPkWfnPjeI6OSys1
//I referenced about AI movements from the above mentioned youtube tutorial on Car AI.
public class AICar : MonoBehaviour
{
    //Sensors to the car.
    public enum Sensor
    {
        LEFT, LEFT_ANGLE, RIGHT, RIGHT_ANGLE, CENTER, BACK
    }

    //Public fields

    //GameObject which will be the 3d model of the car.
    public GameObject modelPrefab;
    //Centre of the mass of the car.
    public Vector3 centreOfMass = new Vector3(0, 0.3f, 0);
    //Top speed of the AI car.
    public int topSpeed = 20;

    // Private fields

    //`StatsController` for this car. It will be hadling statistics like health, enrgy, score etc.
    private StatsController stats;
    //`SimpleCarController` for this car. It will control the underlying model. This script is not attached directly to this object
    //It will be added to the `modelPrefab`.
    private SimpleCarController carController;
    //It will be used to fire and manage projectiles.
    private ProjectileShooter projectileShooter;
    //A car model is attached to this object dynamically therefore need a variable to keep a track of this.
    private bool initialized;
    //Instance of the `modelPrefab`.
    private GameObject modelInstance;
    //Keep track of the horizontal input to be applied to underlying car model. x direction input.
    private float horizontalInput;
    //Keep track of the vertical input to be applied to underlying car model. y direction input.
    private float verticalInput;
    //Current way point of this AI
    private AIManager.WayPoint currWaypoint;
    //Rigid body attached to this AI
    new private Rigidbody rigidbody;
    //A variable to keep track whether it is reversing or not.
    private bool reversing;
    //Direction of reverse. This will be set while reversing to decide whether car is reversed or not.
    private Vector3 reverseDirection;
    //Last time when speed was checked
    private float lastSpeedCheckTime;
    //A flag to keep track whether it is flipped or not.
    private bool flipped;
    //A flag to keep track whether the car was outside smart boundary in the last frame.
    private bool wasOutsideSmartBoundary;
    //A flag to keep track whether an obstacle is detected by the sensors or not.
    private bool obstacleFound;
    //An input needed to avoid the obstacle. x direction
    private float obstacleAvoidanceInput;
    //A flag to keep track whether this AI is inside tornado or not.
    private bool insideTornado;
    //What was the last time when a way point was reached
    private float lastWayPointTime;

    // Unity methods
    void Start()
    {
        //Get this object's AI tag.
        gameObject.tag = GameManager.Tag.AI;
    }

    void LateUpdate()
    {
        //No updates if not initialized or dead.
        if (!initialized || !stats.IsAlive())
        {
            return;
        }
        //If out of level or health drops below zero make it dead.
        if (stats.isOutOflevel || stats.health <= 0)
        {
            GameManager.INSTANCE.PushNotification(stats.displayName + " Eliminated!");
            Destroy(this.gameObject, GameManager.INSTANCE.deathTimer);
            stats.Die();
            AIManager.INSTANCE.NotifyDeath(stats.displayName);
        }
        //Clear if there is some input from the previous sense iteration.
        obstacleFound = false;
        obstacleAvoidanceInput = 0f;
        //Release handbrakes by default. If needed it will be set down the flow again.
        carController.ReleaseHandBrake();
        //AI Behaviors
        AI();
        //If obstacle is found change the direction accordingly in x-direction.
        if (obstacleFound)
        {
            horizontalInput = obstacleAvoidanceInput;
        }
        //Move the car
        carController.Steer(horizontalInput);
        carController.Move(verticalInput);
        carController.UpdateWheelPoses();
        //Collect any score done by the current projectile if exists.
        float damageDoneWithProjectile = projectileShooter.CollectDamageDone();
        stats.AddScore(GameManager.INSTANCE.GetScoreFromDamage(damageDoneWithProjectile));
        //Turn this off by default. If needed it will be set down in the `OnTriggerStaty` again. This logic works with the assumption
        //That `OnTriggerStay` is executed before `LateUpdate`.
        insideTornado = false;
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
        if (CheckTornadoTrigger(collider))
        {
            return;
        }
        if (CheckStormTrigger(collider))
        {
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
        if (CheckRockCollision(collision))
        {
            return;
        }
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

    private bool CheckTornadoTrigger(Collider collider)
    {
        Tornado tornado = collider.GetComponent<Tornado>();
        if (tornado == null)
        {
            return false;
        }
        stats.DamageHealth(tornado.baseDamage);
        insideTornado = true;
        return true;
    }

    private bool CheckStormTrigger(Collider collider)
    {
        Storm storm = collider.GetComponent<Storm>();
        if (storm == null)
        {
            return false;
        }
        //Decrease storm force as AI is not very good at manuvering.
        rigidbody.AddForce(storm.transform.forward * (storm.force / 2));
        //Slow down the AI in storm.
        if (rigidbody.velocity.magnitude > AIManager.INSTANCE.stormSpeed)
        {
            carController.ApplyHandBrake();
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

    private bool CheckRockCollision(Collision collision)
    {
        if (!collision.collider.CompareTag(GameManager.Tag.ROCK))
        {
            return false;
        }
        foreach (ContactPoint contact in collision.contacts)
        {
            //Make it configurable in future.
            //The rock should fall right on the head.
            if (Mathf.Abs(Vector3.Dot(transform.up, contact.normal)) < 0.90)
            {
                continue;
            }
            //Rock will have a rigid body
            float mass = collision.collider.GetComponent<Rigidbody>().mass;
            float relativeVelocity = collision.relativeVelocity.magnitude;

            //Found using experimentaion.
            float powerFactor = 50;
            float maxForce = 200000;
            float radius = 40;
            float upwardRift = 2;

            //When weight of the rock is 1000 and then this value is coming in the range of approx 1000-15000
            //If the value of rock weight change then update this accordingly.
            float damage = (mass * relativeVelocity) / 1000;
            float force = Mathf.Clamp(mass * relativeVelocity * powerFactor, 0, maxForce);

            stats.DamageHealth(damage);
            rigidbody.AddExplosionForce(force, transform.position, radius, upwardRift);

            return true;
        }
        return false;
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
        //If below some velocity then not counted as an impacting collision
        if (collision.relativeVelocity.magnitude <= PhysicsManager.INSTANCE.collisionRelativeVelocityThreshold)
        {
            return;
        }
        //Check whether collision was performed by this AI or another contestant
        bool performedBySelf = false;
        foreach (ContactPoint point in collision.contacts)
        {
            //If the another contestant is in front of this car then damage is done by it.
            Vector3 relative = transform.InverseTransformPoint(point.point);
            if (relative.z >= PhysicsManager.INSTANCE.collisionRelativeVectorZThreshold)
            {
                performedBySelf = true;
                break;
            }
        }

        //Damage multiplier as AI speed is restricted. Change in future if needed.
        float damage = GameManager.INSTANCE.GetDamageFromCollisonRelativeVelcoity(collision.relativeVelocity.magnitude) *
            AIManager.INSTANCE.damageMultiplier;

        //If damge is done by this car then add score and apply damage to the opponent.    
        if (performedBySelf)
        {
            stats.AddScore(GameManager.INSTANCE.GetScoreFromDamage(damage));
            otherStats.DamageHealth(damage);
        }
        else
        {
            //If damge is done by another contenstant then collect energy.
            //The damge to health is updated by another contenstant.
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
        //If inside tornado do nothing
        if (insideTornado)
        {
            horizontalInput = 0;
            verticalInput = 0;
            return;
        }

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
            //We are not checking smart boundary for cases when car is reversing. This is a serious issue so fix in future.
            SmartBoundaryDetection();
            SenseEnv();
        }
    }

    private void ReactToSense(Sensor sensor, RaycastHit hit, Vector3 sensorPos)
    {
        if (AIManager.INSTANCE.IsInvisibleBoundary(hit.collider))
        {
            if (sensor == Sensor.CENTER)
            {
                StartReversing();
            }
            else
            {
                //Slow down on invisible boundary
                if (this.rigidbody.velocity.magnitude > AIManager.INSTANCE.invisibleBoundarySpeed)
                {
                    this.carController.ApplyHandBrake();
                }
                //Update obstacle avoidance.
                this.obstacleFound = true;
                this.obstacleAvoidanceInput += GetObstacleAvoidanceInput(sensor, hit);
            }
            Debug.DrawLine(sensorPos, hit.point, Color.yellow);
        }
        else if (AIManager.INSTANCE.IsObstacle(hit.collider))
        {

            if (sensor == Sensor.CENTER)
            {
                StartReversing();
            }
            else
            {
                //Update obstacle avoidance.
                obstacleFound = true;
                obstacleAvoidanceInput += GetObstacleAvoidanceInput(sensor, hit);
                Debug.DrawLine(sensorPos, hit.point, Color.yellow);
            }
        }
        //Will not shoot if detected by angle sensors.
        else if (sensor != Sensor.LEFT_ANGLE && sensor != Sensor.RIGHT_ANGLE &&
            projectileShooter.CanShoot() && stats.IsEnergyFull() && AIManager.INSTANCE.IsShootable(hit.collider))
        {
            projectileShooter.Shoot();
        }
    }

    private float GetObstacleAvoidanceInput(Sensor sensor, RaycastHit hit)
    {
        switch (sensor)
        {
            case Sensor.RIGHT: return -1;
            case Sensor.RIGHT_ANGLE: return -0.5f;
            case Sensor.LEFT: return 1;
            case Sensor.LEFT_ANGLE: return 0.5f;
            case Sensor.CENTER: return hit.normal.x < 0 ? -1 : 1f;
        }
        Debug.LogError("Getting an unconfigured sensor in method GetObstacleAvoidanceInput");
        return 0;
    }

    private void SenseEnv()
    {
        float sensorLength = AIManager.INSTANCE.sensorLength;
        float sensorAngle = AIManager.INSTANCE.sensorAngle;
        Vector3 sensorPos = transform.position;
        //Adjust sensor position according to car dimensions.
        sensorPos += transform.forward * carController.carDimensions.z / 2;
        sensorPos += transform.up * carController.carDimensions.y / 2;
        RaycastHit hit;

        //Check right obstacles.
        //Front right sensor
        sensorPos += transform.right * carController.carDimensions.x / 2;
        if (Physics.Raycast(sensorPos, transform.forward, out hit, sensorLength))
        {
            ReactToSense(Sensor.RIGHT, hit, sensorPos);
        }
        //Front right angle sensor
        else if (Physics.Raycast(sensorPos, Quaternion.AngleAxis(sensorAngle, transform.up) * transform.forward, out hit, sensorLength))
        {
            ReactToSense(Sensor.RIGHT_ANGLE, hit, sensorPos);
        }

        //Checking Left Obstacles
        //Front left sensor
        //Multiplying by 2 because to place at right we already added an extra half from centre position.
        sensorPos -= 2 * transform.right * (carController.carDimensions.x / 2);
        if (Physics.Raycast(sensorPos, transform.forward, out hit, sensorLength))
        {
            ReactToSense(Sensor.LEFT, hit, sensorPos);
        }
        //Front left angle sensor
        else if (Physics.Raycast(sensorPos, Quaternion.AngleAxis(-sensorAngle, transform.up) * transform.forward, out hit, sensorLength))
        {
            ReactToSense(Sensor.LEFT_ANGLE, hit, sensorPos);
        }

        //If we have a narrow obstacle(missed by left and right sensor) or obstacle on both sides(components from left and right
        //sensors got cancelled.). Use front sensor in this situation.
        //Front sensor
        sensorPos += transform.right * carController.carDimensions.x / 2;
        if (this.obstacleAvoidanceInput == 0 && Physics.Raycast(sensorPos, transform.forward, out hit, sensorLength))
        {
            ReactToSense(Sensor.CENTER, hit, sensorPos);
        }
        //Back sesnor
        //NOT TESTED PROPERLY
        sensorPos -= 2 * transform.forward * (carController.carDimensions.z / 2);
        if (Physics.Raycast(sensorPos, -transform.forward, out hit, sensorLength))
        {
            if (AIManager.INSTANCE.IsInvisibleBoundary(hit.collider))
            {
                if (reversing)
                {
                    StopReversing();
                    Debug.DrawLine(sensorPos, hit.point, Color.yellow);
                }
            }
        }
    }

    //A method to show the current way point selection in debug mode.
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
            //Reverse first time when outside smart boundary.
            if (!wasOutsideSmartBoundary)
            {
                StartReversing();
            }
            //Slow down the speed.
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
        if (now - lastSpeedCheckTime <= AIManager.INSTANCE.speedCheckTimeIntervalInSecs)
        {
            return;
        }
        lastSpeedCheckTime = now;
        float currSpeed = rigidbody.velocity.magnitude;
        //Check vehicle is stucked or not.
        if (currSpeed >= AIManager.INSTANCE.stuckThreshold)
        {
            return;
        }
        //Check vehicle is flipped or not.
        float flipIndicator = Vector3.Dot(transform.up, Vector3.down);
        if (flipIndicator >= AIManager.INSTANCE.flipThreshold)
        {
            flipped = true;
            return;
        }

        if (reversing)
        {
            //If already reversing and stuck
            StopReversing();
        }
        else
        {
            //If vechile is stucked and not flipped then reverse.
            StartReversing();
        }
    }

    private bool IsReachedWayPoint()
    {
        bool reached = false;
        //Reached at the waypoint
        if (currWaypoint == null)
        {
            reached = true;
        }
        //The object which this car was chasing destroyed.
        else if (currWaypoint.containsTransform && currWaypoint.transform == null)
        {
            reached = true;
        }
        //If not able to reach a static way point in a fixed amount of time.
        else if (currWaypoint.wayPointType != AIManager.WayPointType.PLAYER &&
            currWaypoint.wayPointType != AIManager.WayPointType.AI_CAR &&
            Time.time - lastWayPointTime > AIManager.INSTANCE.wayPointReachingThresholdInSecs)
        {
            reached = true;
        }
        else
        {
            reached = Vector3.Distance(
                    transform.position, currWaypoint.containsTransform ? currWaypoint.transform.position : currWaypoint.vector
                    ) <= AIManager.INSTANCE.wayPointDistanceThreshold;
        }
        //Update the time if reached to the waypoint.
        if (reached)
        {
            lastWayPointTime = Time.time;
        }
        return reached;
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
        //Decide on the basis of a probability
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
        //Decide on the basis of a probability
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

    private void StartReversing()
    {
        reversing = true;
        reverseDirection = -transform.forward;
    }

    private void StopReversing()
    {
        reversing = false;
    }

    private void Reverse()
    {
        //We don't want the height component.
        Vector2 reversedDirection2D = new Vector2(reverseDirection.x, reverseDirection.z);
        Vector2 forward2D = new Vector2(transform.forward.x, transform.forward.z);

        if (Vector2.Dot(reversedDirection2D, forward2D) > AIManager.INSTANCE.reversingThreshold)
        {
            DecideWayPoint();
            StopReversing();
            return;
        }
        //Turn with maximum steer angle.
        horizontalInput = 1;
        //Move back with maximum moter force.
        verticalInput = rigidbody.velocity.magnitude > topSpeed ? 0 : -1;
    }
}

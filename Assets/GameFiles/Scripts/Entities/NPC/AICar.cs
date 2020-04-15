﻿using UnityEngine;

public class AICar : MonoBehaviour
{
    //Public fields
    public GameObject modelPrefab;
    public Vector3 centreOfMass = new Vector3(0, 0.3f, 0);
    public int speedLimit = 10;

    // Private fields
    private StatsController stats;
    private SimpleCarController carController;
    private ProjectileShooter projectileShooter;
    private bool initialized;
    private GameObject modelInstance;
    private float horizontalInput;
    private float verticalInput;
    private Transform currWaypoint;
    new private Rigidbody rigidbody;
    private bool reversing;
    private Vector3 reverseDirection;
    private float lastSpeedCheckTime;
    private float speedCheckTimeIntervalInSecs = 5;
    private float stuckThreshold = 0.5f;
    private float wayPointDistanceThreshold = 5;
    private bool flipped;

    // Unity methods
    void Start()
    {
        //Get this object AI tag.
        gameObject.tag = GameManager.Tag.AI;

        rigidbody = GetComponent<Rigidbody>();
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
        //AI Behaviors
        AI();
        //Move the car
        carController.Steer(horizontalInput);
        carController.Move(verticalInput);
        carController.UpdateWheelPoses();
        //Collect any score done by the current projectile if exists.
        int damageDoneWithProjectile = projectileShooter.CollectDamageDone();
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
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        rigidbody.centerOfMass = centreOfMass;
        //Initialize the projectile shooter.
        projectileShooter = GetComponent<ProjectileShooter>();
        //Initialize the stats controller.
        stats = GetComponent<StatsController>();
        //Create the car model.
        modelInstance = Instantiate(modelPrefab, transform.position, Quaternion.identity, transform);
        //Initialize the car controller.
        carController = modelInstance.GetComponent<SimpleCarController>();

        lastSpeedCheckTime = Time.time;
        //Let the script now that updates can be applied on this object as it is initialized.
        initialized = true;
    }

    private void Reset()
    {
        Vector3 rotation = transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(new Vector3(0, rotation.y, 0));
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
                AudioManager.INSTANCE.Play(AudioManager.AudioTrack.ITEM_COLLECT);
                GameManager.INSTANCE.PushNotification("Picked up a ENERGY BOOST");
                stats.CollectEnergy(item.value);
                Destroy(item.gameObject);
                break;
            case Item.ItemType.HEALTH_BOOST:
                if (stats.IsHealthFull())
                {
                    return false;
                }
                AudioManager.INSTANCE.Play(AudioManager.AudioTrack.ITEM_COLLECT);
                GameManager.INSTANCE.PushNotification("Picked up a HEALTH BOOST");
                stats.CollectHealth(item.value);
                Destroy(item.gameObject);
                break;
            case Item.ItemType.SPEED_BOOST:
                AudioManager.INSTANCE.Play(AudioManager.AudioTrack.ITEM_COLLECT);
                GameManager.INSTANCE.PushNotification("Picked up a NITRO BOOST");
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
        if (!performedBySelf)
        {
            return;
        }
        print("Damage done by " + stats.displayName);
        //TODO do this calculations according to the impact.
        int damage = 0;

        stats.CollectEnergy(GameManager.INSTANCE.GetEnergyFromDamage(damage));
        stats.AddScore(GameManager.INSTANCE.GetScoreFromDamage(damage));
        otherStats.DamageHealth(damage);
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
        float now = Time.time;
        if (now - lastSpeedCheckTime > speedCheckTimeIntervalInSecs)
        {
            //At regular intervals check that the car is stucked or not.
            MovementChecks();
            lastSpeedCheckTime = now;
        }
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
            MoveToWayPoint();
        }
    }

    private void MovementChecks()
    {
        float currSpeed = rigidbody.velocity.magnitude;
        if (currSpeed >= stuckThreshold)
        {
            return;
        }
        float flipIndicator = Vector3.Dot(transform.up, Vector3.down);
        if (flipIndicator >= PhysicsManager.INSTANCE.flipThreshold)
        {
            print("Flipping");
            flipped = true;
            return;
        }
        transform.forward = -transform.forward;
        reverseDirection = -transform.forward;
        reversing = true;
    }

    private void MoveToWayPoint()
    {
        if (currWaypoint == null || Vector3.Distance(transform.position, currWaypoint.position) < wayPointDistanceThreshold)
        {
            currWaypoint = AIManager.INSTANCE.GetRandWayPoint();
            Debug.Log("New waypoint selected!");
            Debug.DrawLine(transform.position, currWaypoint.position, Color.blue, 10);
        }
        Vector3 relative = transform.InverseTransformPoint(currWaypoint.position);
        horizontalInput = relative.x / relative.magnitude;
        verticalInput = rigidbody.velocity.magnitude > speedLimit ? 0 : 1;
    }

    private void Reverse()
    {
        if (transform.forward == reverseDirection)
        {
            reversing = false;
            return;
        }
        horizontalInput = 1;
        verticalInput = rigidbody.velocity.magnitude > speedLimit ? 0 : -1;
    }
}

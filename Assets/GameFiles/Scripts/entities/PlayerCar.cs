using UnityEngine;

public class PlayerCar : MonoBehaviour
{
    //Public fields
    public GameObject modelPrefab;
    public Vector3 centreOfMass = new Vector3(0, 0.3f, 0);

    //Private fields
    private float horizontalInput;
    private float verticalInput;
    private ProjectileShooter projectileShooter;
    private StatsController stats;
    private SimpleCarController carController;
    private bool initialized;
    private GameObject modelInstance;
    private UIButtonManager uIButtonManager;
    private bool drifting;
    new private Rigidbody rigidbody;

    //Unity methods
    void Start()
    {
        //Give a specific tag to this object
        gameObject.tag = GameManager.Tag.PLAYER;
        //This component will be used on handheld devices
        uIButtonManager = FindObjectOfType<UIButtonManager>();
    }

    void LateUpdate()
    {
        //No Updates if not initialized
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
            stats.Die();
        }
        ProcessInput();
        //Move the car
        //We are not clearing `horizontalInput` and `veticalInput` as it is expected to be handeled inside `ProcessInput`.
        carController.Steer(horizontalInput);
        carController.Move(verticalInput);
        carController.Drift(drifting, horizontalInput);
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
        Tornado tornado = collider.GetComponent<Tornado>();
        if (tornado != null)
        {
            stats.DamageHealth(tornado.baseDamage);
            return;
        }

        Storm storm = collider.GetComponent<Storm>();
        if (storm != null)
        {
            float force = 10000;
            rigidbody.AddForce(storm.transform.forward * force);
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
        if (collision.collider.CompareTag(GameManager.Tag.ROCK))
        {
            foreach (ContactPoint contact in collision.contacts)
            {
                if (Mathf.Abs(Vector3.Dot(transform.up, contact.normal)) > 0.90)
                {
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
                    break;
                }
            }
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

    //Process the inputs
    public void ProcessInput()
    {
        if (SystemInfo.deviceType == DeviceType.Handheld)
        {
            ProcessHandheldInput();
        }
        else if (SystemInfo.deviceType == DeviceType.Desktop)
        {
            ProcessDesktopInput();
        }
    }

    //Process inputs from handheld devices
    private void ProcessHandheldInput()
    {
        if (GameManager.INSTANCE.useDpad)
        {
            //Use virtual joystick
            UseDpad();
        }
        else
        {
            //Use device accelrometer
            UseAccelrometer();
        }
        //Shoot the projectiile.
        if ((IsPlayerTouched() || uIButtonManager.IsCirclePressed()) && projectileShooter.CanShoot() && stats.IsEnergyFull())
        {
            stats.ConsumeEnergy();
            projectileShooter.Shoot();
        }
        //Reset the car position
        if (uIButtonManager.IsDeltaPressed())
        {
            Reset();
        }
    }

    private void UseDpad()
    {
        verticalInput = uIButtonManager.Vertical();
        horizontalInput = uIButtonManager.Horizontal();
    }

    private void UseAccelrometer()
    {
        float yMin = GameManager.INSTANCE.handHeldAxisMin.y;
        float yMax = GameManager.INSTANCE.handheldAxisMax.y;
        float xMin = GameManager.INSTANCE.handHeldAxisMin.x;
        float xMax = GameManager.INSTANCE.handheldAxisMax.x;
        float y = Mathf.Clamp(Input.acceleration.y, yMin, yMax);
        float x = Mathf.Clamp(Input.acceleration.x, xMin, xMax);

        verticalInput = RemapRange(y, yMin, yMax, -1f, 1f);
        horizontalInput = RemapRange(x, xMin, xMax, -1f, 1f);
    }

    private bool IsPlayerTouched()
    {
        //No touches
        if (Input.touchCount <= 0)
        {
            return false;
        }
        //Get the first touch;
        Touch touch = Input.GetTouch(0);
        //Touch is not ended.
        if (touch.phase != TouchPhase.Ended)
        {
            return false;
        }

        Ray ray = Camera.main.ScreenPointToRay(touch.position);
        RaycastHit hit;
        if (!Physics.Raycast(ray, out hit) || hit.collider == null)
        {
            return false;
        }

        GameObject touchedObject = hit.transform.gameObject;
        bool isPlayer = touchedObject.GetComponent<PlayerCar>() != null;
        return isPlayer;
    }

    //Map a value from one range to other.
    private float RemapRange(float value, float oldMin, float oldMax, float newMin, float newMax)
    {
        return (((value - oldMin) / (oldMax - oldMin)) * (newMax - newMin)) + newMin;
    }

    //Process input from desktop.
    private void ProcessDesktopInput()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        //Reset
        if (Input.GetKeyDown(KeyCode.X))
        {
            Reset();
        }
        //Shooting
        if (Input.GetKeyDown(KeyCode.Tab) && projectileShooter.CanShoot() && stats.IsEnergyFull())
        {
            stats.ConsumeEnergy();
            projectileShooter.Shoot();
        }
        //TODO think of removing it as handbreaking is specifically implemented for AI cars.
        if (Input.GetKeyDown(KeyCode.Q))
        {
            AudioManager.INSTANCE.Play(AudioManager.AudioTrack.HANDBRAKE);
        }
        if (Input.GetKey(KeyCode.Q))
        {
            carController.ApplyHandBrake();
        }
        else
        {
            carController.ReleaseHandBrake();
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            drifting = true;
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            drifting = false;
        }
    }

    private void Reset()
    {
        Vector3 rotation = transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(new Vector3(0, rotation.y, 0));
    }

    //Method used by game manager to initialize this object.
    public void Init()
    {
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.centerOfMass = centreOfMass;
        projectileShooter = GetComponent<ProjectileShooter>();
        stats = GetComponent<StatsController>();
        modelInstance = Instantiate(modelPrefab, transform.position, Quaternion.identity, transform);
        carController = modelInstance.GetComponent<SimpleCarController>();
        initialized = true;
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
        //TODO do this calculations according to the impact.
        int damage = 0;
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

}

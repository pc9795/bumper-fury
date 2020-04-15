using UnityEngine;

public class PlayerCar : MonoBehaviour
{
    //Public fields
    public GameObject modelPrefab;
    public Vector3 centreOfMass = new Vector3(0, 0.3f, 0);

    //Private fields
    private float horizontalInput;
    private float verticalInput;
    private ProjectileShooter projectileShotter;
    private StatsController stats;
    private SimpleCarController carController;
    private bool initialized;
    private GameObject modelInstance;
    private UIButtonManager uIButtonManager;

    //Unity methods
    void Start()
    {
        gameObject.tag = GameManager.Tag.PLAYER;
        uIButtonManager = FindObjectOfType<UIButtonManager>();
    }

    void LateUpdate()
    {
        if (!initialized)
        {
            return;
        }
        //No updates if dead.
        if (!stats.IsAlive())
        {
            return;
        }
        if (stats.isOutOflevel || stats.health <= 0)
        {
            stats.Die();
        }
        ProcessInput();
        carController.Steer(horizontalInput);
        carController.Move(verticalInput);
        carController.UpdateWheelPoses();
        int damageDoneWithProjectile = projectileShotter.CollectDamageDone();
        stats.AddScore(GameManager.INSTANCE.GetScoreFromDamage(damageDoneWithProjectile));
    }

    void OnTriggerEnter(Collider collider)
    {
        Item item = collider.GetComponent<Item>();
        if (item != null)
        {
            switch (item.type)
            {
                case Item.ItemType.ENERGY_BOOST:
                    if (stats.IsEnergyFull())
                    {
                        return;
                    }
                    AudioManager.INSTANCE.Play(AudioManager.AudioTrack.ITEM_COLLECT);
                    GameManager.INSTANCE.PushNotification("Picked up a ENERGY BOOST");
                    stats.CollectEnergy((int)item.value);
                    Destroy(item.gameObject);
                    break;
                case Item.ItemType.HEALTH_BOOST:
                    if (stats.IsHealthFull())
                    {
                        return;
                    }
                    AudioManager.INSTANCE.Play(AudioManager.AudioTrack.ITEM_COLLECT);
                    GameManager.INSTANCE.PushNotification("Picked up a HEALTH BOOST");
                    stats.CollectHealth((int)item.value);
                    Destroy(item.gameObject);
                    break;
                case Item.ItemType.SPEED_BOOST:
                    AudioManager.INSTANCE.Play(AudioManager.AudioTrack.ITEM_COLLECT);
                    GameManager.INSTANCE.PushNotification("Picked up a NITRO BOOST");
                    carController.NitroBoost(item.value, item.duration);
                    Destroy(item.gameObject);
                    break;

            }
            return;
        }
    }

    void OnTriggerStay(Collider collider)
    {
        Lava lava = collider.GetComponent<Lava>();
        if (lava != null)
        {
            stats.DamageHealth(lava.baseDamage);
            return;
        }

        Water water = collider.GetComponent<Water>();
        if (water != null)
        {
            stats.DamageHealth(water.baseDamage);
            return;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        Collider collider = collision.collider;
        //Models will be collided so we have to look for behaviors in parent.
        AICar aICar = collider.GetComponentInParent<AICar>();
        //If it is an AI car
        if (aICar != null)
        {
            StatsController aICarStats = aICar.GetComponent<StatsController>();
            if (aICarStats.IsAlive())
            {
                //TODO do this calculations according to the impact.
                int damage = 20;

                stats.CollectEnergy(GameManager.INSTANCE.GetEnergyFromDamage(damage));
                stats.AddScore(GameManager.INSTANCE.GetScoreFromDamage(damage));
                aICarStats.DamageHealth(damage);
            }
        }
        Barel barel = collider.GetComponent<Barel>();
        if (barel != null)
        {
            AudioManager.INSTANCE.Play(AudioManager.AudioTrack.BAREL_EXPLODE);
            barel.Explode();
        }
    }

    //Custom methods
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

    private void ProcessHandheldInput()
    {
        if (GameManager.INSTANCE.useDpad)
        {
            UseDpad();
        }
        else
        {
            UseAccelrometer();
        }

        ShootIfTouched();

        if (uIButtonManager.IsCirclePressed() && projectileShotter.CanShoot() && stats.IsEnergyFull())
        {
            stats.ConsumeEnergy();
            projectileShotter.Shoot();
        }

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

    private void ShootIfTouched()
    {
        if (Input.touchCount <= 0)
        {
            return;
        }
        //Get the first touch;
        Touch touch = Input.GetTouch(0);
        if (touch.phase != TouchPhase.Ended)
        {
            return;
        }
        Ray ray = Camera.main.ScreenPointToRay(touch.position);
        RaycastHit hit;
        //Depending on short ciructing.
        if (!Physics.Raycast(ray, out hit) || hit.collider == null)
        {
            return;
        }
        GameObject touchedObject = hit.transform.gameObject;
        //Logic to check that the touched object is player.
        bool isPlayer = touchedObject.GetComponent<PlayerCar>() != null;
        if (isPlayer && projectileShotter.CanShoot() && stats.IsEnergyFull())
        {
            stats.ConsumeEnergy();
            projectileShotter.Shoot();
        }
    }

    private float RemapRange(float value, float oldMin, float oldMax, float newMin, float newMax)
    {
        return (((value - oldMin) / (oldMax - oldMin)) * (newMax - newMin)) + newMin;
    }

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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (projectileShotter.CanShoot() && stats.IsEnergyFull())
            {
                stats.ConsumeEnergy();
                projectileShotter.Shoot();
            }
        }
    }

    private void Reset()
    {
        Vector3 rotation = transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(new Vector3(0, rotation.y, 0));
    }

    //Lazy init as model prefab is set dynamically.
    public void Init()
    {
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        rigidbody.centerOfMass = centreOfMass;
        projectileShotter = GetComponent<ProjectileShooter>();
        stats = GetComponent<StatsController>();
        modelInstance = Instantiate(modelPrefab, transform.position, Quaternion.identity, transform);
        carController = modelInstance.GetComponent<SimpleCarController>();
        initialized = true;
    }
}

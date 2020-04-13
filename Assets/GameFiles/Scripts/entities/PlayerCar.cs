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

    //Unity methods

    void Start()
    {
        gameObject.tag = GameManager.Tag.PLAYER;
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
        }
    }
    
    void OnTriggerStay(Collider collider)
    {
        Lava lava = collider.GetComponent<Lava>();
        if (lava != null)
        {
            stats.DamageHealth(lava.baseDamage);
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

using UnityEngine;

public class PlayerCar : MonoBehaviour
{
    //Public fields
    public GameObject modelPrefab;

    //Private fields
    private float horizontalInput;
    private float verticalInput;
    private ProjectileShooter projectileShotter;
    private StatsController playerStats;
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
        ProcessInput();
        carController.Steer(horizontalInput);
        carController.Move(verticalInput);
        carController.UpdateWheelPoses();
        int damageDoneWithProjectile = projectileShotter.CollectDamageDone();
        playerStats.AddScore(GameManager.INSTANCE.GetScoreFromDamage(damageDoneWithProjectile));
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

                playerStats.CollectEnergy(GameManager.INSTANCE.GetEnergyFromDamage(damage));
                playerStats.AddScore(GameManager.INSTANCE.GetScoreFromDamage(damage));
                aICarStats.DamageHealth(damage);
            }
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
            if (projectileShotter.CanShoot() && playerStats.IsEnergyFull())
            {
                playerStats.ConsumeEnergy();
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
        projectileShotter = GetComponent<ProjectileShooter>();
        playerStats = GetComponent<StatsController>();
        modelInstance = Instantiate(modelPrefab, transform.position, Quaternion.identity, transform);
        carController = modelInstance.GetComponent<SimpleCarController>();
        initialized = true;
    }
}

using UnityEngine;

public class PlayerCar : MonoBehaviour
{
    //Private fields
    private float horizontalInput;
    private float verticalInput;
    private ProjectileShooter projectileShotter;
    private StatsController playerStats;
    private SimpleCarController carController;

    //Unity methods
    void Start()
    {
        projectileShotter = GetComponent<ProjectileShooter>();
        playerStats = GetComponent<StatsController>();
        //Car controller will be in a model attached to this object.
        carController = GetComponentInChildren<SimpleCarController>();
    }

    void LateUpdate()
    {
        ProcessInput();
        carController.Steer(horizontalInput);
        carController.Move(verticalInput);
        carController.UpdateWheelPoses();
        int damageDoneWithProjectile = projectileShotter.CollectDamageDone();
        playerStats.AddScore(damageDoneWithProjectile / 2);
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
                //TODO do this calculations according to an impact score.
                playerStats.CollectEnergy(100);
                //TODO do this calculations according to an impact score.
                playerStats.AddScore(10);
                //TODO do this calculations according to an impact score.
                aICarStats.DamageHealth(20);
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
}

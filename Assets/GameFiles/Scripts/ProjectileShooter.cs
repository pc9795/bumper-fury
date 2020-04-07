using UnityEngine;

public class ProjectileShooter : MonoBehaviour
{
    //Public fields
    public GameObject projectileType;
    public float speed = 10;
    public float radius = 15;
    public float power = 30000;
    public float duration = 8;
    //TODO if the parent has a offset then need of this
    public float groundLevel = 0.6f;

    //Private fields
    private GameObject projectileInstance;
    private Rigidbody rigidBody;
    private Vector3 direction;
    private int damageDone = 0;


    //Unity methods
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (!projectileInstance)
        {
            return;
        }
        Collider[] colliders = Physics.OverlapSphere(projectileInstance.transform.position, radius);
        foreach (Collider collider in colliders)
        {
            //Collision will occur with models so have to look for behaviors in parent.
            AICar aICar = collider.GetComponentInParent<AICar>();
            if (aICar == null)
            {
                continue;
            }

            //TODO check the upward modifier settings.
            Rigidbody aICarRigidBody = aICar.GetComponent<Rigidbody>();
            aICarRigidBody.AddExplosionForce(power, projectileInstance.transform.position, radius, 3.0f);

            //TODO calculate damage on the basis of impact
            StatsController aiCarStats = aICar.GetComponent<StatsController>();
            aiCarStats.DamageHealth(1);
            damageDone += 1;
        }
        //Move with the local space
        projectileInstance.transform.position += direction * speed * Time.deltaTime;
    }

    // Custom methods
    public bool CanShoot()
    {
        return projectileInstance == null;
    }

    public void Shoot()
    {
        if (!CanShoot())
        {
            return;
        }
        //Instantiate a new projectile instance
        Vector3 position = rigidBody.transform.position;
        position.y = groundLevel;
        projectileInstance = Instantiate(projectileType, position, Quaternion.identity);
        Destroy(projectileInstance, duration);
        direction = rigidBody.transform.forward;
    }

    public int CollectDamageDone()
    {
        int returnValue = damageDone;
        damageDone = 0;
        return returnValue;
    }
}

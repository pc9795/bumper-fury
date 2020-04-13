using UnityEngine;

public class ProjectileShooter : MonoBehaviour
{
    //Public fields
    public GameObject projectileType;

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
        Projectile projectile = projectileInstance.GetComponent<Projectile>();
        Collider[] colliders = Physics.OverlapSphere(projectileInstance.transform.position, projectile.radius);
        foreach (Collider collider in colliders)
        {
            //Collision will occur with models so have to look for behaviors in parent.
            AICar aICar = collider.GetComponentInParent<AICar>();
            if (aICar == null)
            {
                continue;
            }

            Rigidbody aICarRigidBody = aICar.GetComponent<Rigidbody>();
            aICarRigidBody.AddExplosionForce(projectile.power, projectileInstance.transform.position,
                projectile.radius, projectile.upwardRift);

            StatsController aiCarStats = aICar.GetComponent<StatsController>();
            //TODO Modify base damage on the basis of impact
            aiCarStats.DamageHealth(projectile.baseDamage);
            damageDone += projectile.baseDamage;
        }
        //Move with the local space
        projectileInstance.transform.position += direction * projectile.speed * Time.deltaTime;
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
        AudioManager.INSTANCE.Play(AudioManager.AudioTrack.POWER_USE);
        projectileInstance = Instantiate(projectileType, rigidBody.transform.position, Quaternion.identity);
        Projectile projectile = projectileInstance.GetComponent<Projectile>();
        Vector3 currPos = projectileInstance.transform.position;
        projectileInstance.transform.position = new Vector3(currPos.x, currPos.y + projectile.groundLevel, currPos.z);
        Destroy(projectileInstance, projectile.duration);
        direction = rigidBody.transform.forward;
    }

    public int CollectDamageDone()
    {
        int returnValue = damageDone;
        damageDone = 0;
        return returnValue;
    }
}

using UnityEngine;

public class ProjectileShooter : MonoBehaviour
{
    //Public fields
    //Expected to be a particle system
    //Look for narow object type
    //Particle system which will be played as a bullet/power/projectile.
    public GameObject projectileType;

    //Private fields
    //Instance of the `projectileType` particle system.
    private GameObject projectileInstance;
    //Rigid body to which this script is attached to.
    private Rigidbody rigidBody;
    //This variable will hold the current direction of the projectile
    private Vector3 direction;
    //This variable will hold the damage done by the projectile.
    private float damageDone = 0;
    //Statistics of the parent contestant(AICar or PlayerCar)
    private StatsController stats;

    //Unity methods
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        stats = GetComponent<StatsController>();

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
            Rigidbody colliderRigidBody = null;
            StatsController colliderStats = null;
            //Collision will occur with models so have to look for behaviors in parent.
            AICar aICar = collider.GetComponentInParent<AICar>();
            PlayerCar playerCar = collider.GetComponentInParent<PlayerCar>();
            //Assuming that AICar and PlayerCar will never be on the same objec nd this two always expected to have a rigid body
            //and a staatistics controller.
            colliderRigidBody = aICar != null ? aICar.GetComponent<Rigidbody>() : colliderRigidBody;
            colliderStats = aICar != null ? aICar.GetComponent<StatsController>() : colliderStats;
            colliderRigidBody = playerCar != null ? playerCar.GetComponent<Rigidbody>() : colliderRigidBody;
            colliderStats = playerCar != null ? playerCar.GetComponent<StatsController>() : colliderStats;
            //If no rigidbody or it is detecting the shooter itself.
            if (colliderRigidBody == null || colliderStats.displayName.Equals(stats.displayName))
            {
                continue;
            }
            //Add an explosion force.
            colliderRigidBody.AddExplosionForce(projectile.power, projectileInstance.transform.position,
                projectile.radius, projectile.upwardRift);
            //Do the damage to other contenstant.
            colliderStats.DamageHealth(projectile.baseDamage);
            damageDone += projectile.baseDamage;
        }
        //Move with the local space
        projectileInstance.transform.position += direction * projectile.speed;
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
        //Off set above the ground
        projectileInstance.transform.position = new Vector3(currPos.x, currPos.y + projectile.groundLevel, currPos.z);
        Destroy(projectileInstance, projectile.duration);
        //Set the direction of the projectile.
        direction = rigidBody.transform.forward;
    }

    //The parent will collect the damage done by the current projectile and reset the damageDone.
    public float CollectDamageDone()
    {
        float returnValue = damageDone;
        damageDone = 0;
        return returnValue;
    }
}

﻿using UnityEngine;

public class ProjectileShooter : MonoBehaviour
{
    //Public fields
    public GameObject projectileType;

    //Private fields
    private GameObject projectileInstance;
    private Rigidbody rigidBody;
    private Vector3 direction;
    private float damageDone = 0;
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
            //Collision will occur with models so have to look for behaviors in parent.
            Rigidbody colliderRigidBody = null;
            StatsController colliderStats = null;
            AICar aICar = collider.GetComponentInParent<AICar>();
            PlayerCar playerCar = collider.GetComponentInParent<PlayerCar>();
            //Assuming that AICar and PlayerCar will never be on the same object
            //And if the component has rigidbody then it will also have StatsController.
            colliderRigidBody = aICar != null ? aICar.GetComponent<Rigidbody>() : colliderRigidBody;
            colliderStats = aICar != null ? aICar.GetComponent<StatsController>() : colliderStats;
            colliderRigidBody = playerCar != null ? playerCar.GetComponent<Rigidbody>() : colliderRigidBody;
            colliderStats = playerCar != null ? playerCar.GetComponent<StatsController>() : colliderStats;
            //If no rigidbody or it is detecting the shooter itself.
            if (colliderRigidBody == null || colliderStats.displayName.Equals(stats.displayName))
            {
                continue;
            }
            colliderRigidBody.AddExplosionForce(projectile.power, projectileInstance.transform.position,
                projectile.radius, projectile.upwardRift);
            //Do the damage.
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
        projectileInstance.transform.position = new Vector3(currPos.x, currPos.y + projectile.groundLevel, currPos.z);
        Destroy(projectileInstance, projectile.duration);
        direction = rigidBody.transform.forward;
    }

    public float CollectDamageDone()
    {
        float returnValue = damageDone;
        damageDone = 0;
        return returnValue;
    }
}

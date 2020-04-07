using UnityEngine;

public class DamageBehavior : MonoBehaviour
{
    //Public fields
    //These are expected to be looping animations.
    public GameObject halfDamageIndicator;
    public GameObject fullDamageIndicator;
    public GameObject explosion;
    public int explostionDuration;

    //Pirvate fields;
    private GameObject halfDamageIndicatorInstance;
    private GameObject fullDamageIndicationDurationInstance;
    private GameObject explosionInstance;
    private StatsController stats;

    //Unity methods
    void Start()
    {
        stats = GetComponent<StatsController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (stats.isOutOflevel)
        {
            if (!explosionInstance)
            {
                //Explosion is one time thing.
                explosionInstance = Instantiate(explosion, transform.position, Quaternion.identity);
                Destroy(explosionInstance, explostionDuration);
            }

        }
        else
        {
            if (stats.health == 0)
            {
                if (halfDamageIndicatorInstance)
                {
                    Destroy(halfDamageIndicatorInstance);
                }
                if (!fullDamageIndicationDurationInstance)
                {
                    fullDamageIndicationDurationInstance = Instantiate(fullDamageIndicator, transform.position, Quaternion.identity);
                }

            }
            if (stats.health > 0 && stats.health <= 50)
            {
                if (fullDamageIndicationDurationInstance)
                {
                    Destroy(fullDamageIndicationDurationInstance);
                }
                if (!halfDamageIndicatorInstance)
                {
                    halfDamageIndicatorInstance = Instantiate(halfDamageIndicator, transform.position, Quaternion.identity);
                }
            }
            if (stats.health > 50 && stats.health <= 100)
            {
                if (halfDamageIndicatorInstance)
                {
                    Destroy(halfDamageIndicatorInstance);
                }
                if (fullDamageIndicationDurationInstance)
                {
                    Destroy(fullDamageIndicationDurationInstance);
                }
            }
        }
        //If any of the indicators exist move them according to the local space.
        if (fullDamageIndicationDurationInstance)
        {
            fullDamageIndicationDurationInstance.transform.position = transform.position;
            fullDamageIndicationDurationInstance.transform.forward = transform.forward;
        }
        if (halfDamageIndicatorInstance)
        {
            halfDamageIndicatorInstance.transform.position = transform.position;
            halfDamageIndicatorInstance.transform.forward = transform.forward;
        }
        if (explosionInstance)
        {
            explosionInstance.transform.position = transform.position;
            explosionInstance.transform.forward = transform.forward;
        }
    }

    //Clean Up
    void OnDestroy()
    {
        if (explosionInstance)
        {
            Destroy(explosionInstance);
        }
        if (fullDamageIndicationDurationInstance)
        {
            Destroy(fullDamageIndicationDurationInstance);
        }
        if (halfDamageIndicatorInstance)
        {
            Destroy(halfDamageIndicatorInstance);
        }
    }
}

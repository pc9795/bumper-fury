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
                explosionInstance = Instantiate(explosion, transform.position, Quaternion.identity, transform);
                Destroy(explosionInstance, explostionDuration);
            }
        }
        else
        {
            if (stats.health <= 0)
            {
                if (halfDamageIndicatorInstance)
                {
                    Destroy(halfDamageIndicatorInstance);
                }
                if (!fullDamageIndicationDurationInstance)
                {
                    fullDamageIndicationDurationInstance =
                        Instantiate(fullDamageIndicator, transform.position, Quaternion.identity, transform);
                }
            }
            if (stats.IsHealthCritical())
            {
                if (fullDamageIndicationDurationInstance)
                {
                    Destroy(fullDamageIndicationDurationInstance);
                }
                if (!halfDamageIndicatorInstance)
                {
                    halfDamageIndicatorInstance =
                        Instantiate(halfDamageIndicator, transform.position, Quaternion.identity, transform);
                }
            }
            if (stats.IsHealthFine())
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

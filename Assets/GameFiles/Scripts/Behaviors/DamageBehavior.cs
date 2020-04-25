using UnityEngine;

public class DamageBehavior : MonoBehaviour
{
    //Public fields
    
    //Look for more narow object type
    //Expected to be looping particle system
    //Particle system when health is half damaged
    public GameObject halfDamageIndicator; 
    //Look for more narow object type
    //Expected to be looping particle system
    //Particle system when car is fully damaged
    public GameObject fullDamageIndicator; 
    //Look for more narow object type
    //Expected to be looping particle system
    //Particle system when car is out of the level
    public GameObject explosion;
    //Duration of the explosion
    public int explostionDuration;

    //Pirvate fields;
    
    //Instance of `halfDamageIndicator` particle system
    private GameObject halfDamageIndicatorInstance;
    //Instance of `fullDamageIndicator` particle system
    private GameObject fullDamageIndicationDurationInstance;
    //Instance of `explosion` particle system
    private GameObject explosionInstance;
    //Statistics of parent contenstant
    private StatsController stats;

    //Unity methods
    void Start()
    {
        stats = GetComponent<StatsController>();
    }

    // Update is called once per frame
    void Update()
    {
        //Check out of level.
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
            //Check death
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
            //Check critical health.(Possibly less than half)
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
            //Check health is fine or not.(Possibly more than half)
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

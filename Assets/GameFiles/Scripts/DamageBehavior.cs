using UnityEngine;

public class DamageBehavior : MonoBehaviour
{
    //Public fields
    public GameObject halfDamageIndicator;
    public int halfDamageIndicationDuration;
    public GameObject fullDamageIndicator;
    public int fullDamageIndicationDuration;

    //Pirvate fields;
    private GameObject halfDamageIndicatorInstance;
    private GameObject fullDamageIndicationDurationInstance;
    private StatsController stats;

    //Unity methods
    void Start()
    {
        stats = GetComponent<StatsController>();
    }

    // Update is called once per frame
    void Update()
    {
        //todo check for the conditions where both fires can run together.
        //Check before playing with the structure of if statements.
        if (stats.health == 0 && !fullDamageIndicationDurationInstance)
        {
            fullDamageIndicationDurationInstance = Instantiate(fullDamageIndicator, transform.position, Quaternion.identity);
            Destroy(fullDamageIndicationDurationInstance, fullDamageIndicationDuration);

        }
        if (stats.health > 0 && stats.health <= 50 && !halfDamageIndicatorInstance)
        {
            halfDamageIndicatorInstance = Instantiate(halfDamageIndicator, transform.position, Quaternion.identity);
            Destroy(halfDamageIndicatorInstance, halfDamageIndicationDuration);
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
    }
}

using UnityEngine;

public class AICar : MonoBehaviour
{
    private StatsController stats;

    void Start()
    {
        stats = GetComponent<StatsController>();
    }

    void Update()
    {
        //No updates if dead.
        if (!stats.isAlive())
        {
            return;
        }
        if (stats.isOutOflevel)
        {
            GameManager.INSTANCE.PushNotification(stats.displayName + " Eliminated!");
            Destroy(this.gameObject, GameManager.INSTANCE.deathTimer);
            stats.Die();
        }
    }
}

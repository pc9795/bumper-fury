using UnityEngine;

public class AICar : MonoBehaviour
{
    // Private fields
    private StatsController stats;
    private SimpleCarController carController;

    // Unity methods
    void Start()
    {
        stats = GetComponent<StatsController>();
        //Car controller will be in a model attached to this object.
        carController = GetComponentInChildren<SimpleCarController>();
    }

    void LateUpdate()
    {
        //No updates if dead.
        if (!stats.IsAlive())
        {
            return;
        }
        if (stats.isOutOflevel || stats.health == 0)
        {
            GameManager.INSTANCE.PushNotification(stats.displayName + " Eliminated!");
            Destroy(this.gameObject, GameManager.INSTANCE.deathTimer);
            stats.Die();
        }
        carController.Steer(0);
        carController.Move(0);
        carController.UpdateWheelPoses();
    }
}

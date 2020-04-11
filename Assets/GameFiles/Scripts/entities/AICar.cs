using UnityEngine;

public class AICar : MonoBehaviour
{
    //Public fields
    public GameObject modelPrefab;

    // Private fields
    private StatsController stats;
    private SimpleCarController carController;
    private bool initialized;
    private GameObject modelInstance;

    // Unity methods
    void Start()
    {
        gameObject.tag = GameManager.Tag.AI;
    }

    void LateUpdate()
    {
        if (!initialized)
        {
            return;
        }
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
        //TODO: add AI
        carController.Steer(0);
        carController.Move(0);
        carController.UpdateWheelPoses();
    }

    //Custom methods
    public void Init()
    {
        stats = GetComponent<StatsController>();
        modelInstance = Instantiate(modelPrefab, transform.position, Quaternion.identity, transform);
        carController = modelInstance.GetComponent<SimpleCarController>();
        initialized = true;
    }

}

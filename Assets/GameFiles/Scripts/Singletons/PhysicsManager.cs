using UnityEngine;

public class PhysicsManager : MonoBehaviour
{
    //Public variables
    public static PhysicsManager INSTANCE;
    public float collisionRelativeVelocityThreshold = 15f;
    public float collisionRelativeVectorZThreshold = 1.5f;
    public float flipThreshold = 0.70f;
    public float reversingThreshold = 0.90f;

    //Unity methods
    void Awake()
    {
        Init();
    }

    //Custom methods
    private void Init()
    {
        if (INSTANCE != null)
        {
            Destroy(gameObject);
        }
        else
        {
            INSTANCE = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}

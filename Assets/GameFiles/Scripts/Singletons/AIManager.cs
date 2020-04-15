using UnityEngine;

public class AIManager : MonoBehaviour
{
    //Public variables
    public static AIManager INSTANCE;
    public Bounds smartLevelBounds = new Bounds(new Vector3(0, 100, 0), new Vector3(85, 125, 85));

    //Private variables
    private GameObject[] wayPoints;

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

    public Vector3 GetRandWayPoint()
    {
        return wayPoints[Random.Range(0, wayPoints.Length)].transform.position;
    }

    public void LoadWaypoints(GameObject[] wayPoints)
    {
        this.wayPoints = wayPoints;
    }
}

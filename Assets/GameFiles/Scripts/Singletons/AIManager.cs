using UnityEngine;

public class AIManager : MonoBehaviour
{
    //Public variables
    public static AIManager INSTANCE;

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

    public Transform GetRandWayPoint()
    {
        return wayPoints[Random.Range(0, wayPoints.Length)].transform;
    }

    public void LoadWaypoints(GameObject[] wayPoints)
    {
        this.wayPoints = wayPoints;
    }

}

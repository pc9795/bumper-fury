using UnityEngine;

public class AirLevel : MonoBehaviour
{
    public GameObject stormPrefab;

    private GameObject stormInstance;
    //Unity methods
    void Start()
    {
        GameManager.INSTANCE.InitLevel();
    }

    void Update()
    {
        if (stormInstance)
        {
            return;
        }
        GameObject[] trapPoints = GameManager.INSTANCE.trapPoints;
        int randIndex = Random.Range(0, trapPoints.Length);
        GameObject trapPoint = trapPoints[randIndex];
        //Storm will destroy itself
        stormInstance = Instantiate(stormPrefab, trapPoint.transform.position, Quaternion.identity);
        stormInstance.transform.forward = trapPoint.transform.forward;
    }
}

using UnityEngine;

public class AirLevel : MonoBehaviour
{
    public GameObject stormPrefab;
    public float stormsTimeGap = 5;

    private GameObject stormInstance;
    private bool invoked;

    //Unity methods
    void Start()
    {
        GameManager.INSTANCE.InitLevel();
        invoked = true;
        CreateStorm();
    }

    void Update()
    {
        if (stormInstance || invoked)
        {
            return;
        }
        invoked = true;
        Invoke("CreateStorm", stormsTimeGap);
    }
    private void CreateStorm()
    {
        GameObject[] trapPoints = GameManager.INSTANCE.trapPoints;
        int randIndex = Random.Range(0, trapPoints.Length);
        GameObject trapPoint = trapPoints[randIndex];
        //Storm will destroy itself
        stormInstance = Instantiate(stormPrefab, trapPoint.transform.position, Quaternion.identity);
        stormInstance.transform.forward = trapPoint.transform.forward;
        invoked = false;
    }
}

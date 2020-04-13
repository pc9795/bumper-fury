using UnityEngine;

public class WaterLevel : MonoBehaviour
{
    //Public variables
    public int tornadoIntervals = 10;
    public GameObject tornadoPrefab;

    //Unity methods
    void Start()
    {
        GameManager.INSTANCE.InitLevel();
        InvokeRepeating("GenerateTornado", 10, 10);
    }

    //Custom methods
    private void GenerateTornado()
    {
        GameObject[] trapPoints = GameManager.INSTANCE.trapPoints;
        int randIndex = Random.Range(0, trapPoints.Length);
        Vector3 position = trapPoints[randIndex].transform.position;
        GameObject tornadoInstance = Instantiate(tornadoPrefab, position, Quaternion.identity);
        tornadoInstance.transform.forward = trapPoints[randIndex].transform.forward;
    }

}

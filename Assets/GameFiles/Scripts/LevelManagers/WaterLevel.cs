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
        InvokeRepeating("GenerateTornado", 0, tornadoIntervals);
    }

    //Custom methods
    private void GenerateTornado()
    {
        GameObject[] trapPoints = GameManager.INSTANCE.trapPoints;
        int randIndex = Random.Range(0, trapPoints.Length);
        GameObject trapPoint = trapPoints[randIndex];
        //Tornado will destroy itself.
        GameObject tornadoInstance = Instantiate(tornadoPrefab, trapPoint.transform.position, Quaternion.identity);
        tornadoInstance.transform.forward = trapPoint.transform.forward;
    }

}

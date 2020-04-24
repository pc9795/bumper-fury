using UnityEngine;

public class WaterLevel : MonoBehaviour
{
    //Public variables
    public GameObject tornadoPrefab;
    public int tornadoGap = 5;

    //Private variables
    private GameObject tornadoInstance;
    private bool invoked;

    //Unity methods
    void Start()
    {
        //Game starts at paused state because of level start screen.
        Time.timeScale = 0f;
        GameManager.INSTANCE.InitLevel();
        invoked = true;
        GenerateTornado();
    }

    void Update()
    {
        if (tornadoInstance || invoked)
        {
            return;
        }
        invoked = true;
        Invoke("GenerateTornado", tornadoGap);
    }

    //Custom methods
    private void GenerateTornado()
    {
        GameObject[] trapPoints = GameManager.INSTANCE.trapPoints;
        int randIndex = Random.Range(0, trapPoints.Length);
        GameObject trapPoint = trapPoints[randIndex];
        //Tornado will destroy itself.
        tornadoInstance = Instantiate(tornadoPrefab, trapPoint.transform.position, Quaternion.identity);
        invoked = false;
    }

}

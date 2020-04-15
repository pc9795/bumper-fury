using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthLevel : MonoBehaviour
{
    //Public fields
    public GameObject rockPrefab;
    public int rockShowerDurationInSecs = 5;
    public int rockStayDurationInSecs = 10;
    public int rockShowerIntervalInSecs = 10;
    public float rockMinSize = 0.1f;
    public float rockMaxSize = 0.5f;
    public int rockShowerRocksPerSecs = 2;

    //Private fields
    private bool rockShower;
    private GameObject rockShowerLocation;

    //Unity methods
    void Start()
    {
        GameManager.INSTANCE.InitLevel();
        InvokeRepeating("RockShower", 0, rockShowerIntervalInSecs);
    }

    //Custom methods
    private void RockShower()
    {
        if (rockShower)
        {
            return;
        }
        GameObject[] trapPoints = GameManager.INSTANCE.trapPoints;
        int randIndex = Random.Range(0, trapPoints.Length);
        rockShowerLocation = trapPoints[randIndex];
        StartCoroutine(RockShowerUtil());
    }

    private IEnumerator RockShowerUtil()
    {
        rockShower = true;
        float start = 0.0f;
        List<GameObject> rocks = new List<GameObject>();
        int noOfSeconds = 0;

        while (noOfSeconds < rockShowerDurationInSecs)
        {
            if (start > 1)
            {
                start -= 1;
                noOfSeconds++;
                for (int i = 0; i < rockShowerRocksPerSecs; i++)
                {
                    float scale = Random.Range(rockMinSize, rockMaxSize);
                    GameObject rockInstance = Instantiate(rockPrefab, rockShowerLocation.transform.position,
                        Quaternion.identity);
                    rockInstance.transform.localScale *= scale;
                    rocks.Add(rockInstance);
                }
            }
            start += Time.deltaTime;
            yield return null;
        }
        
        foreach (GameObject rock in rocks)
        {
            Destroy(rock, rockStayDurationInSecs);
        }
        rockShower = false;
    }
}

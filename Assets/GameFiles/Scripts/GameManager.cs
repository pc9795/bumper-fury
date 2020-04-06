using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager INSTANCE;
    private GameObject player;
    private GameObject[] aiCars;

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

    public GameObject GetPlayer()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        return player;

    }

    public GameObject[] GetAICars()
    {
        if (aiCars == null || aiCars.Length == 0)
        {
            aiCars = GameObject.FindGameObjectsWithTag("AI");
        }
        return aiCars;

    }


}

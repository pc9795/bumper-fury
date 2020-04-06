using UnityEngine;
using UnityEngine.UI;

public class PlayerScore : MonoBehaviour
{
    //Private fields
    private Text text;
    private StatsController playerStats;

    void Start()
    {
        text = GetComponent<Text>();
    }

    //Unity methods
    void Update()
    {
        //By default will print 0
        int score = 0;
        if (playerStats == null)
        {
            InitFromGameManager();
        }
        else
        {
            score = playerStats.score;
        }
        text.text = "Score: " + score;
    }

    //Custom methods
    private void InitFromGameManager()
    {
        GameObject player = GameManager.INSTANCE.GetPlayer();
        //Player is not initialized yet. Earyly Exit
        if (player == null)
        {
            return;
        }
        playerStats = player.GetComponent<StatsController>();
    }
}

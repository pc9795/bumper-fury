using UnityEngine;
using UnityEngine.UI;

public class PlayerScoreView : MonoBehaviour
{
    //Private fields
    private Text text;
    private PlayerStatsController playerStats;
 
    void Start()
    {
        text = GetComponent<Text>();
    }

    //Unity methods
    void Update()
    {
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
        if (player == null)
        {
            return;
        }
        playerStats = player.GetComponent<PlayerStatsController>();
    }
}

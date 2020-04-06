using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreboardController : MonoBehaviour
{
    //Public fields
    public GameObject listItem;
    public GameObject parentPanel;

    //Private fields
    PlayerStatsController playerStats;
    List<AICarController> aICars;
    List<GameObject> scoreBoardItems = new List<GameObject>();

    //Unity methods
    void LateUpdate()
    {
        if (playerStats == null || aICars == null)
        {
            InitFromGameManager();
            return;
        }
        List<ScoreBoardEntry> scores = new List<ScoreBoardEntry>();
        scores.Add(new ScoreBoardEntry("Player", playerStats.score));
        foreach (AICarController aiCar in aICars)
        {
            scores.Add(new ScoreBoardEntry(aiCar.id, aiCar.score));
        }
        scores.Sort(delegate (ScoreBoardEntry entry1, ScoreBoardEntry entry2) { return entry2.score - entry1.score; });
        CleanScoreboard();
        DisplayScoreboard(scores);
    }


    //Custom methods
    private class ScoreBoardEntry
    {
        //Not using getters and setters as class is private.
        public string name;
        public int score;

        public ScoreBoardEntry(string name, int score)
        {
            this.name = name;
            this.score = score;
        }
    }
    void InitFromGameManager()
    {
        GameObject player = GameManager.INSTANCE.GetPlayer();
        if (player == null)
        {
            return;
        }
        playerStats = player.GetComponent<PlayerStatsController>();
        GameObject[] aICarGameObjects = GameManager.INSTANCE.GetAICars();
        if (aICarGameObjects == null)
        {
            return;
        }
        aICars = new List<AICarController>();
        foreach (GameObject aiCarGameObject in aICarGameObjects)
        {
            aICars.Add(aiCarGameObject.GetComponentInParent<AICarController>());
        }
    }

    void CleanScoreboard()
    {
        foreach (GameObject scoreBoarItem in scoreBoardItems)
        {
            Destroy(scoreBoarItem);
        }
        scoreBoardItems = new List<GameObject>();
    }


    void DisplayScoreboard(List<ScoreBoardEntry> entries)
    {
        if (entries.Count < 0)
        {
            return;
        }
        for (int i = 0; i < entries.Count; i++)
        {
            ScoreBoardEntry entry = entries[i];
            GameObject listItemInstance = Instantiate(listItem);
            Text text = listItemInstance.GetComponent<Text>();
            text.text = (i + 1) + ". " + entry.name + " (" + entry.score + ")";
            listItemInstance.transform.SetParent(parentPanel.transform);
            scoreBoardItems.Add(listItemInstance);
        }

    }
}

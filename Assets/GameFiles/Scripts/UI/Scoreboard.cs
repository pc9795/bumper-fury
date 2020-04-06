using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Scoreboard : MonoBehaviour
{
    //Public fields
    //It should have a text component.
    public GameObject listItem;
    public GameObject parentPanel;

    //Private fields
    StatsController playerStats;
    Dictionary<string, StatsController> aICarStatsDict;
    List<GameObject> scoreBoardItems = new List<GameObject>();

    //Unity methods
    void LateUpdate()
    {
        if (playerStats == null || aICarStatsDict == null)
        {
            InitFromGameManager();
            return;
        }
        //Add player score
        List<ScoreBoardEntry> scores = new List<ScoreBoardEntry>();
        if (playerStats != null)
        {
            scores.Add(new ScoreBoardEntry("Player", playerStats.score));
        }
        else
        {
            //-1 is a special value which will result in printing K.O.
            scores.Add(new ScoreBoardEntry("Player", -1));
        }
        foreach (KeyValuePair<string, StatsController> keyValuePair in aICarStatsDict)
        {
            if (keyValuePair.Value != null)
            {
                scores.Add(new ScoreBoardEntry(keyValuePair.Key, keyValuePair.Value.score));
            }
            else
            {
                //-1 is a special value which will result in printing K.O.
                scores.Add(new ScoreBoardEntry(keyValuePair.Key, -1));
            }

        }
        //Descending order
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
        playerStats = player.GetComponent<StatsController>();
        GameObject[] aICarGameObjects = GameManager.INSTANCE.GetAICars();
        if (aICarGameObjects == null)
        {
            return;
        }
        aICarStatsDict = new Dictionary<string, StatsController>();
        foreach (GameObject aiCarGameObject in aICarGameObjects)
        {
            StatsController stats = aiCarGameObject.GetComponentInParent<StatsController>();
            aICarStatsDict.Add(stats.displayName, stats);
        }
    }

    private void CleanScoreboard()
    {
        foreach (GameObject scoreBoarItem in scoreBoardItems)
        {
            Destroy(scoreBoarItem);
        }
        scoreBoardItems = new List<GameObject>();
    }


    private void DisplayScoreboard(List<ScoreBoardEntry> entries)
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
            text.text = (i + 1) + ". " + entry.name + " (" + (entry.score != -1 ? "" + entry.score : "K.O.") + ")";
            listItemInstance.transform.SetParent(parentPanel.transform);
            scoreBoardItems.Add(listItemInstance);
        }
    }
}

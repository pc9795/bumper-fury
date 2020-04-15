using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Scoreboard : MonoBehaviour
{
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
    //Public fields
    public GameObject listItem; //It should have a text component.
    public GameObject parentPanel;

    //Private fields
    StatsController playerStats;
    string playerDisplayName;
    Dictionary<string, StatsController> aICarStatsDict;
    List<GameObject> scoreBoardItems = new List<GameObject>();

    //Unity methods
    void LateUpdate()
    {
        if (playerStats == null || aICarStatsDict == null)
        {
            LoadFromGameManager();
        }
        List<ScoreBoardEntry> scores = GetScores();
        //Sort in descending order
        scores.Sort(delegate (ScoreBoardEntry entry1, ScoreBoardEntry entry2) { return entry2.score - entry1.score; });
        CleanScoreboard();
        DisplayScoreboard(scores);
    }

    //Custom methods
    private List<ScoreBoardEntry> GetScores()
    {
        //Add player score
        List<ScoreBoardEntry> scores = new List<ScoreBoardEntry>();
        if (playerStats != null)
        {
            scores.Add(new ScoreBoardEntry(playerDisplayName, playerStats.score));
        }
        else
        {
            //-1 is a special value which will result in printing K.O.
            scores.Add(new ScoreBoardEntry(playerDisplayName, -1));
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
        return scores;
    }

    private void LoadFromGameManager()
    {
        GameObject player = GameManager.INSTANCE.GetPlayer();
        playerStats = player.GetComponent<StatsController>();
        //We are storing the player display name because in future if we decide that game object of the player will be destroyed 
        //on loosing/death then the `playerStats` will return null and we can't access the display name to show.
        playerDisplayName = playerStats.displayName;
        GameObject[] aICarGameObjects = GameManager.INSTANCE.GetAICars();
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

    public bool IsPlayerWinning()
    {
        return IsCharacterWinning(playerDisplayName);
    }

    public bool IsCharacterWinning(string displayName)
    {
        List<ScoreBoardEntry> scores = GetScores();
        string winner = "";
        int maxScore = -1;
        foreach (ScoreBoardEntry entry in scores)
        {
            if (entry.score > maxScore)
            {
                maxScore = entry.score;
                winner = entry.name;
            }
        }

        return winner.Equals(displayName);
    }
}

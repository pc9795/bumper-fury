using UnityEngine;
using UnityEngine.UI;

public class LevelTimer : MonoBehaviour
{
    //Private fields
    private Text text; //Should be applied to a text element
    private int timeLeftInSecs;

    //Unity methods
    void Start()
    {
        text = GetComponent<Text>();
        timeLeftInSecs = GameManager.INSTANCE.levelLengthInSeconds;
        switch (GameManager.INSTANCE.difficulty)
        {
            case GameManager.Difficulty.EASY:
                break;
            case GameManager.Difficulty.MEDIUM:
                break;
            case GameManager.Difficulty.HARD:
                timeLeftInSecs = (int)(0.8f * timeLeftInSecs);
                break;
        }
        InvokeRepeating("UpdateTimer", 1, 1);
    }

    //Custom methods
    private void UpdateTimer()
    {
        if (timeLeftInSecs == 0)
        {
            return;
        }
        text.text = "" + timeLeftInSecs--;
    }

    public int GetTimeLeftInSecs()
    {
        return timeLeftInSecs;
    }
}

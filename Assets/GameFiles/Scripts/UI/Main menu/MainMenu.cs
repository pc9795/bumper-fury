using UnityEngine;

public class MainMenu : MonoBehaviour
{
    //Custom methods
    public void Quit()
    {
        Application.Quit();
    }

    public void StartEasyGame()
    {
        GameManager.INSTANCE.difficulty = GameManager.Difficulty.EASY;
    }

    public void StartNormalGame()
    {
        GameManager.INSTANCE.difficulty = GameManager.Difficulty.MEDIUM;
    }

    public void StartHardGame()
    {
        GameManager.INSTANCE.difficulty = GameManager.Difficulty.HARD;
    }

    public void ButtonSound()
    {
        AudioManager.INSTANCE.Play("Button Click");
    }
}

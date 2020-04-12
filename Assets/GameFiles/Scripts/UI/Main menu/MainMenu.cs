using UnityEngine;

public class MainMenu : MonoBehaviour
{
    void Start()
    {
        AudioManager.INSTANCE.Play(GameManager.INSTANCE.mainMenu.theme);
    }
    //Custom methods
    public void Quit()
    {
        Application.Quit();
    }

    public void SetEasyDifficulty()
    {
        GameManager.INSTANCE.difficulty = GameManager.Difficulty.EASY;
    }

    public void SetNormalDifficulty()
    {
        GameManager.INSTANCE.difficulty = GameManager.Difficulty.MEDIUM;
    }

    public void SetHardDifficulty()
    {
        GameManager.INSTANCE.difficulty = GameManager.Difficulty.HARD;
    }

    public void ButtonSound()
    {
        AudioManager.INSTANCE.Play("Button Click");
    }

    public void StartGame()
    {
        GameManager.INSTANCE.StartGame();
    }
}

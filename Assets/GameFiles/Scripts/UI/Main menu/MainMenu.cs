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

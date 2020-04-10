using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public void Story()
    {

    }

    public void Battle()
    {

    }

    public void Quit()
    {
        Application.Quit();
    }

    public void StartEasyGame()
    {
        GameManager.INSTANCE.SetDifficulty(GameManager.Difficulty.EASY);
    }

    public void StartNormalGame()
    {
        GameManager.INSTANCE.SetDifficulty(GameManager.Difficulty.MEDIUM);
    }

    public void StartHardGame()
    {
        GameManager.INSTANCE.SetDifficulty(GameManager.Difficulty.HARD);
    }
}

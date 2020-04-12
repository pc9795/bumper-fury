using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    //Public fields
    public bool paused;
    public GameObject pauseMenu;
    public GameObject gameOverMenu;

    //Private fields
    StatsController playerStats;

    void Update()
    {
        if (playerStats == null)
        {
            InitFromGameManager();
            return;
        }

        if (!playerStats.IsAlive())
        {
            Time.timeScale = 0f;
            gameOverMenu.SetActive(true);
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (paused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    //Custom methods
    public void QuitToMainMenu()
    {
        Time.timeScale = 1f;
        GameManager.INSTANCE.MainMenu();
    }

    public void Pause()
    {
        paused = true;
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Resume()
    {
        paused = false;
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void ButtonSound()
    {
        AudioManager.INSTANCE.Play("Button Click");
    }

    private void InitFromGameManager()
    {
        GameObject player = GameManager.INSTANCE.GetPlayer();
        if (player == null)
        {
            return;
        }
        playerStats = player.GetComponent<StatsController>();
    }
}

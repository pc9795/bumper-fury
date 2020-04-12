using UnityEngine;

//TODO change the name as it isa acting as a Level manager. Also look for merging the `Level` class inside it.
public class PauseMenu : MonoBehaviour
{
    //Public fields
    public bool paused;
    public GameObject pauseScreen;
    public GameObject gameOverScreen;
    public GameObject levelTimerElem;
    public GameObject roundWonScreen;
    public GameObject scoreBoardElem;

    //Private fields
    private StatsController playerStats;
    private LevelTimer levelTimer;
    private Scoreboard scoreboard;

    //Unity methods
    void Start()
    {
        levelTimer = levelTimerElem.GetComponent<LevelTimer>();
        scoreboard = scoreBoardElem.GetComponent<Scoreboard>();
        //Level will start in a paused state.
        Time.timeScale = 0f;
        AudioManager.INSTANCE.Play("Engine");
    }

    void Update()
    {
        if (playerStats == null)
        {
            GameObject player = GameManager.INSTANCE.GetPlayer();
            playerStats = player.GetComponent<StatsController>();
        }

        //Round finished.
        if (levelTimer.GetTimeLeftInSecs() == 0)
        {
            Time.timeScale = 0f;
            AudioManager.INSTANCE.Stop("Engine");
            if (scoreboard.DoPlayerWon())
            {
                roundWonScreen.SetActive(true);
            }
            else
            {
                gameOverScreen.SetActive(true);
            }
        }
        //Player died
        else if (!playerStats.IsAlive())
        {
            AudioManager.INSTANCE.Stop("Engine");
            Time.timeScale = 0f;
            gameOverScreen.SetActive(true);
        }
        //Check pause is requested.
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
        AudioManager.INSTANCE.Stop("Engine");
        paused = true;
        pauseScreen.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Resume()
    {
        AudioManager.INSTANCE.Play("Engine");
        paused = false;
        pauseScreen.SetActive(false);
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

    public void NextLevel()
    {
        Time.timeScale = 1f;
        GameManager.INSTANCE.NextLevel();
    }

}

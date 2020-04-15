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
    public GameObject dPadConfigElem;
    public GameObject dPadElem;
    public GameObject touchButtonHolderElem;
    public GameObject controlsDescDesktopElem;
    public GameObject controlsDescHandhelElem;

    //Private fields
    private StatsController playerStats;
    private LevelTimer levelTimer;
    private Scoreboard scoreboard;

    //Unity methods
    void Start()
    {
        levelTimer = levelTimerElem.GetComponent<LevelTimer>();
        scoreboard = scoreBoardElem.GetComponent<Scoreboard>();
        
        //TODO uncomment
        //Level will start in a paused state.
        //Time.timeScale = 0f;
        
        AudioManager.INSTANCE.Play("Engine");

        if (SystemInfo.deviceType == DeviceType.Handheld)
        {
            dPadConfigElem.SetActive(true);
            touchButtonHolderElem.SetActive(true);
        }
    }

    void Update()
    {
        if (playerStats == null)
        {
            GameObject player = GameManager.INSTANCE.GetPlayer();
            playerStats = player.GetComponent<StatsController>();
        }

        //Round finished.
        if (levelTimer.GetTimeLeftInSecs() == 0 || GameManager.INSTANCE.AIComponentsLeft() == 0)
        {
            Time.timeScale = 0f;
            AudioManager.INSTANCE.Stop(AudioManager.AudioTrack.ENGINE);
            if (scoreboard.IsPlayerWinning())
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
            AudioManager.INSTANCE.Stop(AudioManager.AudioTrack.ENGINE);
            Time.timeScale = 0f;
            gameOverScreen.SetActive(true);
        }
        //Check pause is requested.
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (paused)
            {
                //Handle the case whent the game is paused but we are navigating other screens from the pause menu.
                if (pauseScreen.activeSelf)
                {
                    Resume();
                }
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
        AudioManager.INSTANCE.Stop(AudioManager.AudioTrack.ENGINE);
        paused = true;
        pauseScreen.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Resume()
    {
        AudioManager.INSTANCE.Play(AudioManager.AudioTrack.ENGINE);
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
        AudioManager.INSTANCE.Play(AudioManager.AudioTrack.BUTTON_CLICK);
    }

    public void NextLevel()
    {
        Time.timeScale = 1f;
        GameManager.INSTANCE.NextLevel();
    }

    public void ToggleDpad()
    {
        GameManager.INSTANCE.useDpad = !GameManager.INSTANCE.useDpad;
        dPadElem.SetActive(!dPadElem.activeSelf);
    }

    public void ToggleControlDesc(bool value)
    {
        if (SystemInfo.deviceType == DeviceType.Desktop)
        {
            controlsDescDesktopElem.SetActive(value);
        }
        else if (SystemInfo.deviceType == DeviceType.Handheld)
        {
            controlsDescHandhelElem.SetActive(value);
        }
    }
}

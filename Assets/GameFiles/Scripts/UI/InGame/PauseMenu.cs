using UnityEngine;
using UnityEngine.UI;

//REF: https://www.youtube.com/watch?v=JivuXdrIHK0
//I referenced about how to create a Pause menu from the above mentioned video

//This class is doing much more than what it is supposed to do(managing pause). So change its name in future or break this 
//class into more to divide the responsibilites.
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
    public GameObject retryBtn;

    //Private fields
    private StatsController playerStats;
    private LevelTimer levelTimer;
    private Scoreboard scoreboard;

    //Unity methods
    void Start()
    {
        levelTimer = levelTimerElem.GetComponent<LevelTimer>();
        scoreboard = scoreBoardElem.GetComponent<Scoreboard>();

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
                HandleRetry();
                gameOverScreen.SetActive(true);
            }
        }
        //Player died
        else if (!playerStats.IsAlive())
        {
            AudioManager.INSTANCE.Stop(AudioManager.AudioTrack.ENGINE);
            Time.timeScale = 0f;
            HandleRetry();
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
    private void HandleRetry()
    {
        if (!GameManager.INSTANCE.CanRetry())
        {
            Button button = retryBtn.GetComponent<Button>();
            button.interactable = false;
            Text text = retryBtn.GetComponentInChildren<Text>();
            text.text = "No Retries Left";
        }
        else
        {
            Text text = retryBtn.GetComponentInChildren<Text>();
            text.text = "Retries Left: " + GameManager.INSTANCE.GetRetiresLeft();
        }
    }
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

    public void Retry()
    {
        GameManager.INSTANCE.Retry();
    }
}

using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public class Notification
    {
        private string _message;
        private float _time;

        public Notification(string message, float time)
        {
            _message = message;
            _time = time;
        }

        public string message { get { return _message; } }
        public float time { get { return _time; } }
    }

    public enum Difficulty
    {
        EASY, MEDIUM, HARD
    }

    public class Level
    {
        private string _sceneName;

        public Level(string sceneName)
        {
            _sceneName = sceneName;
        }
        public string sceneName { get { return _sceneName; } }
    }

    //Public fields
    //TODO check a way to make it configurable
    //The bounds is shifted upwards so that things can be in air and still be considered inside level.
    public Bounds levelBounds = new Bounds(new Vector3(0, 50, 0), new Vector3(100, 100, 100));
    public int deathTimer = 3;
    public static GameManager INSTANCE;
    [HideInInspector]
    public Difficulty difficulty { get; set; }
    [HideInInspector]
    public string selectedCarName { get; set; }
    [HideInInspector]
    public string selectedElementName { get; set; }

    //Private fields
    private GameObject player;
    private GameObject[] aiCars;
    private Queue<Notification> messageQueue = new Queue<Notification>();
    private List<Level> levels = new List<Level>();
    private int currLevel = 0;
    private Level mainMenu;

    //Unity methods
    void Awake()
    {
        Init();
        //Can look on better way to configure this.
        levels.Add(new Level("FireLevel"));
        levels.Add(new Level("WaterLevel"));
        levels.Add(new Level("AirLevel"));
        levels.Add(new Level("EarthLevel"));
        levels.Add(new Level("FinalLevel"));

        mainMenu = new Level("Main Menu");
    }

    //Custom methods
    private void Init()
    {
        if (INSTANCE != null)
        {
            Destroy(gameObject);
        }
        else
        {
            INSTANCE = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public GameObject GetPlayer()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        return player;

    }

    public GameObject[] GetAICars()
    {
        if (aiCars == null || aiCars.Length == 0)
        {
            aiCars = GameObject.FindGameObjectsWithTag("AI");
        }
        return aiCars;
    }

    public void PushNotification(string message)
    {
        messageQueue.Enqueue(new Notification(message, Time.time));
    }

    public Notification PollNotification()
    {
        if (messageQueue.Count == 0)
        {
            return null;
        }
        return messageQueue.Dequeue();
    }

    public void StartGame()
    {
        SceneManager.LoadScene(levels[0].sceneName);
    }

    public void NextLevel()
    {
        currLevel++;
        currLevel %= levels.Count;
        SceneManager.LoadScene(levels[currLevel].sceneName);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(mainMenu.sceneName);
    }
}

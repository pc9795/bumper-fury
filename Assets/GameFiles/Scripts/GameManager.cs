using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    //Publid fields
    //TODO check a way to make it configurable
    //The bounds is shifted upwards so that things can be in air and still be considered inside level.
    public Bounds levelBounds = new Bounds(new Vector3(0, 50, 0), new Vector3(100, 100, 100));
    public int deathTimer = 3;
    public static GameManager INSTANCE;

    //Private fields
    private GameObject player;
    private GameObject[] aiCars;
    private Queue<Notification> messageQueue = new Queue<Notification>();
    private Difficulty difficulty;

    //Unity methods
    void Awake()
    {
        Init();
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

    public enum Difficulty
    {
        EASY, MEDIUM, HARD
    }

    public void SetDifficulty(Difficulty difficulty)
    {
        this.difficulty = difficulty;
    }

    public Difficulty GetDifficulty()
    {
        return difficulty;
    }
}

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

    public class Tag
    {
        public static string PLAYER = "Player";
        public static string AI = "AI";
        public static string SPAWN_POINT = "Spawn Point";
        public static string ITEM_POINT = "Item Point";
        public static string TRAP_POINT = "Trap Point";
    }

    public class Level
    {
        private string _sceneName;
        private string _theme;

        public Level(string sceneName, string theme)
        {
            _sceneName = sceneName;
            _theme = theme;
        }
        public string sceneName { get { return _sceneName; } }
        public string theme { get { return _theme; } }
    }

    [System.Serializable]
    public class Car
    {
        public GameObject modelPrefab;
        public GameObject playablePrefab;
        public string name;
    }

    [System.Serializable]
    public class Elemental
    {
        public GameObject prefab;
        public GameObject powerPrefab;
        public string name;
    }

    //Public fields
    //TODO check a way to make it configurable
    //The bounds is shifted upwards so that things can be in air and still be considered inside level.
    public Bounds levelBounds = new Bounds(new Vector3(0, 50, 0), new Vector3(100, 100, 100));
    public int deathTimer = 3;
    public static GameManager INSTANCE;
    [HideInInspector]
    public Difficulty difficulty { get; set; }
    public List<Car> cars = new List<Car>();
    public List<Elemental> elementals = new List<Elemental>();
    [HideInInspector]
    public int selectedCarIndex;
    [HideInInspector]
    public int selectedElementalIndex;
    [HideInInspector]
    public Level mainMenu;
    public int countOfOpponents = 4;
    [HideInInspector]
    public bool inGame;

    //Private fields
    private GameObject player;
    private GameObject[] aiCars;
    private Queue<Notification> messageQueue = new Queue<Notification>();
    private List<Level> levels = new List<Level>();
    private int currLevel = 0;
    private GameObject[] spawnPoints;
    private GameObject[] trapPoints;
    private GameObject[] itemPoints;
    private string[] aiNames = { "Cormac", "Wilhelm", "Tyrel", "Ivan", "Seth", "Viktor", "Austin", "Roy",
        "Warrick","Carter","August","Benedict","Cyan","Valen","Zared","Daron","Finlay","Kynon","Jordan","Xerxes" };
    private int aiNameIndex;

    //Unity methods
    void Awake()
    {
        Init();

        //Configuring levels.
        //Can look on better way to configure this.
        levels.Add(new Level("FireLevel", "EDM 1"));
        levels.Add(new Level("WaterLevel", "EDM 2"));
        levels.Add(new Level("AirLevel", "EDM 3"));
        levels.Add(new Level("EarthLevel", "EDM 1"));
        levels.Add(new Level("GameEnding","Charged Up"));
        mainMenu = new Level("Main Menu", "Theme");
    }

    void Start()
    {
        // AudioManager.INSTANCE.Play(levels[1].theme);
        // inGame = true;
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
        return player;

    }

    public GameObject[] GetAICars()
    {
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
        AudioManager.INSTANCE.Stop(mainMenu.theme);
        SceneManager.LoadScene(levels[0].sceneName);
        AudioManager.INSTANCE.Play(levels[0].theme);
        inGame = true;
    }

    public void InitLevel()
    {
        spawnPoints = GameObject.FindGameObjectsWithTag(Tag.SPAWN_POINT);
        trapPoints = GameObject.FindGameObjectsWithTag(Tag.TRAP_POINT);
        itemPoints = GameObject.FindGameObjectsWithTag(Tag.ITEM_POINT);
        LoadPlayer();
        LoadAIOpponents();
        PlaceObjectsOnSpawnPoints();
        ItemManager.INSTANCE.LoadItems(itemPoints);
    }

    public void NextLevel()
    {
        AudioManager.INSTANCE.Stop(levels[currLevel].theme);
        currLevel++;
        currLevel %= levels.Count;
        SceneManager.LoadScene(levels[currLevel].sceneName);
        AudioManager.INSTANCE.Play(levels[currLevel].theme);
    }

    public void MainMenu()
    {
        AudioManager.INSTANCE.Stop(levels[currLevel].theme);
        SceneManager.LoadScene(mainMenu.sceneName);
        //Becuase there is no load/save functionality therefore all the progress will be lost. So setting the level to 0 again.
        currLevel = 0;
        AudioManager.INSTANCE.Play(mainMenu.theme);
        inGame = false;
    }

    //TODO: ponder
    public int GetScoreFromDamage(int damage)
    {
        return damage / 2;
    }

    //TODO: ponder
    public int GetEnergyFromDamage(int damage)
    {
        return damage * 5;
    }

    private void LoadPlayer()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        PlayerCar car = player.GetComponent<PlayerCar>();

        ElementalContainer elementalContainer = car.GetComponent<ElementalContainer>();
        elementalContainer.element = elementals[selectedElementalIndex].prefab;
        elementalContainer.Init();

        //No need to init as projectile instance is not created till the player is initialized. So there is no risk of 
        //updates.
        ProjectileShooter projectileShooter = car.GetComponent<ProjectileShooter>();
        projectileShooter.projectileType = elementals[selectedElementalIndex].powerPrefab;

        car.modelPrefab = cars[selectedCarIndex].playablePrefab;
        car.Init();
        NitroBehavior[] nitroBehaviors = car.GetComponentsInChildren<NitroBehavior>();
        foreach (NitroBehavior nitroBehavior in nitroBehaviors)
        {
            nitroBehavior.Init();
        }
    }

    private void LoadAIOpponents()
    {
        GameObject aITempalte = GameObject.FindGameObjectWithTag("AI");
        aiCars = new GameObject[countOfOpponents];

        int elementIndex = Random.Range(0, elementals.Count);
        int carIndex = Random.Range(0, cars.Count);

        for (int i = 0; i < aiCars.Length; i++)
        {
            aiCars[i] = Instantiate(aITempalte, aITempalte.transform.position, Quaternion.identity);
            AICar car = aiCars[i].GetComponent<AICar>();

            //Template doesn't have gravity by default
            Rigidbody rigidbody = car.GetComponent<Rigidbody>();
            rigidbody.useGravity = true;

            //Setting name of the ai character.
            StatsController stats = car.GetComponent<StatsController>();
            stats.displayName = aiNames[aiNameIndex];
            //Cycle names
            aiNameIndex++;
            aiNameIndex %= aiNames.Length;

            ElementalContainer elementalContainer = car.GetComponent<ElementalContainer>();
            elementalContainer.element = elementals[elementIndex].prefab;
            elementalContainer.Init();

            //No need to init as projectile instance is not created till the player is initialized. So there is no risk of 
            //updates.
            ProjectileShooter projectileShooter = car.GetComponent<ProjectileShooter>();
            projectileShooter.projectileType = elementals[elementIndex].powerPrefab;

            car.modelPrefab = cars[carIndex].playablePrefab;
            car.Init();
            NitroBehavior[] nitroBehaviors = car.GetComponentsInChildren<NitroBehavior>();
            foreach (NitroBehavior nitroBehavior in nitroBehaviors)
            {
                nitroBehavior.Init();
            }

            //Cycle elementals and models
            carIndex++;
            carIndex %= cars.Count;
            elementIndex++;
            elementIndex %= elementals.Count;
        }
    }

    private void PlaceObjectsOnSpawnPoints()
    {
        if (spawnPoints == null || spawnPoints.Length < (aiCars.Length + 1))
        {
            print("The no of spawn points are less than the no of objects to place.");
            return;
        }

        int spawnIndex = Random.Range(0, spawnPoints.Length);
        player.transform.position = spawnPoints[spawnIndex].transform.position;
        player.transform.forward = spawnPoints[spawnIndex].transform.forward;
        spawnIndex++;
        spawnIndex %= spawnPoints.Length;

        foreach (GameObject aiCar in aiCars)
        {
            aiCar.transform.position = spawnPoints[spawnIndex].transform.position;
            aiCar.transform.forward = spawnPoints[spawnIndex].transform.forward;
            //Cycle spawn points
            spawnIndex++;
            spawnIndex %= spawnPoints.Length;
        }

    }
}

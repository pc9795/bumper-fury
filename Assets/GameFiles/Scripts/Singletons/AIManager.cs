using UnityEngine;
using System.Collections.Generic;

public class AIManager : MonoBehaviour
{
    public class WayPoint
    {
        private Transform _transform;
        private Vector3 _vector;
        private bool _containsTransform;
        private WayPointType _wayPointType;

        public WayPoint(Transform transform, WayPointType wayPointType)
        {
            _transform = transform;
            _vector = Vector3.zero;
            _containsTransform = true;
            _wayPointType = wayPointType;
        }

        public WayPoint(Vector3 vector, WayPointType wayPointType)
        {
            _transform = null;
            _vector = vector;
            _containsTransform = false;
            _wayPointType = wayPointType;
        }

        public Transform transform { get { return _transform; } }
        public Vector3 vector { get { return _vector; } }
        public bool containsTransform { get { return _containsTransform; } }
        public WayPointType wayPointType { get { return _wayPointType; } }
    }

    public enum WayPointType
    {
        WAYPOINT, PLAYER, AI_CAR, HEALTH, ENERGY, NITRO, SAFTEY
    }

    //Public variables
    public static AIManager INSTANCE;
    public Bounds smartLevelBounds = new Bounds(new Vector3(0, 100, 0), new Vector3(85, 125, 85));
    public int maxAttackingPlayers;
    public float playerAttackingProb = 0.33f;
    public float otherAIAttackcingProb = 0.33f;
    public float sensorLength = 3f;
    public float sensorAngle = 30;

    //Private variables
    private GameObject[] wayPoints;
    private Dictionary<string, WayPointType> wayPointTracker = new Dictionary<string, WayPointType>();
    private int attackingPlayers;

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

    private void CheckAndDecrementAttackingPlayersCount(string identifier)
    {
        if (!wayPointTracker.ContainsKey(identifier))
        {
            return;
        }
        if (wayPointTracker[identifier] != WayPointType.PLAYER)
        {
            return;
        }
        attackingPlayers--;
    }

    public WayPoint GetSafteyPoint(string identifier)
    {
        CheckAndDecrementAttackingPlayersCount(identifier);
        wayPointTracker[identifier] = WayPointType.SAFTEY;
        return new WayPoint(Vector3.zero, WayPointType.SAFTEY);
    }

    public WayPoint GetRandWayPoint(string identifier)
    {
        CheckAndDecrementAttackingPlayersCount(identifier);
        wayPointTracker[identifier] = WayPointType.WAYPOINT;
        return new WayPoint(wayPoints[Random.Range(0, wayPoints.Length)].transform, WayPointType.WAYPOINT);
    }

    private bool CanAttackPlayer(string identifier)
    {
        bool attackingPlayerPreviously = false;
        if (wayPointTracker.ContainsKey(identifier))
        {
            attackingPlayerPreviously = wayPointTracker[identifier] == WayPointType.PLAYER;
        }
        return !attackingPlayerPreviously && (attackingPlayers != maxAttackingPlayers);
    }

    public WayPoint GetPlayerLocation(string identifier)
    {
        //No need to check for decrementing attacking players as an AI car can't target a player twice continuously.
        if (!CanAttackPlayer(identifier))
        {
            return null;
        }
        attackingPlayers++;
        return new WayPoint(GameManager.INSTANCE.GetPlayer().transform, WayPointType.PLAYER);
    }

    public WayPoint GetRandomAICarLocation(string identifier)
    {
        CheckAndDecrementAttackingPlayersCount(identifier);
        List<GameObject> aiCars = GameManager.INSTANCE.GetAliveAICars();
        return new WayPoint(aiCars[Random.Range(0, aiCars.Count)].transform, WayPointType.AI_CAR);
    }

    public WayPoint GetEnergyLocation(string identifier)
    {
        CheckAndDecrementAttackingPlayersCount(identifier);
        Transform transform = ItemManager.INSTANCE.GetItemLocation(Item.ItemType.ENERGY_BOOST);
        if (transform == null)
        {
            return null;
        }
        return new WayPoint(transform, WayPointType.ENERGY);
    }

    public WayPoint GetHealthLocation(string identifier)
    {
        CheckAndDecrementAttackingPlayersCount(identifier);
        Transform transform = ItemManager.INSTANCE.GetItemLocation(Item.ItemType.HEALTH_BOOST);
        if (transform == null)
        {
            return null;
        }
        return new WayPoint(transform, WayPointType.HEALTH);
    }

    public WayPoint GetNitroLocation(string identifier)
    {
        CheckAndDecrementAttackingPlayersCount(identifier);
        Transform transform = ItemManager.INSTANCE.GetItemLocation(Item.ItemType.SPEED_BOOST);
        if (transform == null)
        {
            return null;
        }
        return new WayPoint(transform, WayPointType.NITRO);
    }

    //This method is intended to called at the start of the level by game manager.
    public void LoadWaypoints(GameObject[] wayPoints)
    {
        this.wayPoints = wayPoints;
    }

    public bool IsObstacle(Collider collider)
    {
        if (collider.CompareTag(GameManager.Tag.OBSTACLE))
        {
            return true;
        }
        if (collider.GetComponent<Lava>() != null)
        {
            return true;
        }
        if (collider.GetComponent<Barel>() != null)
        {
            return true;
        }
        if (collider.GetComponent<Oil>() != null)
        {
            return true;
        }
        return false;
    }

    public bool IsShootable(Collider collider)
    {
        if (collider.GetComponentInParent<PlayerCar>() != null)
        {
            return true;
        }
        if (collider.GetComponentInParent<AICar>() != null)
        {
            return true;
        }
        return false;
    }
}

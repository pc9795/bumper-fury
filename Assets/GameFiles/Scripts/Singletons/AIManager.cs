using UnityEngine;
using System.Collections.Generic;
using System;

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
    public int maxAttacking = 1;
    public float playerAttackingProb = 0.33f;
    public float otherAIAttackcingProb = 0.33f;
    public float sensorLength = 12;
    public float sensorAngle = 45;
    public float smartBoundarySpeed = 5;
    public float stormSpeed = 5;
    public float maxSteerAngle = 20;
    public float motorForce = 500;
    public float invisibleBoundarySpeed = 2;
    public float wayPointReachingThresholdInSecs = 10;
    public float flipThreshold = 0.70f;
    public float reversingThreshold = 0;
    public float speedCheckTimeIntervalInSecs = 3;
    public float stuckThreshold = 1f;
    public float wayPointDistanceThreshold = 1;
    public float damageMultiplier = 1.5f;

    //Needed this class as we can't view normal dictionaries in Unity inspector.
    [Serializable]
    public class DictionaryInfo
    {
        public string key;
        public string value;

        public DictionaryInfo(string key, string value)
        {
            this.key = key;
            this.value = value;
        }
    }
    public List<DictionaryInfo> _attackedInfo;
    public List<DictionaryInfo> _attackingInfo;

    //Private variables
    private GameObject[] wayPoints;
    //Right now this dictionary is not used anywhere but can be used in future or could be removed.
    //What is the current way point type for the AI. Identifying using StatsController's displayname.
    private Dictionary<string, WayPointType> wayPointTracker = new Dictionary<string, WayPointType>();
    //Who is getting attacked by how many. Identifying using StatsController's displayname.
    private Dictionary<string, int> attackedInfo = new Dictionary<string, int>();
    //Who is attacking who. Identifying using StatsController's displayname.
    private Dictionary<string, string> attackingInfo = new Dictionary<string, string>();
    private string playerDisplayName;

    //Unity methods
    void Awake()
    {
        Init();
    }

    void Update()
    {
        _attackedInfo = new List<DictionaryInfo>();
        foreach (KeyValuePair<string, int> keyValue in attackedInfo)
        {
            _attackedInfo.Add(new DictionaryInfo(keyValue.Key, "" + keyValue.Value));
        }
        _attackingInfo = new List<DictionaryInfo>();
        foreach (KeyValuePair<string, string> keyValue in attackingInfo)
        {
            _attackingInfo.Add(new DictionaryInfo(keyValue.Key, keyValue.Value));
        }
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

    public void Refresh()
    {
        wayPoints = GameManager.INSTANCE.wayPoints;
        wayPointTracker = new Dictionary<string, WayPointType>();
        attackedInfo = new Dictionary<string, int>();
        attackingInfo = new Dictionary<string, string>();

        List<string> participants = new List<string>();
        participants.Add(GameManager.INSTANCE.GetPlayer().GetComponent<StatsController>().displayName);
        playerDisplayName = participants[0];
        foreach (GameObject aiCar in GameManager.INSTANCE.GetAICars())
        {
            participants.Add(aiCar.GetComponent<StatsController>().displayName);
        }
        foreach (string participant in participants)
        {
            //Just initial value will be replaced at first selection.
            wayPointTracker.Add(participant, WayPointType.WAYPOINT);
            attackedInfo.Add(participant, 0);
            attackingInfo.Add(participant, null);
        }
    }

    public void NotifyDeath(string identifier)
    {
        CheckAndDecrementAttackingCount(identifier);
    }

    private void CheckAndDecrementAttackingCount(string identifier)
    {
        string attacked = attackingInfo[identifier];
        if (attacked == null)
        {
            return;
        }
        attackedInfo[attacked] = attackedInfo[attacked] - 1;
        attackingInfo[identifier] = null;
    }

    public WayPoint GetSafteyPoint(string identifier)
    {
        CheckAndDecrementAttackingCount(identifier);
        wayPointTracker[identifier] = WayPointType.SAFTEY;
        return new WayPoint(Vector3.zero, WayPointType.SAFTEY);
    }

    public WayPoint GetRandWayPoint(string identifier)
    {
        CheckAndDecrementAttackingCount(identifier);
        wayPointTracker[identifier] = WayPointType.WAYPOINT;
        return new WayPoint(wayPoints[UnityEngine.Random.Range(0, wayPoints.Length)].transform, WayPointType.WAYPOINT);
    }

    public WayPoint GetPlayerLocation(string identifier)
    {
        CheckAndDecrementAttackingCount(identifier);
        if (attackedInfo[playerDisplayName] >= maxAttacking)
        {
            return null;
        }
        wayPointTracker[identifier] = WayPointType.PLAYER;
        attackingInfo[identifier] = playerDisplayName;
        attackedInfo[playerDisplayName]++;
        return new WayPoint(GameManager.INSTANCE.GetPlayer().transform, WayPointType.PLAYER);
    }

    public WayPoint GetRandomAICarLocation(string identifier)
    {
        CheckAndDecrementAttackingCount(identifier);
        List<GameObject> aiCars = GameManager.INSTANCE.GetAliveAICars();
        foreach (GameObject aiCar in aiCars)
        {
            string displayName = aiCar.GetComponent<StatsController>().displayName;
            //Won't attack self.
            if (displayName.Equals(identifier))
            {
                continue;
            }
            if (attackedInfo[displayName] < maxAttacking)
            {
                wayPointTracker[identifier] = WayPointType.AI_CAR;
                attackingInfo[identifier] = displayName;
                attackedInfo[displayName]++;
                return new WayPoint(aiCar.transform, WayPointType.AI_CAR);
            }
        }
        return null;

    }

    public WayPoint GetEnergyLocation(string identifier)
    {
        return GetRandomItemLocation(identifier, Item.ItemType.ENERGY_BOOST, WayPointType.ENERGY);
    }

    private WayPoint GetRandomItemLocation(string identifier, Item.ItemType itemType, WayPointType wayPointType)
    {
        CheckAndDecrementAttackingCount(identifier);
        List<Transform> transforms = ItemManager.INSTANCE.GetItemLocation(Item.ItemType.ENERGY_BOOST);
        if (transforms == null || transforms.Count < 1)
        {
            return null;
        }
        wayPointTracker[identifier] = wayPointType;
        return new WayPoint(transforms[UnityEngine.Random.Range(0, transforms.Count)], WayPointType.ENERGY);
    }

    public WayPoint GetHealthLocation(string identifier)
    {
        return GetRandomItemLocation(identifier, Item.ItemType.HEALTH_BOOST, WayPointType.HEALTH);
    }

    public WayPoint GetNitroLocation(string identifier)
    {
        return GetRandomItemLocation(identifier, Item.ItemType.SPEED_BOOST, WayPointType.NITRO);
    }

    public bool IsInvisibleBoundary(Collider collider)
    {
        return collider.CompareTag(GameManager.Tag.INVISIBLE_BOUNDARY);
    }

    public bool IsObstacle(Collider collider)
    {
        return collider.CompareTag(GameManager.Tag.OBSTACLE) || collider.GetComponent<Lava>() != null ||
        collider.GetComponent<Barel>() != null || collider.GetComponent<Oil>() != null ||
        collider.CompareTag(GameManager.Tag.ROCK);
    }

    public bool IsShootable(Collider collider)
    {
        return collider.GetComponentInParent<PlayerCar>() != null || collider.GetComponentInParent<AICar>() != null;
    }
}

using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    [System.Serializable]
    public enum ItemType
    {
        HEALTH_BOOST, ENERGY_BOOST, SPEED_BOOST
    }

    [System.Serializable]
    public class Item
    {
        public string name;
        public ItemType type;
        public GameObject prefab;
    }

    //Public fields
    public static ItemManager INSTANCE;
    public List<Item> items = new List<Item>();
    public int disappearTimeInSecs = 10;
    public int generationWindowInSecs = 20;

    //Private fields
    private Dictionary<GameObject, GameObject> itemPosToItemDict = new Dictionary<GameObject, GameObject>();

    //Unity methods
    void Awake()
    {
        Init();
    }

    void Start()
    {
        InvokeRepeating("GenerateItems", generationWindowInSecs, generationWindowInSecs);
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

    void GenerateItems()
    {
        if (!GameManager.INSTANCE.inGame)
        {
            return;
        }
        foreach (KeyValuePair<GameObject, GameObject> keyValuePair in itemPosToItemDict)
        {
            if (!keyValuePair.Value)
            {
                continue;
            }
            itemPosToItemDict[keyValuePair.Key] = GenerateItemForItemPoint(keyValuePair.Key);
        }
    }

    private GameObject GenerateItemForItemPoint(GameObject itemPoint)
    {
        int randIndex = Random.Range(0, items.Count);
        GameObject itemInstance = Instantiate(items[randIndex].prefab,
            itemPoint.transform.position, Quaternion.identity, itemPoint.transform);
        Destroy(itemInstance, disappearTimeInSecs);
        return itemInstance;

    }

    public void LoadItems(GameObject[] itemPoints)
    {
        foreach (KeyValuePair<GameObject, GameObject> keyValuePair in itemPosToItemDict)
        {
            if (!keyValuePair.Value)
            {
                continue;
            }
            Destroy(keyValuePair.Value);
        }
        itemPosToItemDict = new Dictionary<GameObject, GameObject>();
        foreach (GameObject itemPoint in itemPoints)
        {
            itemPosToItemDict.Add(itemPoint, GenerateItemForItemPoint(itemPoint));
        }
    }
}

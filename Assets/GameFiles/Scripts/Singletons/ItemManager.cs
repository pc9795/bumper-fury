using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    //Public fields
    public static ItemManager INSTANCE;
    public List<GameObject> items = new List<GameObject>(); //It should have Item component
    public int generationWindowInSecs = 30;

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

    public List<Transform> GetItemLocation(Item.ItemType type)
    {
        List<Transform> itemTransforms = new List<Transform>();
        //Can't use iteration as items are collected and destroyed by players.
        List<GameObject> itemPoints = new List<GameObject>(itemPosToItemDict.Keys);
        foreach (GameObject itemPoint in itemPoints)
        {
            if (!itemPosToItemDict[itemPoint])
            {
                continue;
            }
            Item item = itemPosToItemDict[itemPoint].GetComponent<Item>();
            if (item.type == type)
            {
                itemTransforms.Add(item.transform);
            }
        }
        return itemTransforms;
    }


    void GenerateItems()
    {
        if (!GameManager.INSTANCE.inGame)
        {
            return;
        }
        //Can't use iteration as items are collected and destroyed by players.
        List<GameObject> itemPoints = new List<GameObject>(itemPosToItemDict.Keys);
        foreach (GameObject itemPoint in itemPoints)
        {
            if (itemPosToItemDict[itemPoint])
            {
                continue;
            }
            itemPosToItemDict[itemPoint] = GenerateItemForItemPoint(itemPoint);
        }
    }

    private GameObject GenerateItemForItemPoint(GameObject itemPoint)
    {
        int randIndex = Random.Range(0, items.Count);
        GameObject itemInstance = Instantiate(items[randIndex], itemPoint.transform.position, Quaternion.identity);
        Item item = itemInstance.GetComponent<Item>();
        Vector3 position = new Vector3(itemPoint.transform.position.x,
            itemPoint.transform.position.y + item.groundLevel, itemPoint.transform.position.z);
        itemInstance.transform.position = position;
        return itemInstance;

    }

    public void Refresh()
    {
        GameObject[] itemPoints = GameManager.INSTANCE.itemPoints;
        //Destroying previous items.
        //Right now it is working fine but because this dictionary is modifed(players collect itmes) to have to check it in future.
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

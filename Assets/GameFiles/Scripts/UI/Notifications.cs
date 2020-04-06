using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Notifications : MonoBehaviour
{
    //Public fields
    //It should have a text component
    public GameObject listItem;
    public GameObject parentPanel;
    public int capcity = 7;
    public int expireTimeInSecs = 10;

    //Private fields
    private Queue<GameManager.Notification> notifications = new Queue<GameManager.Notification>();
    private List<GameObject> listItems = new List<GameObject>();

    //Unity methods
    void LateUpdate()
    {
        GameManager.Notification notification = GameManager.INSTANCE.PollNotification();
        if (notification != null)
        {
            //Keep latest items according to capacity.
            if (notifications.Count == capcity)
            {
                notifications.Dequeue();
            }
            notifications.Enqueue(notification);
        }
        ClearNotificationBoard();
        GenerateNotificationBoard();
    }

    private void ClearNotificationBoard()
    {
        foreach (GameObject item in listItems)
        {
            Destroy(item);
        }
        listItems = new List<GameObject>();
    }

    //Custom methods
    private void GenerateNotificationBoard()
    {
        float currTime = Time.time;
        int count = notifications.Count;

        for (int i = 0; i < count; i++)
        {
            GameManager.Notification notification = notifications.Dequeue();
            if (currTime - notification.time > expireTimeInSecs)
            {
                //this message is expired. Discard it.
                continue;
            }
            GameObject listItemInstance = Instantiate(listItem);
            Text text = listItemInstance.GetComponent<Text>();
            text.text = notification.message;
            listItemInstance.transform.SetParent(parentPanel.transform);
            listItems.Add(listItemInstance);
            notifications.Enqueue(notification);
        }
    }
}

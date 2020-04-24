using UnityEngine;

public class FireLevel : MonoBehaviour
{
    //Unity methods
    void Start()
    {
        //Game starts at paused state because of level start screen.
        Time.timeScale = 0f;
        GameManager.INSTANCE.InitLevel();
    }
}

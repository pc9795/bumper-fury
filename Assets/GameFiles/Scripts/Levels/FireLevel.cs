using UnityEngine;

public class FireLevel : MonoBehaviour
{
    //Unity methods
    void Start()
    {
        GameManager.INSTANCE.InitLevel();
    }
}

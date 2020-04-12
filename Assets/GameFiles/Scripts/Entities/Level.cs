using UnityEngine;

public class Level : MonoBehaviour
{
    //Unity methods
    void Start()
    {
        GameManager.INSTANCE.InitLevel();
    }
}

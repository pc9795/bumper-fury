using UnityEngine;

public class AirLevel : MonoBehaviour
{
    //Unity methods
    void Start()
    {
        GameManager.INSTANCE.InitLevel();
    }
}

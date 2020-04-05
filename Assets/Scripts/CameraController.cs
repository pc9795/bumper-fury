using UnityEngine;

public class CameraController : MonoBehaviour
{
    //Public fields
    public GameObject toFollow;

    //Unity Methods
    void LateUpdate()
    {
        transform.LookAt(toFollow.transform);
    }
}

using UnityEngine;

public class CameraController : MonoBehaviour
{
    //Public fields
    public GameObject toFollow;
    
    //Private fields
    private Vector3 offset;

    //Unity Methods
    void Start()
    {
        offset = transform.position - toFollow.transform.position;

    }

    void LateUpdate()
    {
        transform.position = toFollow.transform.position + offset;
    }
}

using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject toFollow;
    private Vector3 offset;
    void Start()
    {
        offset = transform.position - toFollow.transform.position;

    }

    void LateUpdate()
    {
        transform.position = toFollow.transform.position + offset;
    }
}

using UnityEngine;

public class MiniMapController : MonoBehaviour
{
    //Public fields
    public Transform toFollow;

    //Unity methods
    void LateUpdate()
    {
        Vector3 newPosition = toFollow.position;
        //Keep the zoom level inact.
        newPosition.y = transform.position.y;
        transform.position = newPosition;
    }
}

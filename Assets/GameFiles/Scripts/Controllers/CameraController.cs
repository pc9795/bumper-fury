using UnityEngine;

public class CameraController : MonoBehaviour
{
    //Public fields
    //Object to follow
    //Ideally it should be of type `Transform`. May be change in future.
    public GameObject toFollow;

    //Unity Methods
    void LateUpdate()
    {
        //Look at the following object.
        transform.LookAt(toFollow.transform);
    }
}

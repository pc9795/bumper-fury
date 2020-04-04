using UnityEngine;

public class SimpleMovementController : MonoBehaviour
{
    public float speed;
    void Start()
    {

    }

    void Update()
    {
        transform.Translate(new Vector3(0, 0, Input.GetAxis("Vertical") * speed));
    }
}

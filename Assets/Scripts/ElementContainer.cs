using UnityEngine;

public class ElementContainer : MonoBehaviour
{
    public GameObject element;
    private GameObject elementInstance;
    private Rigidbody parentRigidBody;
    private float groundLevel = 0.6f;

    void Start()
    {
        // Its parent should be a rigid body.
        parentRigidBody = transform.parent.GetComponent<Rigidbody>();
        Vector3 position = parentRigidBody.transform.position;
        position.y = groundLevel;
        elementInstance = Instantiate(element, position, Quaternion.identity);
    }

    void Update()
    {
        elementInstance.transform.position = transform.position;
    }
}

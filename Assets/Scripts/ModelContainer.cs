using UnityEngine;

public class ModelContainer : MonoBehaviour
{
    public GameObject model;
    private GameObject modelInstance;
    private Rigidbody parentRigidBody;
    private float groundLevel = 0.6f;
    void Start()
    {
        // Its parent should be a rigid body.
        parentRigidBody = transform.parent.GetComponent<Rigidbody>();
        Vector3 position = parentRigidBody.transform.position;
        position.y = groundLevel;
        modelInstance = Instantiate(model, position, Quaternion.identity);
    }

    void Update()
    {
        modelInstance.transform.position = transform.position;
    }
}

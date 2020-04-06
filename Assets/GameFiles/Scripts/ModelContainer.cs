using UnityEngine;

public class ModelContainer : MonoBehaviour
{
    //Public fields
    public GameObject model;
    
    //Private fields
    private GameObject modelInstance;
    private Rigidbody rigidBody;
    private float groundLevel = 0.6f; //todo check for public
    
    //Unity methods
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        Vector3 position = rigidBody.transform.position;
        position.y = groundLevel;
        modelInstance = Instantiate(model, position, Quaternion.identity);
    }

    void Update()
    {
        modelInstance.transform.position = transform.position;
    }
}

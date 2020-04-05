using UnityEngine;

public class ElementContainer : MonoBehaviour
{
    //Public fields
    public GameObject element;
    
    //Private fields
    private GameObject elementInstance;
    private Rigidbody rigidBody;
    private float groundLevel = 0.6f;

    //Unity methods
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        Vector3 position = rigidBody.transform.position;
        position.y = groundLevel;
        elementInstance = Instantiate(element, position, Quaternion.identity);
    }

    void Update()
    {
        elementInstance.transform.position = transform.position;
    }
}

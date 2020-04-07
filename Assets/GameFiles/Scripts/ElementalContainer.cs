using UnityEngine;

public class ElementalContainer : MonoBehaviour
{
    //Public fields
    public GameObject element;
    //TODO if the parent has a offset then need of this
    public float groundLevel = 0.6f;

    //Private fields
    private GameObject elementInstance;
    private Rigidbody rigidBody;

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

    void OnDestroy()
    {
        if (elementInstance)
        {
            Destroy(elementInstance);
        }
    }
}

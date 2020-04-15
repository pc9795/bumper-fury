using UnityEngine;

public class ElementalContainer : MonoBehaviour
{
    //Public fields
    public GameObject element;
    public float groundLevel = 0.6f;

    //Private fields
    private GameObject elementInstance;
    private Rigidbody rigidBody;

    //Unity methods
    void OnDestroy()
    {
        if (elementInstance)
        {
            Destroy(elementInstance);
        }
    }

    //Custom methods

    //Lazy init because the actual elemental is set dynamically.
    public void Init()
    {
        rigidBody = GetComponent<Rigidbody>();
        Vector3 position = rigidBody.transform.position;
        position.y += groundLevel;
        elementInstance = Instantiate(element, position, Quaternion.identity, transform);
    }
}

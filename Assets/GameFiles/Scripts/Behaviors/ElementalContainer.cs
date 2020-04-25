using UnityEngine;

public class ElementalContainer : MonoBehaviour
{
    //Public fields
    //Expected to be an particle system
    //Look for more narrow class
    //Particle system which will play whole time on the car 
    public GameObject element;
    //Offset in the y-axis.
    public float groundLevel = 0.6f;

    //Private fields
    //Instance of the `element` particle system
    private GameObject elementInstance;
    //Rigid body to which this script is attached
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

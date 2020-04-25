using UnityEngine;

public class NitroBehavior : MonoBehaviour
{
    //Public fields
    //Expected to be an particle system
    //Look for narrow objec type
    //Particle system which will be played during nitros.
    public GameObject nitroFlame;
    //Duration of the instance of the particle system
    public int flameDuration;

    //Private fields
    //Rigid body to which this script is attached to
    private Rigidbody rigidBody;
    //At which increment the nitros should be turned on.
    //Can it be moved to `GameManager`?
    private int incrmentSize = 15;
    //Instance of the `nitroFlame` particle system.
    private GameObject nitroFlameInstance;
    //Is initialized. As this script should work only when a rigid body is attached to it.This field should be indicating that 
    //a rigid body is attached or not.
    private bool initialized;

    //Unity methods
    void Update()
    {
        if (!initialized)
        {
            return;
        }
        //Get the integer part. It will be easy then.
        int speed = (int)rigidBody.velocity.magnitude;

        //Create flames at a particular increment
        if (speed != 0 && speed % incrmentSize == 0 && !nitroFlameInstance)
        {
            nitroFlameInstance = Instantiate(nitroFlame, transform.position, Quaternion.identity, transform);
            Destroy(nitroFlameInstance, flameDuration);
        }
        //Turn toward the parent object
        if (nitroFlameInstance)
        {
            nitroFlameInstance.transform.forward = transform.forward;
        }
    }

    //Custom methods

    //Lazy init as the parent rigid body will be attached dynamically.
    public void Init()
    {
        rigidBody = GetComponentInParent<Rigidbody>();
        initialized = true;
    }

}

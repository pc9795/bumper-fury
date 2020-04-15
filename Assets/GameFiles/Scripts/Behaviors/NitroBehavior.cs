using UnityEngine;

public class NitroBehavior : MonoBehaviour
{
    //Public fields
    public GameObject nitroFlame;
    public int flameDuration;

    //Private fields
    private Rigidbody rigidBody;
    private int incrmentSize = 10;
    private GameObject nitroFlameInstance;
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

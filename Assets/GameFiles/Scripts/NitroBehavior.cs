using UnityEngine;

public class NitroBehavior : MonoBehaviour
{
    //Public fields
    public GameObject nitroFlame;
    public int flameDuration;

    //Private fields
    private Rigidbody rigidBody;
    private int incrmentSize;
    private GameObject nitroFlameInstance;

    //Unity methods
    void Start()
    {
        rigidBody = GetComponentInParent<Rigidbody>();
        incrmentSize = 5;
    }

    void Update()
    {
        //Get the integer part. It will be easy then.
        int speed = (int)rigidBody.velocity.magnitude;
        
        //Create flames at a particular increment
        if (speed != 0 && speed % incrmentSize == 0 && !nitroFlameInstance)
        {
            nitroFlameInstance = Instantiate(nitroFlame, transform.position, Quaternion.identity);
            Destroy(nitroFlameInstance, flameDuration);
        }
        
        //Move accordign to local space.
        if (nitroFlameInstance)
        {
            nitroFlameInstance.transform.position = transform.position;
            nitroFlameInstance.transform.forward = transform.forward;
        }
    }
}

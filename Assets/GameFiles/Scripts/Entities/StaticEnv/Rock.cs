using UnityEngine;

public class Rock : MonoBehaviour
{
    //Public fields

    //Increase the mass of the rock by this amount.
    public float massMultiplier = 10;

    //Private fields

    //Rigid body attached to this rock
    new private Rigidbody rigidbody;
    //We will increase the mass of the rock once it is on the ground.
    private bool massUpdated;

    //Unity methods
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag(GameManager.Tag.LEVEL_FLOOR) && !massUpdated)
        {
            rigidbody.mass = rigidbody.mass * massMultiplier;
            massUpdated = true;
        }
    }
}

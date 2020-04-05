using UnityEngine;

public class FlameTornadoAttack : MonoBehaviour
{
    public GameObject flameTornado;
    private GameObject flameTornadoInstance;
    private Rigidbody parentRigidBody;
    public float speed;
    //Depends on the animation. Ponder on externalizing.
    private float duration = 8;
    private Vector3 direction;
    private float groundLevel = 0.6f;

    void Start()
    {
        parentRigidBody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !flameTornadoInstance)
        {
            //Instantiate a new flame tornado
            Vector3 position = parentRigidBody.transform.position;
            position.y = groundLevel;
            flameTornadoInstance = Instantiate(flameTornado, position, Quaternion.identity);
            Destroy(flameTornadoInstance, duration);
            direction = parentRigidBody.transform.forward;
        }
        if (flameTornadoInstance)
        {
            flameTornadoInstance.transform.position += direction * speed * Time.deltaTime;
        }
    }
}

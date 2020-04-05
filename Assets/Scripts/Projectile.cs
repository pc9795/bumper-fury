using UnityEngine;

public class Projectile : MonoBehaviour
{
    //Public ones
    public GameObject projectileType;
    public float speed;
    public float radius;
    public float power;
    //Private ones
    private GameObject projectileInstance;
    private Rigidbody parentRigidBody;
    //Depends on the animation. Ponder on externalizing.
    private float duration = 8;
    private Vector3 direction;
    private float groundLevel = 0.6f;

    void Start()
    {
        // Its parent should be a rigid body.
        parentRigidBody = transform.parent.GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !projectileInstance)
        {
            //Instantiate a new flame tornado
            Vector3 position = parentRigidBody.transform.position;
            position.y = groundLevel;
            projectileInstance = Instantiate(projectileType, position, Quaternion.identity);
            Destroy(projectileInstance, duration);
            direction = parentRigidBody.transform.forward;
        }
        if (projectileInstance)
        {
            Collider[] colliders = Physics.OverlapSphere(projectileInstance.transform.position, radius);
            foreach (Collider collider in colliders)
            {
                AICar aICar = collider.transform.parent.GetComponent<AICar>();
                if (aICar != null && aICar.rigidbody != null)
                {
                    //todo check the upward modifier settings.
                    aICar.rigidbody.AddExplosionForce(power, projectileInstance.transform.position, radius, 3.0f);
                }
            }
            projectileInstance.transform.position += direction * speed * Time.deltaTime;
        }
    }
}

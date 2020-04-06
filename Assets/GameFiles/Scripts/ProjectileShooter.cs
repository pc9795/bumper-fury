using UnityEngine;

public class ProjectileShooter : MonoBehaviour
{
    //Public fields
    public GameObject projectileType;
    public float speed;
    public float radius;
    public float power;

    //Private fields
    private GameObject projectileInstance;
    private Rigidbody rigidBody;
    private float duration = 8; //todo check for public
    private Vector3 direction;
    private float groundLevel = 0.6f; //todo check for public

    //Unity methods
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (!projectileInstance)
        {
            return;
        }
        Collider[] colliders = Physics.OverlapSphere(projectileInstance.transform.position, radius);
        foreach (Collider collider in colliders)
        {
            AICarController aICar = collider.GetComponentInParent<AICarController>();
            if (aICar != null && aICar.rigidbody != null)
            {
                //todo check the upward modifier settings.
                //todo give them energy they are hit.
                aICar.rigidbody.AddExplosionForce(power, projectileInstance.transform.position, radius, 3.0f);
            }
        }
        projectileInstance.transform.position += direction * speed * Time.deltaTime;
    }

    // Custom methods
    public bool CanShoot()
    {
        return projectileInstance == null;
    }

    public void Shoot()
    {
        if (!CanShoot())
        {
            return;
        }
        //Instantiate a new flame tornado
        Vector3 position = rigidBody.transform.position;
        position.y = groundLevel;
        projectileInstance = Instantiate(projectileType, position, Quaternion.identity);
        Destroy(projectileInstance, duration);
        direction = rigidBody.transform.forward;

    }
}

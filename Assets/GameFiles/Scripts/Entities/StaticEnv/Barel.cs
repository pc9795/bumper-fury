using UnityEngine;

public class Barel : MonoBehaviour
{
    //Public variables
    public GameObject explostionAnim;
    public int duration;
    public int radius = 20;
    public float upwardRift = 3.0f;
    public float power = 20000;
    public int baseDamage = 20;

    //Unity methods
    public void Explode()
    {
        GameObject explostionInstance = Instantiate(explostionAnim, transform.position, Quaternion.identity);
        Destroy(explostionInstance, duration);
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
        foreach (Collider collider in colliders)
        {
            //Collision will occur with models so have to look for behaviors in parent.
            AICar aICar = collider.GetComponentInParent<AICar>();
            PlayerCar playerCar = collider.GetComponentInParent<PlayerCar>();
            Rigidbody rigidbody = null;
            StatsController stats = null;
            if (aICar != null)
            {
                rigidbody = aICar.GetComponent<Rigidbody>();
                stats = aICar.GetComponent<StatsController>();
            }
            else if (playerCar != null)
            {
                rigidbody = playerCar.GetComponent<Rigidbody>();
                stats = playerCar.GetComponent<StatsController>();
            }
            else
            {
                continue;
            }
            rigidbody.AddExplosionForce(power, transform.position, radius, upwardRift);
            stats.DamageHealth(baseDamage);
        }
        Destroy(this.gameObject);
    }
}

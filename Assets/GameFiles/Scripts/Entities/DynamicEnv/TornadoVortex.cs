using UnityEngine;

public class TornadoVortex : MonoBehaviour
{
    //Public variables
    public float pullingForce;

    //Unity methods
    void OnTriggerStay(Collider collider)
    {
        AICar aICar = collider.GetComponentInParent<AICar>();
        if (aICar != null)
        {
            aICar.transform.position = Vector3.MoveTowards(aICar.transform.position, transform.position, pullingForce);
        }
        PlayerCar playerCar = collider.GetComponentInParent<PlayerCar>();
        if (playerCar != null)
        {
            playerCar.transform.position = Vector3.MoveTowards(playerCar.transform.position, transform.position, pullingForce);
        }
    }
}

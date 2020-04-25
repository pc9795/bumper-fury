using UnityEngine;

public class TornadoVortex : MonoBehaviour
{
    //Public variables
    
    //A parameter which describes the pull applied to the object inside the vortex.
    public float pullingForce;

    //Unity methods
    void OnTriggerStay(Collider collider)
    {
        //Collider will be on child models so look on the parent.
        AICar aICar = collider.GetComponentInParent<AICar>();
        if (aICar != null)
        {
            aICar.transform.position = Vector3.MoveTowards(aICar.transform.position, transform.position, pullingForce);
        }
        //Collider will be on child models so look on the parent.
        PlayerCar playerCar = collider.GetComponentInParent<PlayerCar>();
        if (playerCar != null)
        {
            playerCar.transform.position = Vector3.MoveTowards(playerCar.transform.position, transform.position, pullingForce);
        }
    }
}

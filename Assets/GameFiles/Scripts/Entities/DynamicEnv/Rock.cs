using UnityEngine;

public class Rock : MonoBehaviour
{
    //Public variables
    public float baseDamage = 10;
    [HideInInspector]
    public float damageMultiplier = 1f;

    //Custom methods
    public float GetDamage()
    {
        return baseDamage * damageMultiplier;
    }

}

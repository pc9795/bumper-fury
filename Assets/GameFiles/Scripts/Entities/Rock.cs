using UnityEngine;

public class Rock : MonoBehaviour
{
    public float baseDamage = 10;
    [HideInInspector]
    public float damageMultiplier = 1f;

    public float GetDamage()
    {
        return baseDamage * damageMultiplier;
    }

}

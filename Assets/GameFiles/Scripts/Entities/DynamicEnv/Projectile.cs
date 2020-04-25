using UnityEngine;

public class Projectile : MonoBehaviour
{
    //Public variables
    //Speed of the projectile
    public float speed = 0.2f;
    //Radius of the projectile
    public float radius = 5;
    //Force of the projectile
    public float power = 15000;
    //Duration of the projectile.
    public float duration = 8;
    //Upward rift for the projectile
    public float upwardRift = 10;
    //Y-axis offset.
    public float groundLevel = 0.6f;
    //Damage done by projectile to a contenstant.
    public float baseDamage = 0.1f;
}

using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10;
    public float radius = 15;
    public float power = 30000;
    public float duration = 8;
    public float upwardRift = 3.0f;
    //TODO if the parent has a offset then need of this
    public float groundLevel = 0.6f;
    public int baseDamage = 1;
}

using UnityEngine;

public class Tornado : MonoBehaviour
{
    //Public variables
    public int speed = 10;

    //Private variables
    private int deathTimer = 5;

    //Unity methods.
    void Update()
    {
        Bounds levelBounds = GameManager.INSTANCE.levelBounds;
        if (!levelBounds.Contains(transform.position))
        {
            Destroy(gameObject, deathTimer);
        }
        //TODO I am not making everything FPS consistent so have to remove this deltaTime here to ensure consistency.
        transform.position += transform.forward * speed * Time.deltaTime;
    }
}

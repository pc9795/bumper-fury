using UnityEngine;

public class Storm : MonoBehaviour
{
    //Public variables
    public int speed = 10;

    //Private variables
    private int deathTimer = 5;

    void Start()
    {
        AudioManager.INSTANCE.Play(AudioManager.AudioTrack.WIND);
    }

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

    void OnDestroy()
    {
        AudioManager.INSTANCE.Stop(AudioManager.AudioTrack.WIND);
    }

}

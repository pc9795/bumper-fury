using UnityEngine;

public class Storm : MonoBehaviour
{
    //Public variables
    public float speed = 0.2f;

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
        transform.position += transform.forward * speed;
    }

    void OnDestroy()
    {
        AudioManager.INSTANCE.Stop(AudioManager.AudioTrack.WIND);
    }

}

using UnityEngine;

public class Storm : MonoBehaviour
{
    //Public variables
    
    //Speed of the storm
    public float speed = 0.2f;
    //Force of the storm.
    public float force = 7500f;
    
    //Private variables
    //Duration of the storm
    private int deathTimer = 5;

    void Start()
    {
        AudioManager.INSTANCE.Play(AudioManager.AudioTrack.WIND);
    }

    //Unity methods.
    void Update()
    {
        //Check if out of bounds.
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

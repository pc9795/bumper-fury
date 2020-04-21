using UnityEngine;

public class Tornado : MonoBehaviour
{
    //Public variables
    public int duration = 5;
    public float baseDamage = 0.1f;

    //Unity methods.
    void Start()
    {
        AudioManager.INSTANCE.Play(AudioManager.AudioTrack.HURRICANE);
    }

    void Update()
    {
        //TODO readjust the prefab to remove this kind of thing.
        //Tornado is the grand-child of main particle system.
        Destroy(transform.parent.transform.parent.gameObject, duration);
    }

    void OnDestroy()
    {
        AudioManager.INSTANCE.Stop(AudioManager.AudioTrack.HURRICANE);
    }
}

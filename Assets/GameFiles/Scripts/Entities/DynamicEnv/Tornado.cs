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
        //In future adjust the prefab so that we don't have to write this messy piece of code.
        //Tornado is the grand-child of main particle system.
        Destroy(transform.parent.transform.parent.gameObject, duration);
    }

    void OnDestroy()
    {
        AudioManager.INSTANCE.Stop(AudioManager.AudioTrack.HURRICANE);
    }
}

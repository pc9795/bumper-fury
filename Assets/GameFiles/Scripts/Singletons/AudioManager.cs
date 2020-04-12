using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [System.Serializable]
    public class Sound
    {
        public string name;
        public AudioClip clip;
        [Range(0f, 1f)]
        public float volume;
        [Range(.1f, 3f)]
        public float pitch;
        public bool loop;
        [HideInInspector]
        public AudioSource source;
    }

    public class AudioTrack
    {
        public static string THEME = "Theme";
        public static string BUTTON_CLICK = "Button Click";
        public static string EDM1 = "EDM 1";
        public static string EDM2 = "EDM 2";
        public static string EDM3 = "EDM 3";
        public static string CHARGED_UP = "Charged Up";
        public static string ENGINE = "Engine";
        public static string ITEM_COLLECT = "Item collect";
        public static string POWER_USE = "Power use";
    }

    //Public fields
    public static AudioManager INSTANCE;
    public List<Sound> sounds = new List<Sound>();

    private Dictionary<string, Sound> soundsDict = new Dictionary<string, Sound>();

    //Unity methods
    void Awake()
    {
        Init();
        foreach (Sound sound in sounds)
        {
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;
            sound.source.volume = sound.volume;
            sound.source.pitch = sound.pitch;
            sound.source.loop = sound.loop;
            soundsDict[sound.name] = sound;
        }
    }

    //Custom methods
    private void Init()
    {
        if (INSTANCE != null)
        {
            Destroy(gameObject);
        }
        else
        {
            INSTANCE = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void Play(string name)
    {
        Sound sound = soundsDict[name];
        if (sound == null)
        {
            print("Requested sound:" + name + " not found!");
            return;
        }
        sound.source.Play();
    }


    public void Stop(string name)
    {
        Sound sound = soundsDict[name];
        if (sound == null)
        {
            print("Requested sound:" + name + " not found!");
            return;
        }
        sound.source.Stop();
    }

    public Sound GetSound(string name)
    {
        return soundsDict[name];
    }

}

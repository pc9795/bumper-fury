﻿using System.Collections.Generic;
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
    //Public fields
    public static AudioManager INSTANCE;
    public List<Sound> sounds = new List<Sound>();

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
        }
        Play("Theme");
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
        Sound sound = null;
        foreach (Sound _sound in sounds)
        {
            if (_sound.name == name)
            {
                sound = _sound;
                break;
            }
        }
        if (sound == null)
        {
            print("Requested sound:" + name + " not found!");
            return;
        }
        sound.source.Play();
    }

}
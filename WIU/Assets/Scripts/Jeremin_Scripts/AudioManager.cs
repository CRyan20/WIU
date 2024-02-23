using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public Sound[] musicSound, effectSound;
    public AudioSource musicSource, effectSource;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayMusic(string name)
    {
        Sound s = Array.Find(musicSound, x => x.name == name);

        if (s == null)
        {
            Debug.Log("Sound Not found. Check if string matches audio file name");
        }
        else
        {
            musicSource.clip = s.clip;
            musicSource.Play();
        }
    }

    public void PlaySound(string name)
    {
        Sound s = Array.Find(effectSound, x => x.name == name);

        if (s == null)
        {
            Debug.Log("Sound Not found. Check if string matches audio file name");
        }
        else
        {
            effectSource.PlayOneShot(s.clip);
        }
    }
    public void StopSound(string name)
    {
        Sound s = Array.Find(effectSound, x => x.name == name);

        if (s == null)
        {
            Debug.Log("Sound Not found. Check if string matches audio file name");
        }
        else
        {
            effectSource.Stop();
        }
    }
    public bool IsPLayingSFX(string name)
    {
        Sound s = Array.Find(effectSound, x => x.name == name);

        if (s == null)
        {
            Debug.Log("Sound Not found. Check if string matches audio file name");
            return false;
        }
        else
        {
            return effectSource.isPlaying;


        }
    }
}


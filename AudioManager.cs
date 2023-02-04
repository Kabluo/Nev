using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        Destroy(gameObject);
    }

    public List<AudioSource> music = new List<AudioSource>();
    public List<AudioSource> soundEffects = new List<AudioSource>();

    public void PlayMusic(int index)
    {
        if(music[index].isPlaying) { return; } //if the same music is called, stop function and keep music playing

        for(int i = 0; i < music.Count; i++) //stop all music, then start the new one
        music[i].Stop();

        music[index].Play();
    }

    public void PlaySFX(int index)
    {
        soundEffects[index].Stop();
        soundEffects[index].Play();
    }

    public void PlaySFXPitchRandomized(int index) //adds pitch variant to sfx, makes repeated sounds less annoying
    {
        soundEffects[index].pitch = Random.Range(0.8f, 1.2f);
        PlaySFX(index);
    }
}

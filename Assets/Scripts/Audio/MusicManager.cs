using System.Collections;
using System.Collections.Generic;
using StaticClassSettingsGame;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] float baseVolume = 0.2f;
    [SerializeField] AudioSource source;
    [SerializeField] AudioClip introductionMusic;
    [SerializeField] AudioClip loopMusic;
    [SerializeField] AudioClip introductionPursuit;
    [SerializeField] AudioClip loopPursuit;
    [SerializeField] AudioClip introductionMenu;
    [SerializeField] AudioClip loopMenu;
    [SerializeField] AudioClip win;
    [SerializeField] AudioClip loose;

    int guardsInPursuit = 0;
    float timeIntroduction = 0.0f;

    void Start()
    {
        Invoke("switchToLoopMenu", introductionMenu.length);
        source.clip = introductionMenu;
        ChangeVolume();
        source.Play();
        source.loop = false;
    }

    void switchToLoopMenu()
    {
        source.clip = loopMenu;
        ChangeVolume();
        source.Play();
        source.loop = true;
    }

    public void SwitchToGameMusic()
    {
        source.loop = true;
        CancelInvoke();
        source.clip = introductionMusic;
        ChangeVolume();
        source.Play();
        Invoke("switchToLoop", introductionMusic.length);
    }

    public void ChangeVolume()
    {
        float volume = source.clip == loopMusic || source.clip == introductionMusic ? baseVolume : baseVolume * 3.0f;

        source.volume = volume * (PlayerSettings.masterMusicLevel * PlayerSettings.musicMusicLevel);
    }

    public void SwitchToPursuit()
    {
        guardsInPursuit++;

        if (guardsInPursuit == 1)
        {
            float currentTime = source.time;
            source.clip = source.clip == loopMusic || source.clip == loopPursuit ? loopPursuit : introductionPursuit;
            ChangeVolume();
            source.time = currentTime;
            source.Play();
        }
    }

    public void SwitchToLow()
    {
        guardsInPursuit = guardsInPursuit - 1 < 0 ? 0 : guardsInPursuit - 1;

        if (guardsInPursuit == 0)
        {
            float currentTime = source.time;
            source.clip = source.clip == loopMusic || source.clip == loopPursuit ? loopMusic : introductionMusic;
            ChangeVolume();
            source.time = currentTime;
            source.Play();
        }
    }

    void switchToLoop()
    {
        source.clip = source.clip == introductionMusic ? loopMusic : loopPursuit;
        source.Play();
    }

    public void Win()
    {
        source.clip = win;
        ChangeVolume();
        source.Play();
        source.loop = false;
    }

    public void Loose()
    {
        source.clip = loose;
        ChangeVolume();
        source.Play();
        source.loop = false;
    }
}

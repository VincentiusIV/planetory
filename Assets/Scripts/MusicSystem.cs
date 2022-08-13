using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MusicSystem : Singleton<MusicSystem>
{
    public AudioClip[] tracks;
    public AudioSource musicSource;

    private AudioClip lastTrack;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }

    private void LateUpdate()
    {
        if (!musicSource.isPlaying)
        {
            ChooseNextTrack();
        }
    }

    private void ChooseNextTrack()
    {
        Debug.Assert(tracks.Length > 0);
        int randIdx = Random.Range(0, tracks.Length);
        if(tracks.Length > 1 && tracks[randIdx] == lastTrack)
        {
            randIdx += 1;
            randIdx %= tracks.Length;
        }
        lastTrack = musicSource.clip;
        musicSource.clip = tracks[randIdx];
        musicSource.Play();
    }
}

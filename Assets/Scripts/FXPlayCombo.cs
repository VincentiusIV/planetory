using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FXPlayCombo : MonoBehaviour
{
    public UnityEvent OnPlay;

    public ParticleSystem ParticleSystem;
    public AudioSource Audio;


    public void Play()
    {
        OnPlay.Invoke();
        ParticleSystem?.Play();
        Audio?.Play();
    }
}

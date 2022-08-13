using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(ParticleSystem))]
[RequireComponent(typeof(AudioSource))]
public class FXPlayCombo : MonoBehaviour
{
    public UnityEvent OnPlay;

    public ParticleSystem ParticleSystem;
    public AudioSource Audio;
    public AudioClip[] clips;
    public bool playOnAwake;
    public bool playOneShot;

    private void Awake()
    {
        if (playOnAwake)
            Play();
    }

    public void Play()
    {
        OnPlay.Invoke();
        ParticleSystem?.Play();
        if(clips.Length > 0 && Audio != null)
        {
            int randIdx = Random.Range(0, clips.Length);
            Audio.clip = clips[randIdx];
        }

        if(playOneShot)
        {
            AudioSource.PlayClipAtPoint(Audio.clip, transform.position);
        }
        else
        {
            Audio?.Play();
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (Audio == null)
        {
            Audio = GetComponent<AudioSource>();
            UnityEditor.EditorUtility.SetDirty(this);
        }
        if(ParticleSystem == null)
        {
            ParticleSystem = GetComponent<ParticleSystem>();           
            UnityEditor.EditorUtility.SetDirty(this);
        }
    }
#endif
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Button))]
[RequireComponent(typeof(AudioSource))]
public class ButtonFX : MonoBehaviour, IPointerEnterHandler
{
    public AudioSource HoverAudio, ClickAudio;

    public AudioClip[] hoverClips;
    public AudioClip[] clickClips;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        PlayRandom(HoverAudio, hoverClips);
    }

    private void OnClick()
    {
        PlayRandom(ClickAudio, clickClips);
    }

    private void PlayRandom(AudioSource source, AudioClip[] clips)
    {

        if (source && clips.Length > 0)
        {
            int randIdx = Random.Range(0, clips.Length);
            source.clip = clips[randIdx];
        }

        source?.Play();

    }
}

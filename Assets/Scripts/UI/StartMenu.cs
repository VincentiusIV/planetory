using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartMenu : MonoBehaviour
{

    public Button playButton, levelsButton, quitButton;
    public GameObject levelSelectRoot;
    public LevelSelectButton[] levelSelectButtons;

    private void Awake()
    {
        levelSelectRoot.SetActive(false);
        playButton.onClick.AddListener(OnPlayClick);
        levelsButton.onClick.AddListener(OnLevelSelectClick);
        quitButton.onClick.AddListener(OnQuitClick);

        for (int i = 0; i < levelSelectButtons.Length; i++)
            levelSelectButtons[i].Setup(i);
    }

    private void OnQuitClick()
    {
        Application.Quit();
    }

    private void OnLevelSelectClick()
    {
        levelSelectRoot.SetActive(!levelSelectRoot.activeSelf);
    }

    private void OnPlayClick()
    {
        LevelManager.Instance.Play();
    }
}

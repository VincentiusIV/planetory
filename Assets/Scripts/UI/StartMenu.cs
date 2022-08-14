using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartMenu : MonoBehaviour
{
    public bool deletePlayerPrefs;
    public Button playButton, levelsButton, quitButton, backToStart;
    public GameObject mainButtonRoot, levelSelectRoot;
    public LevelSelectButton[] levelSelectButtons;
    public Texture2D cursor;

    private void Awake()
    {
#if UNITY_EDITOR
        if (deletePlayerPrefs)
            PlayerPrefs.DeleteAll();
#endif
        levelSelectRoot.SetActive(false);
        playButton.onClick.AddListener(OnPlayClick);
        levelsButton.onClick.AddListener(OnLevelSelectClick);
        quitButton.onClick.AddListener(OnQuitClick);
        backToStart.onClick.AddListener(OnBackToStartClick);

        for (int i = 0; i < levelSelectButtons.Length; i++)
            levelSelectButtons[i].Setup(i);

        Cursor.SetCursor(cursor, Vector2.zero, CursorMode.ForceSoftware);
        SwitchToMainButton();

    }

    private void OnQuitClick()
    {
        Application.Quit();
    }

    private void OnLevelSelectClick()
    {
        SwitchToLevelSelect();
    }

    private void OnPlayClick()
    {
        LevelManager.Play();
    }

    private void OnBackToStartClick()
    {
        SwitchToMainButton();
    }

    private void SwitchToLevelSelect()
    {
        levelSelectRoot.SetActive(true);
        mainButtonRoot.SetActive(false);
    }

    private void SwitchToMainButton()
    {
        levelSelectRoot.SetActive(false);
        mainButtonRoot.SetActive(true);
    }
}

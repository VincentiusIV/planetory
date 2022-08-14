using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class LevelManager : Singleton<LevelManager>
{
    public static string LAST_LEVEL_INDEX = "LastLevelIndex";
    public static string IS_LEVEL_COMPLETE = "IsLevelComplete{0}";
    public string menuName = "StartMenu";
    public string[] levelNames;
    private static int currentLevelIndex;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);

        string currentLevelName = SceneManager.GetActiveScene().name;
        for (int i = 0; i < levelNames.Length; i++)
        {
            if(levelNames[i] == currentLevelName)
            {
                currentLevelIndex = i;
                Debug.Log($"Current level index: {currentLevelIndex}");
                PlayerPrefs.SetInt(LAST_LEVEL_INDEX, currentLevelIndex);
                break;
            }
        }
    }

    public static void Play()
    {
        if (!PlayerPrefs.HasKey(LAST_LEVEL_INDEX))
            PlayerPrefs.SetInt(LAST_LEVEL_INDEX, 0);
        int lastIndex = PlayerPrefs.GetInt(LAST_LEVEL_INDEX);
        Play(lastIndex);
    }

    public static void PlayNext()
    {
        int nextIndex = currentLevelIndex + 1;
        if(nextIndex >= Instance.levelNames.Length)
        {
            nextIndex = 0;
            GoToMainMenu();
        }
        else
        {
            Play(nextIndex);
        }
        PlayerPrefs.SetInt(LAST_LEVEL_INDEX, nextIndex);
    }

    public static void Play(int index)
    {
        Debug.Assert(index >= 0 && index < Instance.levelNames.Length);
        index = Mathf.Clamp(index, 0, Instance.levelNames.Length);
        SceneManager.LoadScene(Instance.levelNames[index]);
        MusicSystem.Instance.ChooseNextTrack();
        PlayerPrefs.SetInt(LAST_LEVEL_INDEX, index);
    }

    public static void GoToMainMenu()
    {
        SceneManager.LoadScene(Instance.menuName);
        MusicSystem.Instance.ChooseNextTrack();
    }

    public static void MarkLevelComplete()
    {
        MarkLevelComplete(currentLevelIndex);
    }

    public static void MarkLevelComplete(int index)
    {
        string key = string.Format(IS_LEVEL_COMPLETE, index);
        PlayerPrefs.SetInt(key, 1);
    }

    public static bool IsLevelComplete(int index)
    {
        string key = string.Format(IS_LEVEL_COMPLETE, index);
        if (!PlayerPrefs.HasKey(key))
            return false;
        int levelComplete = PlayerPrefs.GetInt(key);
        return levelComplete == 1;
    }
}

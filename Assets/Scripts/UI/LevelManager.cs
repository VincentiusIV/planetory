using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : Singleton<LevelManager>
{
    public static string LAST_LEVEL_INDEX = "LastLevelIndex";
    public string menuName = "StartMenu";
    public string[] levelNames;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }

    public void Play()
    {
        if (!PlayerPrefs.HasKey(LAST_LEVEL_INDEX))
            PlayerPrefs.SetInt(LAST_LEVEL_INDEX, 0);
        int lastIndex = PlayerPrefs.GetInt(LAST_LEVEL_INDEX);
        LevelManager.Instance.Play(lastIndex);
    }

    public void Play(int index)
    {
        Debug.Assert(index >= 0 && index < levelNames.Length);
        index = Mathf.Clamp(index, 0, levelNames.Length);
        SceneManager.LoadScene(levelNames[index]);
        PlayerPrefs.SetInt(LAST_LEVEL_INDEX, index);
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene(menuName);
    }
}

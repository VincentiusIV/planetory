using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

[RequireComponent(typeof(Button))]
public class LevelSelectButton : MonoBehaviour
{
    public int levelIndex;
    public TMP_Text levelField;
    public Sprite levelCompletedTexture;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(OnClick);
        LevelManager.MarkLevelComplete(3);
    }

    public void Setup(int levelIndex)
    {
        this.levelIndex = levelIndex;
        levelField.text = levelIndex.ToString();
        if (LevelManager.IsLevelComplete(levelIndex))
        {
            Image ing = GetComponent<Image>();
            ing.sprite = levelCompletedTexture;
        }

    }

    private void OnClick()
    {
        LevelManager.Play(levelIndex);
    }
}

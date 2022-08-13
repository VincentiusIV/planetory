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

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    public void Setup(int levelIndex)
    {
        this.levelIndex = levelIndex;
        levelField.text = levelIndex.ToString();
    }

    private void OnClick()
    {
        LevelManager.Instance.Play(levelIndex);
    }
}

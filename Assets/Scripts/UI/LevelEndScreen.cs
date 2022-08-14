using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelEndScreen : MonoBehaviour
{
    public GameObject visualRoot;
    public Button backToMenuButton, replayButton, nextLevelButton;

    private OutputNode[] outputNodes;

    private void Start()
    {
        outputNodes = FindObjectsOfType<OutputNode>();
        Debug.Assert(outputNodes.Length > 0);
        foreach (var outputNode in outputNodes)
            outputNode.OnReceivedItem.AddListener(OnOutputNodeReceivedItem);

        backToMenuButton.onClick.AddListener(BackToMenu);
        nextLevelButton.onClick.AddListener(NextLevel);
        visualRoot.SetActive(false);
    }

    private void OnOutputNodeReceivedItem()
    {
        bool isSatisfied = true;
        foreach (var outputNode in outputNodes)
            isSatisfied &= outputNode.IsSatisfied;
        if (isSatisfied)
        {
            Debug.Log("Level complete!");
            OnLevelComplete();
            LevelManager.MarkLevelComplete();
        }
    }

    public void NextLevel()
    {
        LevelManager.PlayNext();
    }

    public void BackToMenu()
    {
        LevelManager.GoToMainMenu();
    }

    private void OnLevelComplete()
    {
        visualRoot.SetActive(true);
        Builder.Instance.Disable();
    }
}

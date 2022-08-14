using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class Hint
{
    public Item itemA, itemB, resultX;
}

public class HintSystem : Singleton<HintSystem>
{
    public GameObject visualRoot, hintRoot;
    public HintUI hintUI;
    private Hint[] hints;
    private int hintIdx;

    protected override void Awake()
    {
        base.Awake();
        visualRoot.SetActive(false);
    }

    public void SetHints(Hint[] hints)
    {
        this.hints = hints;
        visualRoot.SetActive(hints.Length > 0);
        hintRoot.SetActive(false);
    }

    public void NextHint()
    {
        if (!hintRoot.activeSelf)
        {
            hintRoot.SetActive(true);
        }
        else
        {
            ++hintIdx;
            hintIdx %= hints.Length;
        }

        hintUI.Setup(hints[hintIdx]);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class HintUI : MonoBehaviour
{
    public Image iconA, iconB, iconX;
    public TMP_Text nameA, nameB, nameX;

    internal void Setup(Hint hint)
    {
        iconA.sprite = hint.itemA.Icon;
        nameA.text = hint.itemA.name;
        iconB.sprite = hint.itemB.Icon;
        nameB.text = hint.itemB.name;
        iconX.sprite = hint.resultX.Icon;
        nameX.text = hint.resultX.name;
    }
}

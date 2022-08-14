using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class Notification : Singleton<Notification>
{
    public GameObject visualRoot;
    public TMP_Text titleField, bodyField, tipField;
    public Image iconField;
    public Button continueButton;

    protected override void Awake()
    {
        base.Awake();
        visualRoot.SetActive(false);
        continueButton.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        Hide();
    }

    public void Show(string title, string body, string tip, Sprite icon)
    {
        visualRoot.SetActive(true);
        Builder.Instance.Disable();

        titleField.text = title;
        bodyField.text = body;
        tipField.text = tip;
        iconField.sprite = icon;
    }

    private void Hide()
    {
        Builder.Instance.Enable();
        visualRoot.SetActive(false);
    }

}

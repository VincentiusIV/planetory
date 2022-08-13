using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using TMPro;

[RequireComponent(typeof(Button))]
public class BuildingButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public UnityEvent OnHover;
    public GameObject nameRoot;
    public TMP_Text nameField;
    public Image icon;
    private BuildResource resource;

    private void Awake()
    {
        Button button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
        nameRoot.SetActive(false);
    }

    public void Setup(BuildResource buildResource)
    {
        this.resource = buildResource;
        icon.sprite = buildResource.GetNodePrefab().Icon;
        nameField.text = resource.prefab.name;
    }

    private void OnClick()
    {
        Builder.Instance.StartBuilding(resource);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnHover.Invoke();
        nameRoot.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        nameRoot.SetActive(false);
    }
}

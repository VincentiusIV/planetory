using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class OutputNode : Node
{
    public int RemainingItemsCount { get; private set; }
    public bool IsSatisfied => RemainingItemsCount <= 0;
    public Item requiredItem;
    public int needed = 5;

    public TMP_Text itemNameField;
    public MeshRenderer itemIconRenderer;

    public RailNode inputRail;

    protected override void Awake()
    {
        base.Awake();
        inputRail.OnCartAwaitingTransfer.AddListener(OnCartAwaitingTransfer);

        itemNameField.text = requiredItem.name;
        itemIconRenderer.material.SetTexture("_BaseMap", requiredItem.Icon);
    }

    private void Start()
    {
        NodeGrid.Instance.Place(inputRail);
    }

    private void OnCartAwaitingTransfer()
    {
        RailCart cart = inputRail.Pop();
        if (cart.Content == requiredItem)
        {
            --RemainingItemsCount;
            cart.Kill();
        }
        else
        {
            cart.Explode();
        }
    }

    private void OnValidate()
    {
        if (requiredItem != null && itemNameField != null)
        {
            itemNameField.text = requiredItem.name;
        }
    }
}

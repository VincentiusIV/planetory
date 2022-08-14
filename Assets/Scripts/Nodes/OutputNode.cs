using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class OutputNode : Node
{
    public int RemainingItemsCount { get; private set; }
    public UnityEvent OnReceivedItem;
    public bool IsSatisfied => RemainingItemsCount <= 0;
    public Item requiredItem;
    public int needed = 1;

    public TMP_Text itemNameField, requiredField;
    public MeshRenderer itemIconRenderer;

    public RailNode inputRail;

    protected override void Awake()
    {
        base.Awake();
        inputRail.OnCartAwaitingTransfer.AddListener(OnCartAwaitingTransfer);
        Debug.Assert(requiredItem != null, this);
        itemNameField.text = requiredItem.name;
        itemIconRenderer.material.SetTexture("_BaseMap", requiredItem.Icon.texture);
        RemainingItemsCount = needed;
        requiredField.text = "Req: " + RemainingItemsCount;
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
            RemainingItemsCount = Math.Max(0, RemainingItemsCount);
            requiredField.text = "Req: " + RemainingItemsCount;
            OnReceivedItem.Invoke();
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

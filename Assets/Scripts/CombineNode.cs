using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 2 In, 1 Out
/// </summary>
public class CombineNode : Node
{
    public RailNode input1, input2, output;
    public RailCart cartPrefab;
    private bool IsWaitingForOutput;

    private void Start()
    {
        input1.OnCartAwaitingTransfer.AddListener(OnInputOfferedItem);
        input2.OnCartAwaitingTransfer.AddListener(OnInputOfferedItem);
    }

    private void OnInputOfferedItem()
    {
        TryCombineInput();
    }

    private void Update()
    {
        if(IsWaitingForOutput)
        {
            if (!output.HasCart)
                TryCombineInput();
        }
    }

    private void TryCombineInput()
    {
        if (input1.HasAwaitingCart && input2.HasAwaitingCart && !output.HasCart)
        {
            IsWaitingForOutput = false;

            RailCart cart1 = input1.Pop();
            RailCart cart2 = input2.Pop();

            Item combinedItem = Combiner.Combine(cart1.Content, cart2.Content);

            if (combinedItem != null)
            {
                RailCart newCart = Instantiate(cartPrefab, output.transform.position, cartPrefab.transform.rotation);
                newCart.SetItem(combinedItem);
                cart1.Kill();
                cart2.Kill();
            }
            else
            {
                cart1.Explode();
                cart2.Explode();
            }
        }
    }
}

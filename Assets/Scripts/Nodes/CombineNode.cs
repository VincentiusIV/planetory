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

    public float timeToCombine = 5.0f;

    private bool IsWaitingForOutput, IsWaitingForInput;
    private float combineTimer;

    private void Start()
    {
        input1.OnCartAwaitingTransfer.AddListener(OnInputOfferedItem);
        input2.OnCartAwaitingTransfer.AddListener(OnInputOfferedItem);
        output.OnCartAwaitingTransfer.AddListener(OnOutputOfferedItem);
    }

    private void OnInputOfferedItem()
    {
        TryCombineInput();
    }

    private void OnOutputOfferedItem()
    {
        TrySeparateOutput();
    }

    private void Update()
    {
        if(IsWaitingForOutput)
        {
            if (!output.HasCart)
            {
                combineTimer -= Time.deltaTime;
                if(combineTimer < 0)
                {
                    combineTimer = timeToCombine;
                    TryCombineInput();
                }
            }
        }

        if (IsWaitingForInput)
        {
            if(!input1.HasCart && !input2.HasCart)
            {
                combineTimer -= Time.deltaTime;
                if(combineTimer < 0)
                {
                    combineTimer = timeToCombine;
                    TrySeparateOutput();
                }
            }
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

                RailCart newCart = output.SpawnCart(this, cartPrefab);
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
        else
        {
            IsWaitingForOutput = true;
        }
    }

    private void TrySeparateOutput()
    {
        if(output.HasAwaitingCart && !input1.HasCart && !input2.HasCart)
        {
            IsWaitingForInput = false;

            RailCart cart = output.Pop();

            (Item, Item) splitItem = Combiner.Split(cart.Content);

            if (splitItem.Item1 != null)
            {
                RailCart newCart = input1.SpawnCart(this, cartPrefab);
                newCart.SetItem(splitItem.Item1);
            }

            if (splitItem.Item2 != null)
            {
                RailCart newCart = input2.SpawnCart(this, cartPrefab);
                newCart.SetItem(splitItem.Item2);
            }
            cart.Kill();
        }
        else
        {
            IsWaitingForInput = true;
        }
    }
}

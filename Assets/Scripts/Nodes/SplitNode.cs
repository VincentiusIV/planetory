using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplitNode : Node
{
    public RailNode input, output1, output2;

    public RailCart cartPrefab;

    public float timeToCombine = 5.0f;

    private bool IsWaitingForOutput;
    private float combineTimer;

    private void Start()
    {
        input.OnCartAwaitingTransfer.AddListener(OnInputOfferedItem);
    }

    private void OnInputOfferedItem()
    {
        TrySplitInput();
    }

    private void TrySplitInput()
    {
        if (input.HasAwaitingCart && !output1.HasCart && !output2.HasCart)
        {
            IsWaitingForOutput = false;

            RailCart cart = input.Pop();

            (Item, Item) splitItem = Combiner.Split(cart.Content);

            if(splitItem.Item1 != null)
            {
                RailCart newCart = output1.SpawnCart(this, cartPrefab);
                newCart.SetItem(splitItem.Item1);
            }

            if (splitItem.Item2 != null)
            {
                RailCart newCart = output2.SpawnCart(this, cartPrefab);
                newCart.SetItem(splitItem.Item2);
            }

            cart.Kill();
        }
    }
}

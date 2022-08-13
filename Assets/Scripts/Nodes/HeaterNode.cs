using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HeaterNode : Node
{
    public UnityEvent OnHeating;
    public RailNode input, output;
    public RailCart cartPrefab;

    public float time = 5.0f;
    private bool IsWaitingForOutput, IsWaitingForInput;
    private float timer;

    private void Start()
    {
        input.OnCartAwaitingTransfer.AddListener(OnInputOfferedItem);
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
        if (IsWaitingForOutput)
        {
            if (!output.HasCart)
            {
                timer -= Time.deltaTime;
                if (timer < 0)
                {
                    timer = time;
                    TryCombineInput();
                }
            }
        }

        if (IsWaitingForInput)
        {
            if (!input.HasCart)
            {
                timer -= Time.deltaTime;
                if (timer < 0)
                {
                    timer = time;
                    TrySeparateOutput();
                }
            }
        }
    }

    private void TryCombineInput()
    {
        if (input.HasAwaitingCart &&  !output.HasCart)
        {
            IsWaitingForOutput = false;

            RailCart cart1 = input.Pop();

            Item heatedItem = Combiner.Heat(cart1.Content);

            if (heatedItem != null)
            {
                OnHeating.Invoke();
                RailCart newCart = output.SpawnCart(this, cartPrefab);
                newCart.SetItem(heatedItem);
                cart1.Kill();
            }
            else
            {
                cart1.Explode();
            }
        }
        else
        {
            IsWaitingForOutput = true;
        }
    }

    private void TrySeparateOutput()
    {
        if (output.HasAwaitingCart && !input.HasCart)
        {
            IsWaitingForInput = false;

            RailCart cart1 = output.Pop();

            Item heatedItem = Combiner.Heat(cart1.Content);

            if (heatedItem != null)
            {
                OnHeating.Invoke();
                RailCart newCart = input.SpawnCart(this, cartPrefab);
                newCart.SetItem(heatedItem);
                cart1.Kill();
            }
            else
            {
                cart1.Explode();
            }
        }
        else
        {
            IsWaitingForInput = true;
        }
    }
}

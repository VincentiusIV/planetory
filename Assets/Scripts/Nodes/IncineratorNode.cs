using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class IncineratorNode : Node
{
    public UnityEvent OnIncinerate;
    public RailNode inputRail;

    private void Start()
    {
        inputRail.OnCartAwaitingTransfer.AddListener(OnInputOfferedItem);
    }

    private void OnInputOfferedItem()
    {
        RailCart cart = inputRail.Pop();
        cart.Explode();
        OnIncinerate.Invoke();
    }
}

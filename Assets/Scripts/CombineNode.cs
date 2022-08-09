using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 2 In, 1 Out
/// </summary>
public class CombineNode : Node
{
    public StraightRailNode input1, input2, output;

    private void Start()
    {
        input1.OnCartAwaitingTransfer.AddListener(OnInputOfferedItem);
        input2.OnCartAwaitingTransfer.AddListener(OnInputOfferedItem);
    }

    private void OnInputOfferedItem()
    {
        // Check items offered on both rails,
        //      if they can be combined
        //          combine them and output
        //      
    }
}

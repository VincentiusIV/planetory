using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceNode : Node, IOutput
{
    public Item outputItem;
    public RailCart cartPrefab;
    public RailNode outputRail;

    private void Start()
    {
        NodeGrid.Instance.PlaceNode(outputRail);
    }

    private void Update()
    {

    }
}

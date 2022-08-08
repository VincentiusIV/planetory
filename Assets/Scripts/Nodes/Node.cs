using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IInput 
{
    void InsertItem(Item item);
}

public interface IOutput
{

}

public class Node : MonoBehaviour
{
    public bool IsInPreviewMode { get; private set; }
    public UnityEvent OnOfferingItem;
    
    private Node[] subnodes;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private int originalSortingOrder;

    protected virtual void Awake()
    {
        List<Node> nodes = new List<Node>(GetComponentsInChildren<Node>());
        nodes.Remove(this);
        subnodes = nodes.ToArray();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
        originalSortingOrder = spriteRenderer.sortingOrder;
    }

    public virtual BuildHandler CreateBuildHandler(BuildResource resource)
    {
        return new SingleBuildHandler(resource);
    }

    public bool Place()
    {
        if (!CanWholeNodeBePlaced())
            return false;
        ExitPreviewMode();
        bool wasPlaced = NodeGrid.Instance.PlaceNode(this);
        foreach (var subnode in subnodes)
            wasPlaced &= subnode.Place();
        Debug.Assert(wasPlaced);
        return wasPlaced;
    }

    public void EnterPreviewMode()
    {
        IsInPreviewMode = true;
        spriteRenderer.sortingOrder = 100;
        foreach (var subnode in subnodes)
            subnode.EnterPreviewMode();
    }

    public void ExitPreviewMode()
    {
        IsInPreviewMode = false;
        spriteRenderer.color = originalColor;
        spriteRenderer.sortingOrder = originalSortingOrder;
        foreach (var subnode in subnodes)
            subnode.ExitPreviewMode();
    }

    private void Update()
    {
        if (IsInPreviewMode)
        {
            spriteRenderer.color = CanBePlaced() ? Color.green : Color.red;
        }
    }

    public bool CanWholeNodeBePlaced()
    {
        bool canBePlaced = CanBePlaced();
        foreach (var subnode in subnodes)
            canBePlaced &= subnode.CanBePlaced();
        return canBePlaced;
    }

    public bool CanBePlaced()
    {
        NodeSlot slot = NodeGrid.Instance.GetSlot(transform.position);
        if (slot != null)
            return !slot.IsFilled;
        else
            return false;
    }
}

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

    private Node[] subnodes;
    private PreviewNode previewNode;

    protected virtual void Awake()
    {
        List<Node> nodes = new List<Node>(GetComponentsInChildren<Node>());
        nodes.Remove(this);
        subnodes = nodes.ToArray();
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
        previewNode = Instantiate(Builder.Instance.previewNodePrefab, transform);
        previewNode.Track(this);
        foreach (var subnode in subnodes)
            subnode.EnterPreviewMode();
    }

    public void ExitPreviewMode()
    {
        IsInPreviewMode = false;
        Destroy(previewNode.gameObject);
        foreach (var subnode in subnodes)
            subnode.ExitPreviewMode();
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

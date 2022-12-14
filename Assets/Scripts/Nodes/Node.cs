using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Node : MonoBehaviour
{
    public bool IsInPreviewMode { get; private set; }
    public bool IsDestructable = true;
    public Node[] subnodes { get; private set; }
    public Sprite Icon;
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
    
    public virtual bool CanBeCombinedWith(Node node)
    {
        return false;
    }

    public bool Place()
    {
        if (!CanWholeNodeBePlaced())
            return false;
        ExitPreviewMode();
        bool wasPlaced = NodeGrid.Instance.Place(this);
        foreach (var subnode in subnodes)
            wasPlaced &= subnode.Place();
        Debug.Assert(wasPlaced);
        return wasPlaced;
    }

    public void Kill()
    {
        Node node = this;
        if(transform.parent != null)
            node = transform.parent.GetComponentInParent<Node>();
        if (node == null)
            node = this;
        Destroy(node.gameObject);
    }

    protected virtual void OnDestroy()
    {
        NodeGrid.Remove(this);
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
        if(previewNode != null)
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
            return slot.CanPlace(this);
        else
            return false;
    }
}

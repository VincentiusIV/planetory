using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class NodeSlot
{
    public Vector2Int Position { get; }
    public bool HasNodes => nodes.Count > 0;
    public List<Node> nodes = new List<Node>();

    public NodeSlot(int x, int y)
    {
        Position = new Vector2Int(x, y);
    }

    public bool Fill(Node node)
    {
        if (!CanPlace(node))
            return false;
        nodes.Add(node);
        return true;
    }

    public bool Remove(Node node)
    {
        if (nodes.Contains(node))
            return nodes.Remove(node);
        return false;
    }

    public bool CanPlace(Node newNode)
    {
        bool canBeCombined = true;
        foreach (var currentNode in nodes)
            canBeCombined &= currentNode.CanBeCombinedWith(newNode);
        return !HasNodes || canBeCombined;
    }

    public void Clear()
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            Node node = nodes[i];
            if(node.IsDestructable)
            {
                node.Kill();
                nodes.RemoveAt(i--);
            }
        }
    }
}

public class NodeGrid : Singleton<NodeGrid>
{
    public NodeSlot[,] nodes;
    public Transform emptyTileRoot;
    public GameObject emptyTilePrefab;

    public int width = 32, height = 32;

    protected override void Awake()
    {
        base.Awake();
        nodes = new NodeSlot[width, height];

        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                nodes[x, y] = new NodeSlot(x, y);
    }

    public void Generate()
    {
        emptyTileRoot.DestroyChildren();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 pos = GetGridPosition(x, y);
                GameObject newEmptyTile = Instantiate(emptyTilePrefab, pos, Quaternion.identity);
                newEmptyTile.transform.SetParent(emptyTileRoot);
            }
        }
    }

    private Vector3 GetGridPosition(int x, int y)
    {
        return new Vector3(-width / 2 + x + 0.5f, -height / 2 + y + 0.5f, 0);
    }

    public Vector3 SnapToGrid(Vector3 position)
    {
        int x = Mathf.FloorToInt(position.x + width / 2);
        int y = Mathf.FloorToInt(position.y + height / 2);
        return GetGridPosition(x, y);
    }

    public NodeSlot GetSlot(Vector3 position)
    {
        int x = Mathf.FloorToInt(position.x + width / 2);
        int y = Mathf.FloorToInt(position.y + height / 2);
        if (x >= 0 && x < width && y >= 0 && y < height)
            return nodes[x, y];
        else
            return null;
    }

    public bool Place(Node node)
    {
        NodeSlot slot = GetSlot(node.transform.position);
        if (slot == null)
            return false;
        if (slot.CanPlace(node))
            return slot.Fill(node);
        else
            return false;        
    }

    public static bool Remove(Node node)
    {
        if (!Instance)
            return true;
        NodeSlot slot = Instance.GetSlot(node.transform.position);
        if (slot == null)
            return false;
        return slot.Remove(node);
    }

    private void OnDrawGizmos()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 pos = GetGridPosition(x, y);
                Gizmos.DrawWireCube(pos, Vector3.one);
            }
        }
    }

}

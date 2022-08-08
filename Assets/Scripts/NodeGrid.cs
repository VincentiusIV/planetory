using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class NodeSlot
{
    public Vector2Int Position { get; }
    public bool IsFilled => node != null;
    public Node node;

    public NodeSlot(int x, int y)
    {
        Position = new Vector2Int(x, y);
    }

    public bool Fill(Node node)
    {
        if (!CanPlace(node))
            return false;
        this.node = node;
        return true;
    }

    public bool CanPlace(Node node)
    {
        return !IsFilled;
    }

    public void Clear()
    {
        Object.Destroy(node.gameObject);
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

    public bool PlaceNode(Node node)
    {
        NodeSlot slot = GetSlot(node.transform.position);
        if (slot == null)
            return false;
        if (slot.CanPlace(node))
            return slot.Fill(node);
        else
            return false;
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class RailNode : Node
{
    public UnityEvent OnCartAwaitingTransfer { get; set; } = new UnityEvent();
    public bool HasAwaitingCart => AwaitingCart != null;
    public bool HasCart => currentCarts.Count > 0;
    protected RailCart AwaitingCart;
    private List<RailCart> currentCarts = new List<RailCart>();
    private int nextRailIdx = 0;

    public void OnCartEnter(RailCart cart)
    {
        if(!currentCarts.Contains(cart))
            currentCarts.Add(cart);
        ++nextRailIdx;
    }

    public void OnCartExit(RailCart cart)
    {
        if (currentCarts.Contains(cart))
            currentCarts.Remove(cart);
    }

    private void OnDestroy()
    {
        foreach (var cart in currentCarts)
        {
            if(cart != null)
                Destroy(cart.gameObject);
        }
    }

    public void Push(RailCart railCart)
    {
        Debug.Assert(AwaitingCart == null);
        AwaitingCart = railCart;
        OnCartAwaitingTransfer?.Invoke();
    }

    public RailCart Pop()
    {
        RailCart cart = AwaitingCart;
        AwaitingCart = null;
        return cart;
    }

    public bool Contains(RailCart cart)
    {
        Vector3 diff = cart.transform.position - transform.position;
        return Mathf.Abs(diff.x) < 0.49f && Mathf.Abs(diff.y) < 0.49f;
    }

    public RailNode Next(Vector3 dir)
    {
        dir = dir.RoundToInt();

        NodeSlot nextSlot = NodeGrid.Instance.GetSlot(transform.position + dir);
        if (nextSlot == null || !nextSlot.HasNodes)
            return null;
        (Vector3, bool) originEndpoint = GetEndpoint(dir);
        if (!originEndpoint.Item2)
            // this node doesnt have an endpoint in given direction.
            return null;

        List<RailNode> validNodes = new List<RailNode>();
        foreach (var node in nextSlot.nodes)
        {
            RailNode railNode = node as RailNode;
            if(railNode != null)
            {
                (Vector3, bool) connectedEndpoint = railNode.GetEndpoint(-dir);
                if (connectedEndpoint.Item2)
                    validNodes.Add(railNode);
            }
        }

        if(validNodes.Count > 0)
            return validNodes[nextRailIdx % validNodes.Count];
        else
            return null;
    }

    public override bool CanBeCombinedWith(Node node)
    {
        return (node as RailNode) != null;
    }

    public RailCart SpawnCart(RailCart cartPrefab)
    {
        return Instantiate(cartPrefab, transform.position, transform.rotation);
    }

    public (Vector3, bool) GetEndpoint(Vector3 dir)
    {
        (Vector3, Vector3) endpoints = GetEndpoints();
        Vector3 calculatedEndpoint = transform.position + dir * 0.5f;
        float diff1 = (endpoints.Item1 - calculatedEndpoint).sqrMagnitude;
        if (diff1 < 0.0001f)
            return (endpoints.Item1, true);
        float diff2 = (endpoints.Item2 - calculatedEndpoint).sqrMagnitude;
        if(diff2 < 0.0001f)
            return (endpoints.Item2, true);
        return (transform.position, false);
    }

    public abstract Vector3 Constrain(Vector3 position);
    public abstract (Vector3, Vector3) GetEndpoints();
}

public class StraightRailNode : RailNode
{
    public Vector3 inPositionLocal, outPositionLocal;

    public Vector3 inPositionWorld => transform.TransformPoint(inPositionLocal);
    public Vector3 outPositionWorld => transform.TransformPoint(outPositionLocal);

    public override Vector3 Constrain(Vector3 position)
    {
        return MathHelper.FindNearestPointOnLine(inPositionWorld, outPositionWorld, position);
    }

    public override (Vector3, Vector3) GetEndpoints()
    {
        return (inPositionWorld, outPositionWorld);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(inPositionWorld, outPositionWorld);
    }
}

public static class MathHelper 
{
    public static Vector2 FindNearestPointOnLine(Vector2 origin, Vector2 end, Vector2 point)
    {
        Vector2 heading = (end - origin);
        float magnitudeMax = heading.magnitude;
        heading.Normalize();
        Vector2 lhs = point - origin;
        float dotP = Vector2.Dot(lhs, heading);
        dotP = Mathf.Clamp(dotP, 0f, magnitudeMax);
        return origin + heading * dotP;
    }

}

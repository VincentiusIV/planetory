using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class RailNode : Node
{
    public UnityEvent OnCartAwaitingTransfer { get; set; } = new UnityEvent();
    public bool HasAwaitingCart => AwaitingCart != null;
    public bool HasCart => currentCarts.Count > 0;
    public bool allowCombinations = true;
    public bool isEndpoint = false;
    public int endpointTriggerIndex = 0;
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

    protected override void OnDestroy()
    {
        base.OnDestroy();

        foreach (var cart in currentCarts)
        {
            if (cart != null)
                Destroy(cart.gameObject);
        }
    }

    public void Push(RailCart railCart)
    {
        if (!isEndpoint)
            return;
        (Vector3, Vector3) endpoints = GetEndpoints();
        Vector3 endpoint = endpointTriggerIndex == 0 ? endpoints.Item1 : endpoints.Item2;
        Vector3 diff = endpoint - railCart.FrontPosition;
        if(diff.sqrMagnitude < 0.1f)
        {
            Debug.Assert(AwaitingCart == null);
            AwaitingCart = railCart;
            OnCartAwaitingTransfer?.Invoke();
        }
    }

    public Vector3 GetTriggerEndpoint()
    {
        (Vector3, Vector3) endpoints = GetEndpoints();
        return endpointTriggerIndex == 0 ? endpoints.Item1 : endpoints.Item2;
    }

    public RailCart Pop()
    {
        if (!isEndpoint)
            return null;

        RailCart cart = AwaitingCart;
        AwaitingCart = null;
        return cart;
    }

    public bool Contains(RailCart cart, Vector3 position)
    {
        Vector3 diff = position - transform.position;
        return Mathf.Abs(diff.x) < 0.49f && Mathf.Abs(diff.y) < 0.49f;
    }

    public RailNode Next(Vector3 dir)
    {
        (Vector3, Vector3) endpointDirs = GetEndpointDirections();
        Vector3 dirToUse = endpointDirs.Item1;
        if (Vector3.Dot(dir, endpointDirs.Item1) < Vector3.Dot(dir, endpointDirs.Item2))
            dirToUse = endpointDirs.Item2;

        NodeSlot nextSlot = NodeGrid.Instance.GetSlot(transform.position + dirToUse);
        if (nextSlot == null || !nextSlot.HasNodes)
            return null;
        (Vector3, bool) originEndpoint = GetEndpoint(dirToUse);
        if (!originEndpoint.Item2)
            // this node doesnt have an endpoint in given direction.
            return null;

        List<RailNode> validNodes = new List<RailNode>();
        foreach (var node in nextSlot.nodes)
        {
            RailNode railNode = node as RailNode;
            if(railNode != null)
            {
                (Vector3, bool) connectedEndpoint = railNode.GetEndpoint(-dirToUse);
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
        float thisYRot = ClampAngle(transform.eulerAngles.z, -180.0f, 180.0f);
        float theirYRot = ClampAngle(node.transform.eulerAngles.z, -180.0f, 180.0f);

        return !isEndpoint && allowCombinations && ((node as RailNode) != null && 
            ((thisYRot != theirYRot) ||
            (node.GetType() != GetType())));
    }

    static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -180.0f)
            angle += 360.0f;
        if (angle > 180.0f)
            angle -= 360.0f;
        return Mathf.Clamp(angle, min, max);
    }

    public RailCart SpawnCart(Node sourceNode, RailCart cartPrefab)
    {
        (Vector3, Vector3) outputEndpoints = GetEndpoints();
        Vector3 diff1 = outputEndpoints.Item1 - sourceNode.transform.position;
        Vector3 diff2 = outputEndpoints.Item2 - sourceNode.transform.position;
        Vector3 spawnPos = outputEndpoints.Item1 - (outputEndpoints.Item1 -transform.position).normalized * 0.05f;
        if (diff1.sqrMagnitude > diff2.sqrMagnitude)
            spawnPos = outputEndpoints.Item2 - (outputEndpoints.Item2 - transform.position).normalized * 0.05f;
        RailCart newCart = Instantiate(cartPrefab, spawnPos, transform.rotation);
        newCart.Setup(this);
        return newCart;
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

    public (Vector3, Vector3) GetEndpointDirections()
    {
        (Vector3, Vector3) endpoints = GetEndpoints();
        return (
            (endpoints.Item1 - transform.position).normalized,
            (endpoints.Item2 - transform.position).normalized
        );
    }
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

        if(isEndpoint)
        {
            (Vector3, Vector3) endpoints = GetEndpoints();
            Vector3 endpoint = endpointTriggerIndex == 0 ? endpoints.Item1 : endpoints.Item2;
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(endpoint, 0.1f);
        }
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

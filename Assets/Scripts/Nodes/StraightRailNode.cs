using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class RailNode : Node
{
    public UnityEvent OnCartAwaitingTransfer { get; set; } = new UnityEvent();

    protected RailCart AwaitingCart;

    public void Push(RailCart railCart)
    {
        Debug.Assert(AwaitingCart == null);
        AwaitingCart = railCart;
        OnCartAwaitingTransfer.Invoke();
    }

    public RailCart Pop()
    {
        RailCart cart = AwaitingCart;
        AwaitingCart = null;
        return cart;
    }

    public abstract Vector3 Constrain(Vector3 position);
}

public class StraightRailNode : RailNode
{
    public Vector3 inPositionLocal, outPositionLocal;

    public Vector3 inPositionWorld => transform.TransformPoint(inPositionLocal);
    public Vector3 outPositionWorld => transform.TransformPoint(outPositionLocal);

    public override Vector3 Constrain(Vector3 position)
    {
        return FindNearestPointOnLine(inPositionWorld, outPositionWorld, position);
    }

    public Vector2 FindNearestPointOnLine(Vector2 origin, Vector2 end, Vector2 point)
    {
        Vector2 heading = (end - origin);
        float magnitudeMax = heading.magnitude;
        heading.Normalize();
        Vector2 lhs = point - origin;
        float dotP = Vector2.Dot(lhs, heading);
        dotP = Mathf.Clamp(dotP, 0f, magnitudeMax);
        return origin + heading * dotP;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(inPositionWorld, outPositionWorld);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CornerRailNode : RailNode
{
    public Vector3[] points;

    protected override void Awake()
    {
        base.Awake();
        if (points.Length < 2)
        {
            Debug.LogError("please assing some points to corner rail node...");
            Destroy(gameObject);
        }
    }

    public override Vector3 Constrain(Vector3 position)
    {
        Vector3 closestPoint = position;
        float closestDistance = float.MaxValue;
        for (int i = 0; i < points.Length - 1; i++)
        {
            Vector3 a = GetWorldPoint(i);
            Vector3 b = GetWorldPoint(i + 1);
            Vector3 closest = MathHelper.FindNearestPointOnLine(a, b, position);
            Vector3 delta = (closest - position);
            if (delta.magnitude < closestDistance)
            {
                closestDistance = delta.magnitude;
                closestPoint = closest;
            }
        }
        return closestPoint;
    }

    public override (Vector3, Vector3) GetEndpoints()
    {
        return (GetWorldPoint(0), GetWorldPoint(points.Length - 1));
    }

    private Vector3 GetWorldPoint(int index) => transform.TransformPoint(points[index]);

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        for (int i = 0; i < points.Length - 1; i++)
        {
            Vector3 a = GetWorldPoint(i);
            Vector3 b = GetWorldPoint(i + 1);
            Gizmos.DrawLine(a, b);
        }
    }
}

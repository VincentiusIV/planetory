using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public static class TransformExtensions
{
    public static Transform GetHighestParent(this Transform transform)
    {
        Transform highest = transform;
        while (highest.parent != null)
            highest = highest.parent;
        return highest;
    }

    public static void DestroyChildren(this Transform transform)
    {
        Transform[] children = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
            children[i] = transform.GetChild(i);
        foreach (var child in children)
        {
            if (Application.isPlaying)
                Object.Destroy(child.gameObject);
            else
                Object.DestroyImmediate(child.gameObject);
        }
    }

    public static Transform FindInChildren(this Transform trans, string name)
    {
        if (trans == null)
            return null;

        if (trans.name == name || trans.name == name)
            return trans;
        else
        {
            Transform found;

            for (int i = 0; i < trans.childCount; i++)
            {
                found = FindInChildren(trans.GetChild(i), name);
                if (found != null)
                    return found;
            }

            return null;
        }
    }

}

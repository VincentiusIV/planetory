using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(NodeGrid))]
public class NodeGridEditor : Editor
{
    NodeGrid grid;

    public void OnEnable()
    {
        grid = (NodeGrid)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Generate"))
            grid.Generate();
    }
}

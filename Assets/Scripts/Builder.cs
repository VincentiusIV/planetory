using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BuildResource
{
    public GameObject prefab;

    public Node GetNodePrefab()
    {
        Node node = prefab.GetComponent<Node>();
        Debug.Assert(node != null);
        return node; 
    }

    public BuildHandler CreateBuildHandler()
    {
        return GetNodePrefab().CreateBuildHandler(this);
    }
}

public abstract class BuildHandler
{
    public BuildResource Resource { get; }

    public BuildHandler(BuildResource resource)
    {
        Resource = resource ?? throw new ArgumentNullException(nameof(resource));
    }

    public abstract bool StartBuild();
    public abstract bool ConfirmBuild();
    public abstract void Update();
    public abstract bool CancelBuild();
}

public class Builder : Singleton<Builder>
{
    public BuildResource[] resources;

    private BuildHandler activeBuildHandler;

    private void Update()
    {
        activeBuildHandler?.Update();

        for (int i = 0; i < Mathf.Min(9, resources.Length); i++)
        {
            KeyCode alphaNumeric = (KeyCode)((int)KeyCode.Alpha1 + i);
            if (Input.GetKeyDown(alphaNumeric))
            {
                activeBuildHandler?.CancelBuild();
                activeBuildHandler = resources[i].CreateBuildHandler();
                activeBuildHandler?.StartBuild();
            }
        }

        if(activeBuildHandler != null)
        {
            if (Input.GetMouseButtonDown(0))
            {
                activeBuildHandler.ConfirmBuild();
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                activeBuildHandler?.CancelBuild();
                activeBuildHandler = null;
            }
        }
        
    }

    private void OnValidate()
    {
        for (int i = 0; i < resources.Length; i++)
        {
            if (resources[i].prefab == null)
                continue;
            if(resources[i].prefab.GetComponent<Node>() == null)
            {
                Debug.LogWarning("resource prefab doesnt have a node script!");
            }
        }
    }
}

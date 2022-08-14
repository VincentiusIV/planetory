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
    public abstract void DeleteNode();
}

public class Builder : Singleton<Builder>
{
    public bool IsInDemolitionMode { get; private set; }
    public bool IsEnabled { get; private set; } = true;
    public NodeSlot SelectedSlot { get; private set; }
    public Vector3 SelectionPosition { get; private set; }
    public BuildResource[] resources;
    public PreviewNode previewNodePrefab;
    public LayerMask clickPlaneLayer;
    public FXPlayCombo placeFX;

    public Texture2D defaultCursor, demolitionCursor;

    private BuildHandler activeBuildHandler;

    protected override void Awake()
    {
        base.Awake();
        Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.ForceSoftware);
    }

    private void Update()
    {
        if (!IsEnabled)
            return;
        UpdateSelectedSlot();

        activeBuildHandler?.Update();

        for (int i = 0; i < Mathf.Min(9, resources.Length); i++)
        {
            KeyCode alphaNumeric = (KeyCode)((int)KeyCode.Alpha1 + i);
            if (Input.GetKeyDown(alphaNumeric))
            {
                StartBuilding(resources[i]);
            }
        }

        if(activeBuildHandler != null)
        {
            if (Input.GetMouseButton(0))
            {
                if (IsInDemolitionMode)
                    activeBuildHandler.DeleteNode();
                else
                    activeBuildHandler.ConfirmBuild();
            }

#if UNITY_EDITOR
            if(Input.GetMouseButton(1))
            {
                 activeBuildHandler.DeleteNode();
            }
#endif
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                activeBuildHandler?.CancelBuild();
                activeBuildHandler = null;
            }
        }
        
    }

    private void UpdateSelectedSlot()
    {
        Vector3 mouseScreenPosition = Input.mousePosition;
        if (Camera.main.orthographic)
        {
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);
            Vector3 gridPosition = NodeGrid.Instance.SnapToGrid(mouseWorldPosition);
            SelectedSlot = NodeGrid.Instance.GetSlot(gridPosition);
            SelectionPosition = gridPosition;
        }
        else
        {
            Ray ray = Camera.main.ScreenPointToRay(mouseScreenPosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Camera.main.farClipPlane, Builder.Instance.clickPlaneLayer))
            {
                Vector3 mouseHitPosition = hit.point;
                Vector3 gridPosition = NodeGrid.Instance.SnapToGrid(mouseHitPosition);
                SelectedSlot = NodeGrid.Instance.GetSlot(gridPosition);
                SelectionPosition = gridPosition;
            }
        }
    }

    public void StartBuilding(BuildResource resource)
    {
        StopDemolition();
        activeBuildHandler?.CancelBuild();
        activeBuildHandler = resource.CreateBuildHandler();
        activeBuildHandler?.StartBuild();
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

    public void SwitchDemolitionMode()
    {
        if (IsInDemolitionMode)
            StopDemolition();
        else
            StartDemolition();
    }

    private void StartDemolition()
    {
        activeBuildHandler?.CancelBuild();
        IsInDemolitionMode = true;
        Cursor.SetCursor(demolitionCursor, Vector2.zero, CursorMode.ForceSoftware);
    }

    private void StopDemolition()
    {
        IsInDemolitionMode = false;
        Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.ForceSoftware);
    }

    public void Disable()
    {
        IsEnabled = false;
        activeBuildHandler?.CancelBuild();
        StopDemolition();
    }

    internal void Enable()
    {
        IsEnabled = true;
    }
}

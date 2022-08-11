using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleBuildHandler : BuildHandler
{
    private Node prefabInstance;
    private NodeSlot selectedSlot;
    private Quaternion instanceRotation = Quaternion.identity;

    public SingleBuildHandler(BuildResource resource) : base(resource)
    {

    }

    public override bool StartBuild()
    {
        if (Resource.prefab == null)
            return false;
        prefabInstance = Object.Instantiate(Resource.prefab, Vector3.zero, instanceRotation).GetComponent<Node>();
        prefabInstance.EnterPreviewMode();
        return true;
    }

    public override void Update()
    {
        if (prefabInstance == null)
            return;
        Vector3 mouseScreenPosition = Input.mousePosition;
        if(Camera.main.orthographic)
        {
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);
            Vector3 gridPosition = NodeGrid.Instance.SnapToGrid(mouseWorldPosition);
            selectedSlot = NodeGrid.Instance.GetSlot(gridPosition);
            prefabInstance.transform.position = gridPosition;
        }
        else
        {
            Ray ray = Camera.main.ScreenPointToRay(mouseScreenPosition);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit, Camera.main.farClipPlane, Builder.Instance.clickPlaneLayer))
            {
                Vector3 mouseHitPosition = hit.point;
                Vector3 gridPosition = NodeGrid.Instance.SnapToGrid(mouseHitPosition);
                selectedSlot = NodeGrid.Instance.GetSlot(gridPosition);
                prefabInstance.transform.position = gridPosition;
            }
        }

        if(Input.GetKeyDown(KeyCode.R))
        {
            instanceRotation *= Quaternion.Euler(0, 0, 90);
            prefabInstance.transform.rotation = instanceRotation;
        }
    }

    public override bool ConfirmBuild()
    {
        bool result = prefabInstance.Place();
        if(result)
            StartBuild();
        return result;
    }

    public override bool CancelBuild()
    {
        if (prefabInstance == null)
            return false;
        Object.Destroy(prefabInstance.gameObject);
        return true;
    }

    public override void DeleteNode()
    {
        selectedSlot?.Clear();
    }
}

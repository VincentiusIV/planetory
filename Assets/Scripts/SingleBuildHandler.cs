using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleBuildHandler : BuildHandler
{
    private Node prefabInstance;

    public SingleBuildHandler(BuildResource resource) : base(resource)
    {

    }

    public override bool StartBuild()
    {
        if (Resource.prefab == null)
            return false;
        prefabInstance = Object.Instantiate(Resource.prefab, Vector3.zero, Quaternion.identity).GetComponent<Node>();
        prefabInstance.EnterPreviewMode();
        return true;
    }

    public override void Update()
    {
        if (prefabInstance == null)
            return;
        Vector3 mouseScreenPosition = Input.mousePosition;
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);
        Vector3 gridPosition = NodeGrid.Instance.SnapToGrid(mouseWorldPosition);
        prefabInstance.transform.position = gridPosition;       

        if(Input.GetKeyDown(KeyCode.R))
        {
            prefabInstance.transform.rotation *= Quaternion.Euler(0, 0, 90);
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
}

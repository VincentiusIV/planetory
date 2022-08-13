using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleBuildHandler : BuildHandler
{
    private NodeSlot selectedSlot => Builder.Instance.SelectedSlot;
    private Node prefabInstance;
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

        prefabInstance.transform.position = Builder.Instance.SelectionPosition;

        if(Input.GetKeyDown(KeyCode.R) || Input.mouseScrollDelta.y > 0)
        {
            instanceRotation *= Quaternion.Euler(0, 0, 90);
            prefabInstance.transform.rotation = instanceRotation;
        }
        else if(Input.mouseScrollDelta.y < 0)
        {
            instanceRotation *= Quaternion.Euler(0, 0, -90);
            prefabInstance.transform.rotation = instanceRotation;
        }
    }

    public override bool ConfirmBuild()
    {
        if (prefabInstance == null)
            return false;

        bool result = prefabInstance.Place();
        if(result)
        {
            Builder.Instance.StartCoroutine(PlaceFXRoutine(prefabInstance));
            StartBuild();
        }
        return result;
    }

    private IEnumerator PlaceFXRoutine(Node target)
    {
        Vector3[] subpositions = new Vector3[target.subnodes.Length];
        for (int i = 0; i < target.subnodes.Length; i++)
            subpositions[i] = target.subnodes[i].transform.position;
        Builder.Instance.placeFX.transform.position = target.transform.position;
        Builder.Instance.placeFX.Play();
        foreach (var nodePos in subpositions)
        {
            yield return null;
            Builder.Instance.placeFX.transform.position = nodePos;
            Builder.Instance.placeFX.Play();
        }
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
        if(selectedSlot != null && selectedSlot.HasNodes)
            Builder.Instance.StartCoroutine(PlaceFXRoutine(selectedSlot.nodes[0]));
        selectedSlot?.Clear();
    }
}

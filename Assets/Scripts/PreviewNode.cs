using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewNode : MonoBehaviour
{
    public Color validColor = Color.green, invalidColor = Color.red;

    private Node targetNode;
    private MeshRenderer meshRenderer;
    private Material materialInstance;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        materialInstance = Instantiate(meshRenderer.material);
        gameObject.SetActive(false);
    }

    public void Track(Node node)
    {
        targetNode = node;
        gameObject.SetActive(targetNode != null);
    }

    private void Update()
    {
        if (targetNode.IsInPreviewMode)
        {
            materialInstance.color = targetNode.CanBePlaced() ? validColor : invalidColor;
            meshRenderer.material = materialInstance;
        }
    }
}

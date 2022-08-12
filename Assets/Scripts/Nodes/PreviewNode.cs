using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewNode : MonoBehaviour
{
    public Color validColor = Color.green, invalidColor = Color.red;

    private Node targetNode;
    private MeshRenderer meshRenderer;
    private Material materialInstance;

    private MeshRenderer[] targetRenderers = new MeshRenderer[0];
    private Dictionary<MeshRenderer, Material[]> materialBackup = new Dictionary<MeshRenderer, Material[]>();

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.enabled = false;

        materialInstance = Instantiate(meshRenderer.material);
        gameObject.SetActive(false);
    }

    public void Track(Node node)
    {
        targetNode = node;
        gameObject.SetActive(targetNode != null);
        if(targetNode != null)
        {
            List<MeshRenderer> renderers = new List<MeshRenderer>();
            Queue<Transform> objectsToCheck = new Queue<Transform>();
            objectsToCheck.Enqueue(targetNode.transform);
            while(objectsToCheck.Count > 0)
            {
                Transform current = objectsToCheck.Dequeue();

                MeshRenderer renderer = current.GetComponent<MeshRenderer>();
                if(renderer != null)
                    renderers.Add(renderer);
                
                for (int i = 0; i < current.childCount; i++)
                {
                    Transform child = current.GetChild(i);
                    if (child.GetComponent<Node>() != null)
                        continue;
                    objectsToCheck.Enqueue(child);
                }
            }
            targetRenderers = renderers.ToArray();
            materialBackup.Clear();
            foreach (var renderer in targetRenderers)
            {
                materialBackup.Add(renderer, renderer.materials);

                Material[] materials = renderer.materials;
                for (int i = 0; i < materials.Length; i++)
                    materials[i] = materialInstance;
                renderer.materials = materials;
            }
        }
    }

    private void OnDestroy()
    {
        foreach (var renderer in targetRenderers)
            renderer.materials = materialBackup[renderer];
    }

    private void Update()
    {
        if (targetNode.IsInPreviewMode)
        {
            materialInstance.color = targetNode.CanBePlaced() ? validColor : invalidColor;
            foreach (var renderer in targetRenderers)
            {
                Material[] materials = renderer.materials;
                for (int i = 0; i < materials.Length; i++)
                    materials[i] = materialInstance;
                renderer.materials = materials;
            }
            transform.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);
        }
    }
}

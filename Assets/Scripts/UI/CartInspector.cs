using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CartInspector : MonoBehaviour
{
    public GameObject visualRoot;
    public Vector3 worldOffset = new Vector3(0.0f, 0.5f, -2.0f);
    public TMP_Text nameField;

    private void FixedUpdate()
    {
        NodeSlot selectedSlot = Builder.Instance.SelectedSlot;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Camera.main.farClipPlane))
        {
            RailCart cart = hit.collider.GetComponent<RailCart>();
            visualRoot.SetActive(cart != null);
            if(visualRoot.activeSelf)
            {
                nameField.text = cart.Content.name;
                nameField.transform.position = Camera.main.WorldToScreenPoint(cart.transform.position + worldOffset);
            }
        }
        else
        {
            visualRoot.SetActive(false);
        }
    }
}

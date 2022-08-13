using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prebuilder : MonoBehaviour
{
    private void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Node node = transform.GetChild(i).GetComponent<Node>();
            if (node != null)
                node.Place();
        }
    }
}

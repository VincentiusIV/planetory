using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlignWithCamera : MonoBehaviour
{
    public Vector3 offset;

    private void Update()
    {
        transform.rotation = Quaternion.LookRotation(Vector3.up, -Vector3.forward)
            * Quaternion.Euler(offset);

    }
}

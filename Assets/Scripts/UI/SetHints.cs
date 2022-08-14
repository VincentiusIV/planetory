using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetHints : MonoBehaviour
{
    public Hint[] hints;

    void Start()
    {
        HintSystem.Instance.SetHints(hints);
    }
}

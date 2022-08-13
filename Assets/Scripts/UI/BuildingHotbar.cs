using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingHotbar : MonoBehaviour
{
    public BuildingButton[] buildingButtons;

    private void Start()
    {
        BuildResource[] resources = Builder.Instance.resources;
        Debug.Assert(resources.Length <= buildingButtons.Length, "UI cannot support enough resources!");
        for (int i = 0; i < buildingButtons.Length; i++)
        {
            buildingButtons[i].gameObject.SetActive(i < resources.Length);
            if(buildingButtons[i].gameObject.activeSelf)
            {
                buildingButtons[i].Setup(resources[i]);
            }
        }
    }

    public void SwitchDemolitionMode()
    {
        Builder.Instance.SwitchDemolitionMode();
    }
}

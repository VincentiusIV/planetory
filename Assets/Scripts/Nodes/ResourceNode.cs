using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ResourceNode : Node
{
    public float timeBetweenSpawn = 1.0f;

    public Item outputItem;
    public RailCart cartPrefab;
    public RailNode outputRail;
    public TMP_Text itemNameField;
    public MeshRenderer itemIconRenderer;

    private float nextCartSpawnTimer;

    protected override void Awake()
    {
        base.Awake();
        Debug.Assert(outputItem != null);
        NodeGrid.Instance.Place(outputRail);
    }

    private void Start()
    {
        itemNameField.text = outputItem.name;
        itemIconRenderer.material.SetTexture("_BaseMap", outputItem.Icon.texture);
    }

    private void FixedUpdate()
    {
        if(!outputRail.HasCart)
        {
            nextCartSpawnTimer -= Time.fixedDeltaTime;

            if (nextCartSpawnTimer <= 0.0f)
            {
                RailCart newCart = outputRail.SpawnCart(this, cartPrefab);
                newCart.SetItem(outputItem);
                outputRail.OnCartEnter(newCart);
                nextCartSpawnTimer = timeBetweenSpawn;
            }
        }
    }

    private void OnValidate()
    {
        if(outputItem != null && itemNameField != null)
        {
            itemNameField.text = outputItem.name;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class CreateItemsTool
{
    [MenuItem("Factory/CreateItems")]
    public static void CreateItemsFromPNGs()
    {
        Texture[] icons = Resources.LoadAll<Texture>("Icons");
        Item[] items = Resources.LoadAll<Item>("Items");
        Dictionary<string, Item> nameToItems = new Dictionary<string, Item>();
        foreach (var item in items)
            nameToItems.Add(item.name, item);

        foreach (var icon in icons)
        {
            string name = icon.name.Split('_')[0];
            if (nameToItems.ContainsKey(name))
                continue;

            Item newItem = ScriptableObject.CreateInstance<Item>();
            newItem.name = name;
            newItem.Icon = icon;

            nameToItems.Add(name, newItem);
            AssetDatabase.CreateAsset(newItem, $"Assets/Resources/Items/{newItem.name}.asset");
        }
        AssetDatabase.SaveAssets();
    }
}

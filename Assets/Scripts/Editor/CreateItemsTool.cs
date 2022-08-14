using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class CreateItemsTool
{
    [MenuItem("Factory/CreateItems")]
    public static void CreateItemsFromPNGs()
    {
        Sprite[] icons = Resources.LoadAll<Sprite>("Icons");
        Item[] items = Resources.LoadAll<Item>("Items");
        Dictionary<string, Item> nameToItems = new Dictionary<string, Item>();
        foreach (var item in items)
            nameToItems.Add(item.name, item);

        foreach (var icon in icons)
        {
            string name = icon.name.Split('_')[0];
            if (nameToItems.ContainsKey(name))
            {
                Item item = AssetDatabase.LoadAssetAtPath<Item>($"Assets/Resources/Items/{name}.asset");
                Debug.Assert(item != null);
                item.Icon = icon;
                EditorUtility.SetDirty(item);
                continue;
            }

            Item newItem = ScriptableObject.CreateInstance<Item>();
            newItem.name = name;
            newItem.Icon = icon;

            nameToItems.Add(name, newItem);
            AssetDatabase.CreateAsset(newItem, $"Assets/Resources/Items/{newItem.name}.asset");
        }
        AssetDatabase.SaveAssets();
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Combiner : Singleton<Combiner>
{
    private static Item[] itemCollection;

    private static CombineRecipe[] allRecipes;
    private static Dictionary<CombineRecipe, Item> recipeToItem;
    private static Dictionary<Item, List<CombineRecipe>> itemToRecipe;
    private static Dictionary<Item, Item> itemToHeated;
    private static Dictionary<Item, Item> itemToEnergized;

    protected override void Awake()
    {
        base.Awake();

        itemCollection = Resources.LoadAll<Item>("Items");
        Debug.Log($"Found {itemCollection.Length} items!");

        List<CombineRecipe> recipes = new List<CombineRecipe>();
        recipeToItem = new Dictionary<CombineRecipe, Item>();
        itemToRecipe = new Dictionary<Item, List<CombineRecipe>>();
        itemToEnergized = new Dictionary<Item, Item>();
        itemToHeated = new Dictionary<Item, Item>();
        foreach (var item in itemCollection)
        {
            foreach (var recipe in item.recipes)
            {
                recipes.Add(recipe);
                recipeToItem.Add(recipe, item);
                if (!itemToRecipe.ContainsKey(item))
                    itemToRecipe.Add(item, new List<CombineRecipe>());
                itemToRecipe[item].Add(recipe);
            }

            itemToHeated.TryAdd(item, item.HeatingResult);
            itemToEnergized.TryAdd(item, item.EnergizedResult);
        }
        allRecipes = recipes.ToArray();
    }

    public static Item Combine(Item itemA, Item itemB)
    {
        List<Item> possible = new List<Item>();

        foreach (var recipe in allRecipes)
        {
            if (recipe.itemA == itemA && recipe.itemB == itemB ||
               recipe.itemA == itemB && recipe.itemB == itemA)
                return recipeToItem[recipe];
        }
        return null;
    }

    public static (Item, Item) Split(Item content)
    {
        List<CombineRecipe> recipes = new List<CombineRecipe>();
        itemToRecipe.TryGetValue(content, out recipes);
        if(recipes != null && recipes.Count > 0)
        {
            int randIdx = Random.Range(0, recipes.Count);
            CombineRecipe recipe = recipes[randIdx];
            return (recipe.itemA, recipe.itemB);
        }
        return (content, null);
    }

    public static Item Energize(Item item)
    {
        Item result = null;
        itemToEnergized.TryGetValue(item, out result);
        return result;
    }

    public static Item Heat(Item item)
    {
        Item result = null;
        itemToHeated.TryGetValue(item, out result);
        return result;
    }
}

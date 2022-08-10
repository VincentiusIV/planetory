using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combiner : Singleton<Combiner>
{
    private static Item[] itemCollection;

    private static CombineRecipe[] allRecipes;
    private static Dictionary<CombineRecipe, Item> recipeToItem;

    protected override void Awake()
    {
        base.Awake();

        itemCollection = Resources.LoadAll<Item>("Items");
        Debug.Log($"Found {itemCollection.Length} items!");

        List<CombineRecipe> recipes = new List<CombineRecipe>();
        recipeToItem = new Dictionary<CombineRecipe, Item>();
        foreach (var item in itemCollection)
        {
            foreach (var recipe in item.recipes)
            {
                recipes.Add(recipe);
                recipeToItem.Add(recipe, item);
            }
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
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct CombineRecipe
{
    public Item itemA, itemB;
}

[CreateAssetMenu(menuName = "Factory/Item")]
public class Item : ScriptableObject
{
    public Texture Icon;

    public CombineRecipe[] recipes;

    public Item HeatingResult;
    public Item EnergizedResult;
}

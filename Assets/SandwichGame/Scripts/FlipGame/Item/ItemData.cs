using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "ScriptableObjects/Item Data", order = 1)]
public class ItemData : ScriptableObject
{
    public List<Item> Items;
}

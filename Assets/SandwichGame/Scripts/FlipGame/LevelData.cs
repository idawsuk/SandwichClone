using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "ScriptableObjects/Level Data", order = 1)]
public class LevelData : ScriptableObject
{
    public List<ItemGrid> LevelItems;

    [System.Serializable]
    public struct ItemGrid
    {
        public Item Item;
        public Vector2Int GridPosition;
    }

    [ContextMenu("Validate Level")]
    public void Validate()
    {
        for (int i = 0; i < LevelItems.Count; i++)
        {
            for (int j = 1; j < LevelItems.Count; j++)
            {
                if(i != j && LevelItems[i].GridPosition == LevelItems[j].GridPosition)
                {
                    Debug.LogError("Stacking items detected on index : " + i + " and " + j);
                    return;
                }
            }

            if(LevelItems[i].GridPosition.x < 0 || LevelItems[i].GridPosition.y < 0)
            {
                Debug.LogError("Position must be positive numbers");
                return;
            }
        }

        Debug.Log("Nice");
    }
}

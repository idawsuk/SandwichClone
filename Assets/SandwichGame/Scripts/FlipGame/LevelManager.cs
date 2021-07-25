using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelManager", menuName = "ScriptableObjects/Level Manager", order = 1)]
public class LevelManager : ScriptableObject
{
    [SerializeField] List<LevelData> levelData;

    public LevelData GetCurrentLevel(int index)
    {
        if (index > levelData.Count - 1)
            return null;

        return levelData[index];
    }

    public void AddLevel(LevelData data)
    {
        levelData.Add(data);
    }
}

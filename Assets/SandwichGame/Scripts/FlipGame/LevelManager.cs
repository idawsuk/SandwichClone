using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] List<LevelData> levelData;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public LevelData GetCurrentLevel(int index)
    {
        if (index > levelData.Count - 1)
            return null;

        return levelData[index];
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEditor : MonoBehaviour
{
    [SerializeField] LevelManager levelManager;
    [SerializeField] ItemData itemData;

    public string path = "Assets/SandwichGame/Data/Level/";

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public List<string> GetItems()
    {
        List<string> items = new List<string>();

        items.Add("Empty");
        for (int i = 0; i < itemData.Items.Count; i++)
        {
            items.Add(itemData.Items[i].Name);
        }

        return items;
    }

    public void SaveLevel(string levelName, List<LevelData.ItemGrid> itemGrid)
    {
#if UNITY_EDITOR
        var obj = ScriptableObject.CreateInstance<LevelData>();

        obj.LevelItems = itemGrid;

        string assetPath = path + levelName + ".asset";

        UnityEditor.AssetDatabase.CreateAsset(obj, assetPath);
        UnityEditor.AssetDatabase.SaveAssets();
        UnityEditor.AssetDatabase.Refresh();

        LevelData newLevelData = UnityEditor.AssetDatabase.LoadAssetAtPath(assetPath, typeof(LevelData)) as LevelData;

        levelManager.AddLevel(newLevelData);
#endif
    }
}

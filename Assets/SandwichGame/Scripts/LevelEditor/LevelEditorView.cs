using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelEditorView : MonoBehaviour
{
    [SerializeField] LevelEditor levelEditor;

    [SerializeField] TMP_InputField gridX, gridY, levelName;
    [SerializeField] TMP_Dropdown prefab;
    [SerializeField] Button generateButton, saveButton;
    [SerializeField] GridLayoutGroup layoutGroup;

    List<TMP_Dropdown> grid = new List<TMP_Dropdown>();
    List<string> itemData;

    // Start is called before the first frame update
    void Start()
    {
        generateButton.onClick.AddListener(GenerateGrid);
        saveButton.onClick.AddListener(SaveLevel);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void GenerateGrid()
    {
        int width = int.Parse(gridX.text);
        int height = int.Parse(gridY.text);

        if (width > 5)
            width = 5;

        layoutGroup.constraintCount = width;

        itemData = levelEditor.GetItems();

        for (int i = 0; i < width * height; i++)
        {
            if(i >= grid.Count)
            {
                GameObject newObj = Instantiate(prefab.gameObject, layoutGroup.transform);

                newObj.SetActive(true);
                TMP_Dropdown dropdown = newObj.GetComponent<TMP_Dropdown>();

                dropdown.options.Clear();
                dropdown.AddOptions(itemData);

                grid.Add(dropdown);
            }
        }
    }

    List<LevelData.ItemGrid> ConvertToGrid()
    {
        List<LevelData.ItemGrid> itemGrid = new List<LevelData.ItemGrid>();
        int x = 0;
        int y = 0;

        int width = int.Parse(gridX.text);
        int height = int.Parse(gridY.text);

        for (int i = 0; i < grid.Count; i++)
        {
            if (i % width == 0)
            {
                y = 0;
                x++;
            }

            if(grid[i].value > 0)
            {
                Debug.Log("[" + x + ", " + y + "] : " + itemData[grid[i].value]);
                LevelData.ItemGrid newItem = new LevelData.ItemGrid();
                newItem.Item = new Item(); 
                newItem.Item.Name = itemData[grid[i].value];

                newItem.GridPosition = new Vector2Int(x, y);

                itemGrid.Add(newItem);
            }

            y++;
        }
        return itemGrid;
    }

    void SaveLevel()
    {
        levelEditor.SaveLevel(levelName.text, ConvertToGrid());
    }
}

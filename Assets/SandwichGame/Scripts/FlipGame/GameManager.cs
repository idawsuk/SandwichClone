using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class GameManager : MonoBehaviour
{
    [SerializeField] PlayerInput playerInput;

    [SerializeField] Tile tilePrefab;
    [SerializeField] LevelData TestLevel;

    const string BREAD_ITEM = "Bread";

    AsyncOperationHandle<GameObject> loadObjectHandler;
    Tile[,] grid;
    Tile selectedTile;

    // Start is called before the first frame update
    void Start()
    {
        LevelData data = GetLevelData();
        CreateGrid(data);
        StartCoroutine(LoadObjects(data));

        playerInput.OnTouchBegin += StartInput;
        playerInput.OnTouchEnd += EndInput;
    }

    LevelData GetLevelData()
    {
        return TestLevel;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        Addressables.Release(loadObjectHandler);
    }

    void CreateGrid(LevelData levelData)
    {
        int maxX = int.MinValue;
        int maxY = int.MinValue;

        for (int i = 0; i < levelData.LevelItems.Count; i++)
        {
            Vector2Int gridPosition = levelData.LevelItems[i].GridPosition;

            if (gridPosition.x >= maxX)
                maxX = gridPosition.x;

            if (gridPosition.y >= maxY)
                maxY = gridPosition.y;
        }

        maxX++;
        maxY++;

        grid = new Tile[maxX, maxY];
        for (int i = 0; i < maxX; i++)
        {
            for (int j = 0; j < maxY; j++)
            {
                GameObject newTileObject = Instantiate(tilePrefab.gameObject);
                newTileObject.SetActive(true);

                Tile newTile = newTileObject.GetComponent<Tile>();
                newTile.Grid = new Vector2Int(i, j);

                grid[i, j] = newTile;
                newTile.transform.position = new Vector3(i, 0, j);
            }
        }
    }

    IEnumerator LoadObjects(LevelData levelData)
    {
        for (int i = 0; i < levelData.LevelItems.Count; i++)
        {
            loadObjectHandler = Addressables.LoadAssetAsync<GameObject>(levelData.LevelItems[i].Item.Name);
            yield return loadObjectHandler;

            int x = levelData.LevelItems[i].GridPosition.x;
            int y = levelData.LevelItems[i].GridPosition.y;

            Vector3 position = new Vector3(x, 0, y);
            GameObject newObject = Instantiate(loadObjectHandler.Result, position, Quaternion.Euler(Vector3.zero));
            TileItem tileItem = newObject.GetComponent<TileItem>();

            grid[x, y].ItemStack.Add(tileItem);
        }
    }

    void StartInput(Tile tile)
    {
        selectedTile = tile;
    }

    void EndInput(Vector2 direction)
    {
        Vector2Int directionInt = new Vector2Int((int)direction.x, (int)direction.y);
        int gridX = (int)selectedTile.transform.position.x;
        int gridY = (int)selectedTile.transform.position.y;

        Vector2Int targetGrid = selectedTile.Grid + directionInt;
        if (targetGrid.x < 0 || targetGrid.y < 0) // out of bounds
            return;

        if (grid[targetGrid.x, targetGrid.y].ItemStack.Count == 0) // empty tile
            return;

        MoveTile(grid[targetGrid.x, targetGrid.y]);
    }

    void MoveTile(Tile targetTile)
    {
        List<TileItem> items = selectedTile.ItemStack;

        selectedTile.ItemStack.Reverse();
        //selectedTile.SortItem();
        //items[0].transform.position = new Vector3(targetTile.transform.position.x, targetTile.Offset * targetTile.ItemStack.Count, targetTile.transform.position.z);

        targetTile.ItemStack.AddRange(items);
        //targetTile.SortItem();

        for (int i = 0; i < targetTile.ItemStack.Count; i++)
        {
            Transform itemTransform = targetTile.ItemStack[i].transform;

            itemTransform.localPosition = new Vector3(targetTile.Grid.x, i * targetTile.Offset, targetTile.Grid.y);
        }

        selectedTile.ItemStack.Clear();
    }
}

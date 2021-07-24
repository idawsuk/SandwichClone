using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class GameManager : MonoBehaviour
{
    #region direct reference
    [SerializeField] PlayerInput playerInput;
    [SerializeField] LevelManager levelManager;

    [SerializeField] Tile tilePrefab;
    [SerializeField] LevelData TestLevel;

    [SerializeField] MoveHistoryManager moveHistoryManager;

    [SerializeField] CameraController cameraController;
    #endregion

    const string BREAD_ITEM = "Bread";

    #region private variables
    AsyncOperationHandle<GameObject> loadObjectHandler;
    Tile[,] grid;
    Tile selectedTile;

    int itemCount;

    int currentLevel;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        LevelData data = GetLevelData();

        if(data != null)
        {
            CreateGrid(data);
            StartCoroutine(LoadObjects(data));

            itemCount = data.LevelItems.Count;

            FocusCamera();

            playerInput.OnTouchBegin += StartInput;
            playerInput.OnTouchEnd += EndInput;
        }
    }

    LevelData GetLevelData()
    {
        currentLevel = PlayerPrefs.GetInt("currentLevel", 0);
        return levelManager.GetCurrentLevel(currentLevel);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        Addressables.Release(loadObjectHandler);
    }

    void FocusCamera()
    {
        List<Transform> tiles = new List<Transform>();

        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                Debug.Log((x * grid.GetLength(0)) + y + " : " + grid[x, y].transform.name, grid[x, y].gameObject);
                tiles.Add(grid[x, y].transform);
            }
        }
        cameraController.Focus(tiles.ToArray());
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
        if (selectedTile.ItemStack.Count == 0)
            return;

        Vector2Int directionInt = new Vector2Int((int)direction.x, (int)direction.y);

        Vector2Int targetGrid = selectedTile.Grid + directionInt;
        if (targetGrid.x < 0 || targetGrid.y < 0 || targetGrid.x > grid.GetLength(0) - 1 || targetGrid.y > grid.GetLength(1) - 1) // out of bounds
            return;

        if (grid[targetGrid.x, targetGrid.y].ItemStack.Count == 0) // empty tile
            return;

        MoveItems(grid[targetGrid.x, targetGrid.y]);
    }

    void MoveItems(Tile targetTile)
    {
        playerInput.enabled = false;

        moveHistoryManager.AddMove(selectedTile, targetTile, selectedTile.ItemStack.Count);

        selectedTile.MoveItem(targetTile, () =>
        {
            selectedTile = null;

            playerInput.enabled = true;

            if(IsCompleted(targetTile))
            {
                LoadNextLevel();
            }
        });
    }

    bool IsCompleted(Tile tile)
    {
        if(tile.ItemStack.Count == itemCount)
        {
            if (tile.ItemStack[0].Item.Name.Equals(BREAD_ITEM) && tile.ItemStack[itemCount - 1].Item.Name.Equals(BREAD_ITEM))
                return true;
        }

        return false;
    }

    void LoadNextLevel()
    {
        PlayerPrefs.SetInt("currentLevel", currentLevel + 1);

        LevelData newData = levelManager.GetCurrentLevel(currentLevel + 1);

        if(newData != null)
            UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
    }

    [ContextMenu("Reset Level")]
    public void ResetLevel()
    {
        int undoIndex = moveHistoryManager.moveHistory.Count - 1;

        if (undoIndex < 0)
            return;

        MoveHistory history = moveHistoryManager.moveHistory[undoIndex];
        moveHistoryManager.moveHistory.RemoveAt(undoIndex);

        history.CurrentTile.Undo(history.PreviousTile, history.ItemCount, () =>
        {
            ResetLevel();
        });
    }

    [ContextMenu("Reset Progress")]
    void ResetProgress()
    {
        PlayerPrefs.SetInt("currentLevel", 0);
    }
}

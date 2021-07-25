using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class GameManager : MonoBehaviour
{
    #region direct reference
    [Header("Input")]
    [SerializeField] FlipGameInput flipGameInput;
    [SerializeField] BaseInput munchGameInput;

    [Header("Game")]
    [SerializeField] LevelManager levelManager;
    [SerializeField] Tile tilePrefab;
    [SerializeField] LevelData TestLevel;
    [SerializeField] MoveHistoryManager moveHistoryManager;
    [SerializeField] CameraController cameraController;
    [SerializeField] MeshSlicer meshSlicer;

    [Header("Boundaries")]
    [SerializeField] Transform top;
    [SerializeField] Transform bottom;
    [SerializeField] Transform left;
    [SerializeField] Transform right;
    #endregion

    const string BREAD_ITEM = "Bread";
    const int BITE_REQUIRED = 3;

    #region private variables
    AsyncOperationHandle<GameObject> loadObjectHandler;
    Tile[,] grid;
    Tile selectedTile;

    int itemCount;
    int biteCount;
    int currentLevel;

    Tile finalTile;

    GameState currentState = GameState.None;
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

            flipGameInput.OnTouchBegin += FlipGameInputStarted;
            flipGameInput.OnTouchEnd += FlipGameInputEnded;

            munchGameInput.OnTouchStarted += Bite;

            ChangeState(GameState.Main);
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
        
    }

    void ChangeState(GameState state)
    {
        if (currentState == state)
            return;

        currentState = state;

        switch (currentState)
        {
            case GameState.Main:
                flipGameInput.enabled = true;
                munchGameInput.enabled = false;
                break;
            case GameState.Bite:
                flipGameInput.enabled = false;
                munchGameInput.enabled = true;
                break;
            case GameState.Finished:
                flipGameInput.enabled = false;
                munchGameInput.enabled = false;

                LoadNextLevel();
                break;
            default:
                break;
        }
    }

    void FocusCamera()
    {
        float gridX = grid.GetLength(0) - 1;
        float gridY = grid.GetLength(1) - 1;

        top.transform.position = new Vector3(gridX / 2f, 0, gridY + 1);
        bottom.transform.position = new Vector3(gridX / 2f, 0, -1);
        left.transform.position = new Vector3(-1, 0, 0);
        right.transform.position = new Vector3(gridX + 1, 0, 0);

        cameraController.Focus(top, bottom, left, right);
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

    void FlipGameInputStarted(Tile tile)
    {
        selectedTile = tile;
    }

    void FlipGameInputEnded(Vector2 direction)
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
        flipGameInput.enabled = false;
        moveHistoryManager.AddMove(selectedTile, targetTile, selectedTile.ItemStack.Count);

        selectedTile.MoveItem(targetTile, () =>
        {
            selectedTile = null;
            flipGameInput.enabled = true;

            if(IsCompleted(targetTile))
            {
                MainGameComplete(targetTile);
            }
        });
    }

    void MainGameComplete(Tile finalTile)
    {
        this.finalTile = finalTile;
        this.finalTile.SortItem();
        this.finalTile.ResetRotation();
        cameraController.Focus(this.finalTile.transform, this.finalTile.ItemStack.Count, .2f);

        MeshRenderer[] objects = finalTile.ItemStack[0].GetComponentsInChildren<MeshRenderer>();

        GameObject[] gameObjects = new GameObject[objects.Length];
        for (int i = 0; i < objects.Length; i++)
        {
            gameObjects[i] = objects[i].gameObject;
        }

        meshSlicer.SetObjects(gameObjects);

        ChangeState(GameState.Bite);
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

    void Bite()
    {
        Debug.Log("BITE " + biteCount);
        meshSlicer.transform.position = finalTile.transform.position;

        meshSlicer.Slice(biteCount);

        biteCount++;

        if(biteCount >= BITE_REQUIRED)
        {
            ChangeState(GameState.Finished);
        }
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

    enum GameState
    {
        None, Main, Bite, Finished
    }
}

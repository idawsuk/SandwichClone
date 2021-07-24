using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveHistoryManager : MonoBehaviour
{
    public List<MoveHistory> moveHistory = new List<MoveHistory>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void AddMove(Tile prevTile, Tile currentTile, int itemCount)
    {
        moveHistory.Add(new MoveHistory { PreviousTile = prevTile, CurrentTile = currentTile, ItemCount = itemCount });
    }
}

[System.Serializable]
public class MoveHistory
{
    public Tile PreviousTile;
    public Tile CurrentTile;
    public int ItemCount;
}

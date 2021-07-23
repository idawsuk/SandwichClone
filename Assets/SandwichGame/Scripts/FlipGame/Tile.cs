using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public List<TileItem> ItemStack = new List<TileItem>();
    public Vector2Int Grid;
    public float Offset = .2f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SortItem()
    {
        if(ItemStack.Count >= 2)
        for (int i = 1; i < ItemStack.Count; i++)
        {
            ItemStack[i].transform.SetParent(ItemStack[0].transform);
        }
    }

    private void OnDrawGizmos()
    {
        //Gizmos.DrawCube(this.transform.position, new Vector3(.9f, .2f, .9f));
    }
}

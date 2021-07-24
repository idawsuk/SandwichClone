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

    public void MoveItem(Tile target, System.Action onComplete)
    {
        ItemStack[0].Move(new Vector3(target.transform.position.x, (ItemStack.Count - 1 + target.ItemStack.Count) * target.Offset, target.transform.position.z), onComplete);
    }

    public void Undo(Tile target, int itemCount, System.Action onComplete)
    {
        if(itemCount > 1) {
            for (int i = 1; i < ItemStack.Count - 1; i++)
            {
                ItemStack[i].transform.SetParent(ItemStack[ItemStack.Count - 1].transform);
            }

            TileItem item = ItemStack[ItemStack.Count - 1];

            item.Move(new Vector3(target.transform.position.x, 0, target.transform.position.z), () =>
            {
                if (ItemStack.Count > 1)
                {
                    List<TileItem> items = new List<TileItem>();
                    for (int i = 1; i < ItemStack.Count; i++)
                    {
                        items.Add(ItemStack[i]);
                    }
                    ItemStack.RemoveRange(1, ItemStack.Count - 1);

                    target.ItemStack.AddRange(items);

                    target.SortItem();
                }

                onComplete();
            }, true);

        }else if(itemCount == 1)
        {
            TileItem item = ItemStack[ItemStack.Count - 1];
            item.transform.SetParent(null);

            item.Move(new Vector3(target.transform.position.x, 0, target.transform.position.z), () =>
            {
                ItemStack.Remove(item);

                target.ItemStack.Add(item);

                onComplete();
            }, true);

        }

    }

    private void OnDrawGizmos()
    {
        //Gizmos.DrawCube(this.transform.position, new Vector3(.9f, .2f, .9f));
    }
}

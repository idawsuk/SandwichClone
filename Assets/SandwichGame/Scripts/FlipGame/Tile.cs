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
        if (ItemStack.Count >= 2)
            for (int i = 1; i < ItemStack.Count; i++)
            {
                ItemStack[i].transform.SetParent(ItemStack[0].transform);
            }
    }

    public void MoveItem(Tile target, System.Action onComplete)
    {
        SortItem();

        ItemStack[0].Move(new Vector3(target.transform.position.x, (ItemStack.Count - 1 + target.ItemStack.Count) * target.Offset, target.transform.position.z),
            () =>
            {
                ItemStack.Reverse();
                target.ItemStack.AddRange(ItemStack);
                ItemStack.Clear();

                onComplete();
            }
        );
    }

    public void FinishAnimation(System.Action onComplete)
    {
        for (int i = 0; i < ItemStack.Count; i++)
        {
            if(i == 0)
            {
                ItemStack[i].Rotate(onComplete);
            } else
            {
                ItemStack[i].MiniJump(i + 1);
            }
        }
    }

    public void BiteAnimation()
    {
        ItemStack[0].PunchScale();
    }

    public void HideItems()
    {
        for (int i = 0; i < ItemStack.Count; i++)
        {
            ItemStack[0].gameObject.SetActive(false);
        }
    }

    public void ResetRotation()
    {
        for (int i = 0; i < ItemStack.Count; i++)
        {
            ItemStack[i].transform.localEulerAngles = Vector3.zero;
        }
    }

    public void Undo(Tile target, int itemCount, System.Action onComplete)
    {
        if (itemCount > 1)
        {
            TileItem item = ItemStack[ItemStack.Count - 1];

            for (int i = ItemStack.Count - itemCount; i < ItemStack.Count - 1; i++)
            {
                ItemStack[i].transform.SetParent(item.transform);
            }

            item.transform.SetParent(null);

            item.Move(new Vector3(target.transform.position.x, 0, target.transform.position.z), () =>
            {
                List<TileItem> items = new List<TileItem>();
                for (int i = ItemStack.Count - itemCount; i < ItemStack.Count; i++)
                {
                    items.Add(ItemStack[i]);
                }
                ItemStack.RemoveRange(ItemStack.Count - itemCount, itemCount);

                target.ItemStack.AddRange(items);
                target.ItemStack.Reverse();
                target.SortItem();

                onComplete();
            }, true);

        }
        else if (itemCount == 1)
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

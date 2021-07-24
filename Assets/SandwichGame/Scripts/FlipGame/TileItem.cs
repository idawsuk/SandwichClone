using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TileItem : MonoBehaviour
{
    public Item Item;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Move(Vector3 target, System.Action onComplete)
    {
        Vector3 diff = transform.position - target;
        Debug.Log(diff);
        Vector3 direction = Vector3.zero;
        if (diff.x > 0)
            direction = Vector3.forward;
        else if (diff.x < 0)
            direction = Vector3.back;
        else if (diff.z < 0)
            direction = Vector3.right;
        else
            direction = Vector3.left;

        Vector3 rotation = transform.localEulerAngles;
        transform.DORotate(rotation + (direction * 180), .5f, RotateMode.FastBeyond360);
        transform.DOJump(target, .5f, 1, .5f).OnComplete(() =>
        {
            onComplete?.Invoke();
        });
    }
}

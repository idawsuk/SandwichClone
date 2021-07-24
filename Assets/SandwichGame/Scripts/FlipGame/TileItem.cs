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

    public void Move(Vector3 target, System.Action onComplete, bool isReverse = false)
    {
        Vector3 diff = transform.position - target;

        Vector3 direction = Vector3.zero;
        if (diff.x > 0)
            direction = Vector3.forward;
        else if (diff.x < 0)
            direction = Vector3.back;
        else if (diff.z < 0)
            direction = Vector3.right;
        else
            direction = Vector3.left;

        if (isReverse)
            direction = -direction;

        Vector3 targetRotation = transform.localEulerAngles + (direction * 180);
        if (isReverse)
            targetRotation = Vector3.zero;
        transform.DORotate(targetRotation, .5f, RotateMode.FastBeyond360).SetEase(Ease.InOutQuad);
        transform.DOJump(target, .5f, 1, .5f).SetEase(Ease.InOutQuad).OnComplete(() =>
        {
            onComplete?.Invoke();
        });
    }
}

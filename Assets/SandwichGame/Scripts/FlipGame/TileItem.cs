using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TileItem : MonoBehaviour
{
    public Item Item;

    //finish animation
    float strength = 10f;
    int vibrato = 4;
    float randomness = 10f;

    // Start is called before the first frame update
    void Start()
    {
        Vector3 startPosition = transform.position;

        transform.position = new Vector3(startPosition.x, 4, startPosition.z);
        transform.DOMove(startPosition, 1).SetDelay(Random.Range(0f, .5f)).SetEase(Ease.InOutQuart);
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

    public void MiniJump(int index)
    {
        transform.DOJump(this.transform.position, .2f * index, 1, 1f).SetEase(Ease.InOutSine);
        transform.DOShakeRotation(.8f, strength, vibrato, randomness).SetEase(Ease.OutQuad).SetDelay(.2f);
    }

    public void Rotate(System.Action onComplete = null)
    {
        transform.DORotate(new Vector3(0, 315, 0), 1f, RotateMode.FastBeyond360).SetEase(Ease.InOutSine).OnComplete(() =>
        {
            onComplete?.Invoke();
        });
    }

    public void PunchScale()
    {
        transform.DOPunchScale(Vector3.one * .2f, .3f, 4).SetEase(Ease.OutElastic);
    }
}

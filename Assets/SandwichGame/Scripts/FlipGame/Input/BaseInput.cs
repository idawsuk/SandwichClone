using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseInput : MonoBehaviour
{
    [SerializeField] protected Camera mainCamera;

    public delegate void TouchEvent();
    public TouchEvent OnTouchStarted;
    public TouchEvent OnTouchEnded;


    // Start is called before the first frame update
    protected virtual void Start()
    {
        if (!mainCamera)
            mainCamera = Camera.main;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (Application.isEditor)
        {
            if (Input.GetMouseButtonDown(0))
            {
                TouchStarted(Input.mousePosition);
            }

            if (Input.GetMouseButtonUp(0))
            {
                TouchEnded(Input.mousePosition);
            }

            return;
        }


        if (Input.touchCount == 1)
        {
            if (Input.GetTouch(0).phase.Equals(TouchPhase.Began))
            {
                TouchStarted(Input.GetTouch(0).position);
            }

            if (Input.GetTouch(0).phase.Equals(TouchPhase.Ended))
            {
                TouchEnded(Input.GetTouch(0).position);
            }
        }
    }

    protected virtual void TouchStarted(Vector2 position)
    {
        OnTouchStarted?.Invoke();
    }

    protected virtual void TouchEnded(Vector2 position)
    {
        OnTouchEnded?.Invoke();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] LayerMask layerMask;
    [SerializeField] Camera mainCamera;

    public delegate void TouchBegin(Tile tile);
    public TouchBegin OnTouchBegin;
    public delegate void TouchEnd(Vector2 direction);
    public TouchEnd OnTouchEnd;

    Vector2 touchStartPosition;
    Vector2 touchEndPosition;
    float moveThreshold;

    bool isTileFound = false;

    // Start is called before the first frame update
    void Start()
    {
        if (!mainCamera)
            mainCamera = Camera.main;

        moveThreshold = Screen.width / 10f; // 10% of screen width
    }

    // Update is called once per frame
    void Update()
    {
        if(Application.isEditor)
        {
            if(Input.GetMouseButtonDown(0))
            {
                TouchStarted(Input.mousePosition);
            }

            if(Input.GetMouseButtonUp(0))
            {
                TouchEnded(Input.mousePosition);
            }

            return;
        }


        if(Input.touchCount == 1)
        {
            if (Input.GetTouch(0).phase.Equals(TouchPhase.Began))
            {
                TouchStarted(Input.GetTouch(0).position);
            }

            if(Input.GetTouch(0).phase.Equals(TouchPhase.Ended))
            {
                TouchEnded(Input.GetTouch(0).position);
            }
        }
    }

    void TouchStarted(Vector2 position)
    {
        touchStartPosition = position;

        Ray ray = Camera.main.ScreenPointToRay(touchStartPosition);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            Tile tile = hit.collider.gameObject.GetComponent<Tile>();

            if (tile)
            {
                OnTouchBegin?.Invoke(tile);
                isTileFound = true;
            } else
            {
                isTileFound = false;
            }
        } else
        {
            isTileFound = false;
        }
    }

    void TouchEnded(Vector2 position)
    {
        if (!isTileFound)
            return;

        touchEndPosition = position;
        Vector2 delta = touchEndPosition - touchStartPosition;

        if(Vector2.Distance(touchStartPosition, touchEndPosition) > moveThreshold)
        {
            Vector2 direction = Vector2.zero;
            if(Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
            {
                if (delta.x > 0)
                    direction = Vector2.right;
                else
                    direction = Vector3.left;
            } else
            {
                if (delta.y > 0)
                    direction = Vector2.up;
                else
                    direction = Vector2.down;
            }

            OnTouchEnd?.Invoke(direction);
        }

        isTileFound = false;
    }
}

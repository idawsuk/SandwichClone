using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Camera mainCamera;

    bool wasAnyPointOutside = false;
    Transform[] targets;

    // Start is called before the first frame update
    void Start()
    {
        if (!mainCamera)
            mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Focus(Transform[] targets)
    {
        this.targets = targets;
        wasAnyPointOutside = true;

        Vector3 averageCenter = Vector3.zero;

        float widthOffset = Screen.width / 5f;
        float heightOffset = Screen.height / 2.5f;

        foreach (Transform t in targets)
        {
            averageCenter += t.position;
            Vector3 v = mainCamera.WorldToScreenPoint(t.position);
            Debug.Log("Point at: " + v);
            if (v.x < 0 + widthOffset || v.y < 0 + heightOffset || v.z < 0 || v.x > Screen.width - widthOffset || v.y > Screen.height - heightOffset)
            {
                wasAnyPointOutside = true;
            }
        }
        Debug.Log("Center: " + averageCenter / targets.Length);
        mainCamera.transform.position = (averageCenter / targets.Length) + Vector3.up - mainCamera.transform.forward;
        mainCamera.transform.LookAt((averageCenter / targets.Length));
        while (wasAnyPointOutside)
        {
            wasAnyPointOutside = false;
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, mainCamera.transform.position - mainCamera.transform.forward, Time.deltaTime * 25f);
            foreach (Transform t in targets)
            {
                Vector3 v = mainCamera.WorldToScreenPoint(t.position);
                if (v.x < 0 + widthOffset || v.y < 0 + heightOffset || v.z < 0 || v.x > Screen.width - widthOffset || v.y > Screen.height - heightOffset)
                {
                    wasAnyPointOutside = true;
                }
            }
        }
    }

    public void Focus(Transform[,] tiles)
    {
        Vector3 averageCenter = Vector3.zero;
        for (int x = 0; x < tiles.GetLength(0); x++)
        {
            for (int y = 0; y < tiles.GetLength(1); y++)
            {
                averageCenter += tiles[x, y].position;
            }
        }


    }
}

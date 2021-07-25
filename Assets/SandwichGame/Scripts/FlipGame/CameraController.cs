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

    public void Focus(params Transform[] targets)
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
            if (v.x < 0 || v.y < 0 || v.z < 0 || v.x > Screen.width || v.y > Screen.height)
            {
                wasAnyPointOutside = true;
            }
        }
        
        mainCamera.transform.position = (averageCenter / targets.Length) + Vector3.up - mainCamera.transform.forward;
        mainCamera.transform.LookAt((averageCenter / targets.Length));
        while (wasAnyPointOutside)
        {
            wasAnyPointOutside = false;
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, mainCamera.transform.position - mainCamera.transform.forward, Time.deltaTime * 25f);
            foreach (Transform t in targets)
            {
                Vector3 v = mainCamera.WorldToScreenPoint(t.position);
                if (v.x < 0 || v.y < 0 || v.z < 0 || v.x > Screen.width || v.y > Screen.height)
                {
                    wasAnyPointOutside = true;
                }
            }
        }
    }
}

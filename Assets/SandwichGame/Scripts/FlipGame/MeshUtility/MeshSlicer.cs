using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EzySlice;

//https://github.com/DavidArayan/ezy-slice
public class MeshSlicer : MonoBehaviour
{
    [SerializeField] GameObject[] slicers;
    GameObject[] objectToSlice;

    GameObject[] slicedObjects;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetObjects(GameObject[] objects)
    {
        objectToSlice = objects;
        slicedObjects = new GameObject[objectToSlice.Length];
    }

    public bool Slice(int index)
    {
        if (index < slicers.Length)
        {
            for (int i = 0; i < objectToSlice.Length; i++)
            {
                SlicedHull hull = objectToSlice[i].Slice(slicers[index].transform.position, slicers[index].transform.up);

                if(hull != null)
                {
                    Material mat = objectToSlice[i].GetComponent<MeshRenderer>().material;

                    hull.CreateLowerHull(objectToSlice[i], mat).SetActive(false);
                    slicedObjects[i] = hull.CreateUpperHull(objectToSlice[i], mat);
                    slicedObjects[i].transform.position = objectToSlice[i].transform.position;

                    objectToSlice[i].SetActive(false);

                    objectToSlice[i] = slicedObjects[i];
                }
            }

            return true;
        }

        return false;
    }

    
}

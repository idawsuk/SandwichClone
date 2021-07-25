using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    [SerializeField] GameObject prefab;
    [SerializeField] int startingPool = 3;
    [SerializeField] GameObject confetti;

    List<ParticleSystem> objectPool = new List<ParticleSystem>();

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < startingPool; i++)
        {
            GameObject newObject = Instantiate(prefab);
            objectPool.Add(newObject.GetComponent<ParticleSystem>());
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public ParticleSystem Spawn()
    {
        for (int i = 0; i < objectPool.Count; i++)
        {
            if (!objectPool[i].isPlaying)
            {
                objectPool[i].Play();
                return objectPool[i];
            }
        }

        GameObject newObject = Instantiate(prefab);
        ParticleSystem particleSystem = newObject.GetComponent<ParticleSystem>();
        objectPool.Add(particleSystem);

        return particleSystem;
    }

    public void SpawnConfetti(Vector3 position)
    {
        confetti.transform.position = position;
        confetti.SetActive(true);
    }
}

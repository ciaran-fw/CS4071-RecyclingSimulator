using UnityEngine;
using System.Collections.Generic;

public class TrashSpawner : MonoBehaviour
{
    [SerializeField] private BoxCollider spawnArea;

    private List<GameObject> spawnedTrash = new List<GameObject>();

    private bool spawningTrash = false;

    [Range(0f, 10f)]
    [SerializeField] private float spawnDelay = 0.5f;
    private float spawnTimer = 0f;

    [Range(1, 100)]
    [SerializeField] private int spawnAmount = 5;

    [Header("Trash Prefabs")]
    [SerializeField] private GameObject[] plasticTrash;
    [SerializeField] private GameObject[] cardboardTrash;
    [SerializeField] private GameObject[] metalTrash;
    [SerializeField] private GameObject[] otherTrash;

    [Header("Spawn Weights")]
    [SerializeField] private float plasticWeight = 20f;
    [SerializeField] private float cardboardWeight = 20f;
    [SerializeField] private float metalWeight = 20f;
    [SerializeField] private float otherWeight = 40f;

    private void Update()
    {
        if (!spawningTrash) return;

        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnDelay)
        {
            for (int i = 1; i <= spawnAmount; i++) SpawnTrash();
            
            spawnTimer = 0f;
        }
    }

    public void SpawnTrash()
    {
        Bounds bounds = spawnArea.bounds;

        float x = Random.Range(bounds.min.x, bounds.max.x);
        float z = Random.Range(bounds.min.z, bounds.max.z);

        Vector3 spawnPosition = new Vector3(x, bounds.max.y, z);

        GameObject trash = GetRandomTrash();

        Quaternion rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);

        GameObject newTrash = Instantiate(trash, spawnPosition, rotation);

        spawnedTrash.Add(newTrash);
    }

    private GameObject GetRandomTrash()
    {
        float totalWeight = cardboardWeight + plasticWeight + metalWeight + otherWeight;
        float randomValue = Random.Range(0f, totalWeight);

        if (randomValue < plasticWeight)
        {
            return GetRandomFromArray(plasticTrash);
        }

        randomValue -= plasticWeight;

        if (randomValue < cardboardWeight)
        {
            return GetRandomFromArray(cardboardTrash);
        }

        randomValue -= cardboardWeight;

        if (randomValue < metalWeight)
        {
            return GetRandomFromArray(metalTrash);
        }

        return GetRandomFromArray(otherTrash);
    }

    private GameObject GetRandomFromArray(GameObject[] array)
    {
        return array[Random.Range(0, array.Length)];
    }

    public void StartSpawning()
    {
        spawningTrash = true;
    }

    public void ResetSpawner()
    {
        spawningTrash = false;
        spawnTimer = 0f;
    }

    public void RemoveAllTrash()
    {
        foreach (GameObject trash in spawnedTrash)
        {
            if (trash != null)
                Destroy(trash);
        }

        spawnedTrash.Clear();
    }
}

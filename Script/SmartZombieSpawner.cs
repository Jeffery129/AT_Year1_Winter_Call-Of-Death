using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SmartZombieSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject zombiePrefab;
    public int maxZombies = 10;
    public float spawnRadius = 30f;
    public Transform[] spawnCenters;
    public float minDistanceFromPlayer = 10f;
    public Transform player;

    [Header("NavMesh Sampling")]
    public float sampleDistance = 2f;

    [Header("Spawn Cooldown")]
    public float spawnCooldown = 3f;
    private float lastSpawnTime = 0f;

    private List<GameObject> spawnedZombies = new List<GameObject>();

    private void Update()
    {
        //////////////////////////////////////////////////////////
        //
        //for (int i = spawnedZombies.Count - 1; i >= 0; i--)
        //{
        //    if (spawnedZombies[i] == null)
        //    {
        //        spawnedZombies.RemoveAt(i);
        //    }
        //}
        //
        //  ªªªªªªªªªªªªªªªªªª
        spawnedZombies.RemoveAll(z => z == null);
        //////////////////////////////////////////////////////////

        if (spawnedZombies.Count < maxZombies && Time.time - lastSpawnTime >= spawnCooldown)
        {
            SpawnZombie();
            lastSpawnTime = Time.time;
        }
    }

    private void SpawnZombie()
    {
        Transform spawnCenter = spawnCenters[Random.Range(0, spawnCenters.Length)];

        Vector3 randomPos = spawnCenter.position + new Vector3(
            Random.Range(-spawnRadius, spawnRadius),
            0,
            Random.Range(-spawnRadius, spawnRadius)
        );

        if (Vector3.Distance(randomPos, player.position) < minDistanceFromPlayer)
        {
            return;
        }

        if (NavMesh.SamplePosition(randomPos, out NavMeshHit hit, sampleDistance, NavMesh.AllAreas))
        {
            GameObject zombie = Instantiate(zombiePrefab, hit.position, Quaternion.identity);
            spawnedZombies.Add(zombie);
        }
        else
        {
            Debug.Log("No valid NavMesh found for zombie spawn.");
        }
    }
}

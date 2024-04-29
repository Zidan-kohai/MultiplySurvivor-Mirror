using Mirror;
using UnityEngine;

public class EnemySpawner : NetworkBehaviour
{
    public GameObject enemyPrefab; 
    public float spawnInterval = 3f; 
    private int currentEnemies = 0; 

    private void Start()
    {
        if (isServer)
        {
            InvokeRepeating(nameof(SpawnEnemy), 0f, spawnInterval);
        }
    }

    private void SpawnEnemy()
    {
        GameObject enemy = Instantiate(enemyPrefab, transform.position, Quaternion.identity);
        NetworkServer.Spawn(enemy);
        currentEnemies++;
    }

}

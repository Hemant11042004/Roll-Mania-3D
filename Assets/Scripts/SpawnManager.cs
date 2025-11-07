using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnManager : MonoBehaviour
{
    [Header("Enemy Settings")]
    public GameObject[] enemyPrefab;
    public float spawnRange = 11.0f;
    public int waveNumber = 1;
    public int maxEnemies = 10;

    [Header("Powerup Settings")]
    public GameObject[] powerupPrefabs;
    public float powerupSpawnDelay = 5f;
    public float powerupCheckInterval = 2f;
    public int maxPowerups = 3;

    private int enemyCount;
    private List<GameObject> activePowerups = new List<GameObject>();
    private bool gameInitialized = false;

    void Start()
    {
        StartCoroutine(WaitForGameStart());
    }

    IEnumerator WaitForGameStart()
    {
        // Wait until GameManager says the game has started
        yield return new WaitUntil(() => GameManager.Instance != null && GameManager.Instance.gameStarted);

        // Start the first wave
        SpawnEnemyWave(waveNumber);
        StartCoroutine(SpawnPowerupRoutine());
        gameInitialized = true;
    }

    void Update()
    {
        // Ensure GameManager exists before accessing it
        if (GameManager.Instance == null)
            return;

        // Skip updating if game hasn't started, is paused, or is over
        if (!gameInitialized || !GameManager.Instance.gameStarted || GameManager.Instance.isPaused || GameManager.Instance.gameOver)
            return;

        // Count active enemies in the scene
        enemyCount = FindObjectsByType<Enemy>(FindObjectsSortMode.None).Length;

        // When all enemies are cleared, go to the next level
        if (enemyCount == 0)
        {
            waveNumber++;

            // Limit total enemies to 10 maximum
            int enemiesToSpawn = Mathf.Min(waveNumber, maxEnemies);

            SpawnEnemyWave(enemiesToSpawn);

            // Update Level UI
            GameManager.Instance.UpdateLevelText(waveNumber);
        }
    }


    void SpawnEnemyWave(int enemiesToSpawn)
    {
        Debug.Log($"Spawning {enemiesToSpawn} enemies for wave {waveNumber}");

        for (int i = 0; i < enemiesToSpawn; i++)
        {
            int randomEnemy = Random.Range(0, enemyPrefab.Length);
            Instantiate(
                enemyPrefab[randomEnemy],
                GenerateSpawnPosition(),
                enemyPrefab[randomEnemy].transform.rotation
            );
        }
    }

    private IEnumerator SpawnPowerupRoutine()
    {
        yield return new WaitForSeconds(powerupSpawnDelay);

        while (true)
        {
            if (!GameManager.Instance.gameStarted)
            {
                yield return null;
                continue;
            }

            activePowerups.RemoveAll(p => p == null);

            if (activePowerups.Count < maxPowerups && powerupPrefabs.Length > 0)
            {
                int randomPowerup = Random.Range(0, powerupPrefabs.Length);
                GameObject newPowerup = Instantiate(
                    powerupPrefabs[randomPowerup],
                    GenerateSpawnPosition(),
                    powerupPrefabs[randomPowerup].transform.rotation
                );

                activePowerups.Add(newPowerup);

                if (activePowerups.Count > maxPowerups)
                {
                    if (activePowerups[0] != null)
                        Destroy(activePowerups[0]);
                    activePowerups.RemoveAt(0);
                }
            }

            yield return new WaitForSeconds(powerupCheckInterval);
        }
    }

    private Vector3 GenerateSpawnPosition()
    {
        float spawnPosX = Random.Range(-spawnRange, spawnRange);
        float spawnPosZ = Random.Range(-spawnRange, spawnRange);
        return new Vector3(spawnPosX, -2, spawnPosZ);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    enum WavePhase
    {
        WaitingToSpawn,
        SpawningWave,
        Fighting,
        WaitingBetweenWaves,
        Finished
    }

    const int WaveCount = 3;
    const int EnemiesPerWave = 3;
    const float SpawnStaggerSeconds = 1.5f;

    [SerializeField] Transform[] spawnPoints;
    [SerializeField] EnemyAI enemyPrefab;
    [SerializeField] float delayBeforeFirstWave = 8f;
    [SerializeField] float delayBetweenWaves = 10f;
    [SerializeField] Player player;
    [SerializeField] float spawnSpread = 1.2f;

    readonly List<EnemyAI> spawnedEnemies = new List<EnemyAI>();
    WavePhase wavePhase = WavePhase.WaitingToSpawn;
    float waveTimer;
    int wavesSpawned;
    bool spawningEnabled = true;
    Coroutine spawnWaveRoutine;

    public int TotalWaves => WaveCount;
    public int WavesSpawned => wavesSpawned;
    public bool AllWavesFinished => wavePhase == WavePhase.Finished;

    void Awake()
    {
        if (enemyPrefab == null)
        {
            Debug.LogError("Enemy Spawner: enemyPrefab is not assigned.");
            enabled = false;
            return;
        }

        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            CollectSpawnPointsFromChildren();
        }
    }

    void Start()
    {
        if (player == null)
        {
            player = FindObjectOfType<Player>();
        }

        waveTimer = delayBeforeFirstWave;
        Debug.Log($"[Spawner] Ready: {WaveCount} waves x {EnemiesPerWave} enemies = {WaveCount * EnemiesPerWave} total.");
    }

    void Update()
    {
        spawnedEnemies.RemoveAll(enemy => enemy == null);

        if (!spawningEnabled || wavePhase == WavePhase.Finished || wavePhase == WavePhase.SpawningWave)
        {
            return;
        }

        switch (wavePhase)
        {
            case WavePhase.WaitingToSpawn:
            case WavePhase.WaitingBetweenWaves:
                waveTimer -= Time.deltaTime;
                if (waveTimer <= 0f)
                {
                    BeginWaveSpawn();
                }

                break;

            case WavePhase.Fighting:
                if (spawnedEnemies.Count == 0)
                {
                    if (wavesSpawned >= WaveCount)
                    {
                        wavePhase = WavePhase.Finished;
                        Debug.Log("[Spawner] All waves cleared.");
                    }
                    else
                    {
                        wavePhase = WavePhase.WaitingBetweenWaves;
                        waveTimer = delayBetweenWaves;
                        Debug.Log($"[Spawner] Wave {wavesSpawned} cleared. Next wave in {delayBetweenWaves}s.");
                    }
                }

                break;
        }
    }

    public void SetSpawningEnabled(bool enabled)
    {
        spawningEnabled = enabled;
        if (!enabled)
        {
            if (spawnWaveRoutine != null)
            {
                StopCoroutine(spawnWaveRoutine);
                spawnWaveRoutine = null;
            }

            wavePhase = WavePhase.Finished;
        }
    }

    void BeginWaveSpawn()
    {
        if (spawnWaveRoutine != null)
        {
            StopCoroutine(spawnWaveRoutine);
        }

        spawnWaveRoutine = StartCoroutine(SpawnWaveRoutine());
    }

    IEnumerator SpawnWaveRoutine()
    {
        wavePhase = WavePhase.SpawningWave;
        wavesSpawned++;
        Debug.Log($"[Spawner] Wave {wavesSpawned}/{WaveCount} started. Spawning {EnemiesPerWave} enemies.");

        for (int i = 0; i < EnemiesPerWave; i++)
        {
            SpawnEnemy(i);
            Debug.Log($"[Spawner] Spawned enemy {i + 1}/{EnemiesPerWave} for wave {wavesSpawned}. Active: {spawnedEnemies.Count}");

            if (i < EnemiesPerWave - 1)
            {
                yield return new WaitForSeconds(SpawnStaggerSeconds);
            }
        }

        wavePhase = WavePhase.Fighting;
        spawnWaveRoutine = null;
        Debug.Log($"[Spawner] Wave {wavesSpawned} spawn complete. Active enemies: {spawnedEnemies.Count}");
    }

    void SpawnEnemy(int waveSlot)
    {
        if (player == null)
        {
            player = FindObjectOfType<Player>();
        }

        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("[Spawner] No cover points configured.");
            return;
        }

        Transform coverSpot = spawnPoints[waveSlot % spawnPoints.Length];
        Vector3 spawnPosition = GetSpawnPositionAtDoor(waveSlot);

        EnemyAI enemy = Instantiate(enemyPrefab, spawnPosition, transform.rotation);
        enemy.name = $"Enemy_W{wavesSpawned}_S{waveSlot + 1}";
        enemy.Init(player, coverSpot);
        spawnedEnemies.Add(enemy);
    }

    Vector3 GetSpawnPositionAtDoor(int waveSlot)
    {
        Vector3 spawnPosition = transform.position;

        if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 4f, NavMesh.AllAreas))
        {
            spawnPosition = hit.position;
        }

        int slot = waveSlot % 3;
        float lateralOffset = (slot - 1) * spawnSpread;
        spawnPosition += transform.right * lateralOffset;

        if (NavMesh.SamplePosition(spawnPosition, out NavMeshHit offsetHit, 2f, NavMesh.AllAreas))
        {
            spawnPosition = offsetHit.position;
        }

        return spawnPosition;
    }

    void CollectSpawnPointsFromChildren()
    {
        int childCount = transform.childCount;
        if (childCount == 0)
        {
            return;
        }

        spawnPoints = new Transform[childCount];
        for (int i = 0; i < childCount; i++)
        {
            spawnPoints[i] = transform.GetChild(i);
        }
    }
}

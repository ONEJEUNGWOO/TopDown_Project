using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    private Coroutine waveRoutine;

    [SerializeField] private List<GameObject> enemyPrefabs;
    private Dictionary<string, GameObject> enemyPrefabDic;

    [SerializeField] List<Rect> spawnAreas;
    [SerializeField] private Color gizmoColor = new Color(1, 0, 0, .3f);
    private List<EnemyController> activeEnemies = new List<EnemyController>();

    private bool enemySpawnComplite;

    [SerializeField] private float timeBetweenSpawns = 0.2f;
    [SerializeField] private float timeBetweenWave = 1f;

    GameManager gameManager;

    [SerializeField] private List<GameObject> itemPrefabs;

    public void Init(GameManager gameManager)
    {
        this.gameManager = gameManager;

        enemyPrefabDic = new Dictionary<string, GameObject>();
        foreach (GameObject prefab in enemyPrefabs)
        {
            enemyPrefabDic[prefab.name] = prefab;
        }
    }

    public void StartWave(int waveCount)
    {
        if (waveCount <= 0)
        {
            gameManager.EndOfWave();
            return;
        }

        Debug.Log("소환");
        if (waveRoutine != null)
            StopCoroutine(waveRoutine);
        waveRoutine = StartCoroutine(SpawnWave(waveCount));
    }

    public void StopWave()
    {
        StopAllCoroutines();
    }

    private IEnumerator SpawnWave(int waveCount)
    {
        enemySpawnComplite = false;
        yield return new WaitForSeconds(timeBetweenWave);

        for (int i = 0; i < waveCount; i++)
        {
            yield return new WaitForSeconds(timeBetweenSpawns);
            SpawnRandomEnemy();
        }

        enemySpawnComplite = true;
    }

    private void SpawnRandomEnemy(string prerfabName = null)
    {
        if (enemyPrefabs.Count == 0 || spawnAreas.Count == 0)
        {
            Debug.Log("Enenmy prefaps 혹은 Spawn Areas가 설정되지 않았습니다");
            return;
        }

        GameObject randomprefap;
        if (prerfabName == null)
        {
            randomprefap = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];
        }
        else
        {
            randomprefap = enemyPrefabDic[prerfabName];
        }

        Rect randomArea = spawnAreas[Random.Range(0, spawnAreas.Count)];

        Vector2 randomPosition = new Vector2(
            Random.Range(randomArea.xMin, randomArea.xMax),
            Random.Range(randomArea.yMin, randomArea.yMax));

        GameObject spawnEnemy = Instantiate(randomprefap, new Vector3(randomPosition.x, randomPosition.y), Quaternion.identity);
        EnemyController enemyController = spawnEnemy.GetComponent<EnemyController>();
        enemyController.Init(this, gameManager.player.transform);

        activeEnemies.Add(enemyController);
    }

    private void OnDrawGizmosSelected()
    {
        if (spawnAreas == null) return;

        Gizmos.color = gizmoColor;
        foreach (var area in spawnAreas)
        {
            Vector3 center = new Vector3(area.x + area.width / 2, area.y + area.height / 2);
            Vector3 size = new Vector3(area.width, area.height);

            Gizmos.DrawCube(center, size);
        }
    }

    public void RemoveEnemyOnDeath(EnemyController enemy)
    {
        activeEnemies.Remove(enemy);
        CreateRandomItem(enemy.transform.position);
        if (enemySpawnComplite && activeEnemies.Count == 0)
            gameManager.EndOfWave();
    }

    public void CreateRandomItem(Vector3 position)
    {
        GameObject item = Instantiate(itemPrefabs[Random.Range(0, itemPrefabs.Count)], position, Quaternion.identity);
    }

    public void StartStage(StageInstance stageInstance)
    {
        if (waveRoutine != null)
            StopCoroutine(waveRoutine);

        waveRoutine = StartCoroutine(SpawnStart(stageInstance));
    }

    private IEnumerator SpawnStart(StageInstance stageInstance)
    {
        enemySpawnComplite = false;
        yield return new WaitForSeconds(timeBetweenWave);

        WaveData waveData = stageInstance.currentStageInfo.waves[stageInstance.currentWave];

        for (int i = 0; i < waveData.monsters.Length; i++)
        {
            yield return new WaitForSeconds(timeBetweenSpawns);

            MonsterSpawnData monsterSpawnData = waveData.monsters[i];
            for (int j = 0; j < monsterSpawnData.spawnCount; j++)
            {
                SpawnRandomEnemy(monsterSpawnData.monsterType);
            }
        }

        if (waveData.hasBoss)
        {
            yield return new WaitForSeconds(timeBetweenSpawns);

            gameManager.MainCameraShake();
            SpawnRandomEnemy(waveData.bossType);
        }

        enemySpawnComplite = true;
    }
}

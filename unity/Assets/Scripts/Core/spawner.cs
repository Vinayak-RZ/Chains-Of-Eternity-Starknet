using System.Collections.Generic;
using System.Threading.Tasks;
using PurrNet;
using Unity.AppUI.UI;
using UnityEngine;
using UnityEngine.UI;

public class Spawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    public List<Spawnable> spawnables = new(); // n is just spawnables.Count

    [Header("Spawn Area")]
    public Vector2 spawnAreaSize = new Vector2(10f, 10f); // Width and height of the spawn area

    public UnityEngine.Canvas mainCanvas;

    public GameObject EnemyHealthBarPrefab;

    private async void Start()
    {
        // Wait until DojoManager is initialized
        while (!DojoManager.Instance.IsInitialized)
            await Task.Yield();

    }
    void Update()
    {
        foreach (var spawnable in spawnables)
        {
            spawnable.timer += Time.deltaTime;

            if (spawnable.timer >= spawnable.spawnInterval && spawnable.currentCount < spawnable.maxSpawnCount)
            {
                Spawn(spawnable);
                spawnable.timer = 0f;
            }
        }
    }

    void Spawn(Spawnable s)
    {
        // Calculate a random position within the spawn area rectangle centered around this spawner's position
        Vector2 offset = new Vector2(
            Random.Range(-spawnAreaSize.x / 2f, spawnAreaSize.x / 2f),
            Random.Range(-spawnAreaSize.y / 2f, spawnAreaSize.y / 2f)
        );

        Vector2 spawnPosition = (Vector2)transform.position + offset;

        GameObject obj = UnityProxy.InstantiateDirectly(s.prefab, spawnPosition, Quaternion.identity);
        if (obj.TryGetComponent<Enemy>(out Enemy enemy))
        {
            // Initialize the enemy with a reference to the main canvas and health bar prefab
            Initialize(enemy, EnemyHealthBarPrefab);
        }
        s.currentCount++;
        _ = CallEnemySpawn();
        // Optional: Handle death/cleanup to decrement count later
        // obj.GetComponent<SpawnedObject>()?.Init(() => s.currentCount--);
    }
    private void Initialize(Enemy enemy, GameObject healthBarPrefab)
    {
        GameObject canvasObj = UnityProxy.InstantiateDirectly(mainCanvas.gameObject);
        UnityEngine.Canvas canvas = canvasObj.GetComponent<UnityEngine.Canvas>();
        canvasObj.transform.SetParent(mainCanvas.transform, false);
        enemy.enemyslider = canvasObj.GetComponent<Slider>();
    }
    private async Task CallEnemySpawn()
    {
        try
        {
            // This runs asynchronously without blocking gameplay
            var result = await DojoActions.CreateEnemy(
                pos_x: (ushort)transform.position.x,
                pos_y: (ushort)transform.position.y,
                enemy_type: (sbyte)0 // Default enemy type
            );

            Debug.Log($"✅ Enemy spawned. Tx: {result.Inner}");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"❌ Error spawning enemy: {ex.Message}");
        }
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.black;
        
        Gizmos.DrawWireCube(transform.position, spawnAreaSize);
    }
}

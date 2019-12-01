using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public struct SpawnHeight
    {
        public float min;
        public float max;
    }

    public GameObject EnemyPrefab;
    public float shiftSpeed;
    public float spawnRate;
    public SpawnHeight spawnHeight;
    public Vector3 spawnPos;
    public Vector2 targetAspectRatio;
    public bool beginInScreenCenter;

    List<Transform> enemies;
    float spawnTimer;
    GameManager game;
    float targetAspect;
    Vector3 dynamicSpawnPos;

    void Start()
    {
        enemies = new List<Transform>();
        game = GameManager.Instance;
        if (beginInScreenCenter)
            SpawnEnemy();
    }

    void OnEnable()
    {
        GameManager.OnGameOverConfirmed += OnGameOverConfirmed;
    }

    void OnDisable()
    {
        GameManager.OnGameOverConfirmed -= OnGameOverConfirmed;
    }

    void OnGameOverConfirmed()
    {
        for (int i = enemies.Count - 1; i >= 0; i--)
        {
            GameObject temp = enemies[i].gameObject;
            enemies.RemoveAt(i);
            Destroy(temp);
        }
        if (beginInScreenCenter)
            SpawnEnemy();
    }

    void Update()
    {
        if (game.GameOver) return;

        targetAspect = (float)targetAspectRatio.x / targetAspectRatio.y;
        dynamicSpawnPos.x = (spawnPos.x * Camera.main.aspect) / targetAspect;

        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnRate)
        {
            SpawnEnemy();
            spawnTimer = 0;
        }

        ShiftEnemies();
    }

    void SpawnEnemy()
    {
        GameObject enemy = Instantiate(EnemyPrefab) as GameObject;
        enemy.transform.SetParent(transform);
        enemy.transform.localPosition = dynamicSpawnPos;
        if (beginInScreenCenter && enemies.Count == 0)
        {
            enemy.transform.localPosition = Vector3.zero;
        }
        float randomYPos = Random.Range(spawnHeight.min, spawnHeight.max);
        enemy.transform.position += Vector3.up * randomYPos;
        enemies.Add(enemy.transform);
    }

    void ShiftEnemies()
    {
        for (int i = enemies.Count - 1; i >= 0; i--)
        {
            enemies[i].position -= Vector3.right * shiftSpeed * Time.deltaTime;
            if (enemies[i].position.x < (-dynamicSpawnPos.x * Camera.main.aspect) / targetAspect)
            {
                GameObject temp = enemies[i].gameObject;
                enemies.RemoveAt(i);
                Destroy(temp);
            }
        }
    }
}

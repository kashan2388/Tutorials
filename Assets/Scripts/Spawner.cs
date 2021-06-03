using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Wave[] Waves; //웨이브 배열
    public Enemy enemy;

    Wave currenWave;
    int currentWaveNumber;//현재 웨이브 횟수

    float nextSpawnTime;//다음 스폰시간
    int enemiesRemainingToSpawn;//남아있는 스폰할 적
    int enemiesRemaingAlive;

    MapGenerator map;
    void Start()
    {
        map = FindObjectOfType<MapGenerator>();
        NextWave();
    }
    void Update()
    {
        if(enemiesRemainingToSpawn >0 && Time.time > nextSpawnTime) //남아있는 스폰될 적이 0보다 크고 현재 시간이 다음스폰시간보다 크다면 
        {
            enemiesRemainingToSpawn--; //남아있는 스폰될 적을 하나 줄이고 
            nextSpawnTime = Time.time + currenWave.timeBetweenSpawns; //Time.time 이 현재시간을 가리킴

            StartCoroutine(SpawnEnemy());
        }
    }

    IEnumerator SpawnEnemy()
    {
        float spawnDelay = 1;
        float tileFlashSpeed = 4;

        Transform randomTile = map.GetRandomOpenTile();
        Material tileMat = randomTile.GetComponent<Renderer>() .material;
        Color initialColour = tileMat.color;
        Color flashColur = Color.red;
        float spawnTimer = 0;

        while (spawnTimer < spawnDelay)
        {
           tileMat.color = Color.Lerp(initialColour, flashColur, Mathf.PingPong(spawnTimer * tileFlashSpeed, 1));

            spawnTimer += Time.deltaTime;
            yield return null;
        }

        Enemy spawnedEnemy = Instantiate(enemy, Vector3.zero, Quaternion.identity) as Enemy;
        spawnedEnemy.OnDeath += OnEnemyDeath; //적 스폰 때마다 spawnEnemy.OnDeath 에 OnEnemyOnDeath메서드 추가

        

    }
     
    void OnEnemyDeath()
    {
        print("적은 죽었다");
        enemiesRemaingAlive--;
        print("남아있는 적: " + enemiesRemaingAlive);

        if(enemiesRemaingAlive == 0)
        {
            NextWave();
        }
    }

    void NextWave()
    {
        currentWaveNumber++;
        print("웨이브 " + currentWaveNumber);
        if(currentWaveNumber -1 <Waves.Length) //웨이브 배열 길이(Length)
        {
            currenWave = Waves[currentWaveNumber - 1];

            enemiesRemainingToSpawn = currenWave.enemyCount;

            enemiesRemaingAlive = enemiesRemainingToSpawn;
        }

    }

    [System.Serializable]
    public class Wave
    {
        public int enemyCount;
        public float timeBetweenSpawns;
    }

    
}

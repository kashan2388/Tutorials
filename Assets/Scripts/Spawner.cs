using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Wave[] Waves; //웨이브 배열
    public Enemy enemy;

    LivingEntity playerEntity;
    Transform playerT;

    Wave currenWave;
    int currentWaveNumber;//현재 웨이브 횟수

    float nextSpawnTime;//다음 스폰시간
    int enemiesRemainingToSpawn;//남아있는 스폰할 적
    int enemiesRemaingAlive;

    MapGenerator map;

    float timeBetWeenCampingChecks = 2;
    float campThresholdDistance = 1.5f; //캠프 한계거리(최소 움직여야 할 거리)
    float nextCampCheckTime;
    Vector3 campPositionOld;
    bool isCamping;

    bool isDisabled;

    public event System.Action<int> OnNewWave;

    void Start()
    {
        playerEntity = FindObjectOfType<Player>();
        playerT = playerEntity.transform;

        nextCampCheckTime = timeBetWeenCampingChecks + Time.time;
        campPositionOld = playerT.position;
        playerEntity.OnDeath += OnPlayerDeath;

        map = FindObjectOfType<MapGenerator>();
        NextWave();

    }
    void Update()
    {
        if (!isDisabled)
        {
            if (Time.time > nextCampCheckTime)
            {
                nextCampCheckTime = Time.time + timeBetWeenCampingChecks;

                //플레이어의 현재위치와 과거위치 사이의 거리가, 기준거리(campDistance)보다 작은가? 
                isCamping = (Vector3.Distance(playerT.position, campPositionOld) < campThresholdDistance);
                campPositionOld = playerT.position;

            }

            if (enemiesRemainingToSpawn > 0 && Time.time > nextSpawnTime) //남아있는 스폰될 적이 0보다 크고 현재 시간이 다음스폰시간보다 크다면 
            {
                enemiesRemainingToSpawn--; //남아있는 스폰될 적을 하나 줄이고 
                nextSpawnTime = Time.time + currenWave.timeBetweenSpawns; //Time.time 이 현재시간을 가리킴

                StartCoroutine(SpawnEnemy());
            }
        }
    }

    IEnumerator SpawnEnemy()
    {
        float spawnDelay = 1;
        float tileFlashSpeed = 4;

        Transform spawnTile = map.GetRandomOpenTile();
        if(isCamping)
        {
            spawnTile = map.GetTileFromPosition(playerT.position);
        }
        Material tileMat = spawnTile.GetComponent<Renderer>() .material;
        Color initialColour = tileMat.color;
        Color flashColur = Color.red;
        float spawnTimer = 0;

        while (spawnTimer < spawnDelay)
        {
           tileMat.color = Color.Lerp(initialColour, flashColur, Mathf.PingPong(spawnTimer * tileFlashSpeed, 1));

            spawnTimer += Time.deltaTime;
            yield return null;
        }
        //위의 구문, spawnTimer이 대기시간 spawndelay를 넘었을 때 아래의 구문 실행(적을 소환)
        Enemy spawnedEnemy = Instantiate(enemy, spawnTile.position + Vector3.up, Quaternion.identity) as Enemy;
        spawnedEnemy.OnDeath += OnEnemyDeath; //적 스폰 때마다 spawnEnemy.OnDeath 에 OnEnemyOnDeath메서드 추가

    }

    void OnPlayerDeath()
    {
        isDisabled = true;
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

    void ResetPlayerPosition()
    {
        playerT.position = map.GetTileFromPosition(Vector3.zero).position + Vector3.up*3;
    }

    void NextWave() //처음과 플레잉어가 웨이브를 끝낼 때마다 호출 
    {
        currentWaveNumber++;
        print("웨이브 " + currentWaveNumber);
        if(currentWaveNumber -1 <Waves.Length) //웨이브 배열 길이(Length)
        {
            currenWave = Waves[currentWaveNumber - 1];

            enemiesRemainingToSpawn = currenWave.enemyCount;

            enemiesRemaingAlive = enemiesRemainingToSpawn;

            if(OnNewWave != null)
            {
                OnNewWave(currentWaveNumber);
            }
            ResetPlayerPosition();
        }

    }

    [System.Serializable]
    public class Wave
    {
        public int enemyCount;
        public float timeBetweenSpawns;
    }

    
}

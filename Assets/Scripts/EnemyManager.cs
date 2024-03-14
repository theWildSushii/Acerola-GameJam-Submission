using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class EnemyManager : MonoBehaviour {

    [SerializeField] private float tickRate = Mathf.PI;
    [SerializeField] private float targetStress = 10f;
    [SerializeField] private float stressOverTime = 0.05f;
    [SerializeField] private EnemyController[] enemyPrefabs;
    [SerializeField] private Transform[] spawnPoints;

    private List<EnemyPool> pools = new List<EnemyPool>();

    private float timeToNextTick = 0f;

    private bool isSpawning = true;

    private void Awake() {
        foreach(EnemyController prefab in enemyPrefabs) {
            pools.Add(new EnemyPool(prefab));
        }
    }

    private void Update() {
        targetStress += stressOverTime * Time.deltaTime;
        if(Time.time < timeToNextTick) {
            return;
        }
        timeToNextTick = Time.time + ( 1f / tickRate );
        if(isSpawning) {
            if(EnemyController.CurrentStress < targetStress) {
                EnemyPool pool = pools.Random();
                if(EnemyController.CurrentStress + pool.StressContribution <= targetStress) {
                    EnemyController spawned = pool.Get();
                    spawned.transform.position = SpawnPoint.RandomPoint;
                    spawned.SearchPoint(PlayerController.Instance.Position);
                    if(EnemyController.CurrentStress >= targetStress * 0.9f) {
                        isSpawning = false;
                    }
                }
            }
        } else {
            if(EnemyController.CurrentStress <= targetStress * 0.382f) {
                isSpawning = true;
            }
        }
    }


    private class EnemyPool {
        public ObjectPool<EnemyController> pool;

        private EnemyController prefab;

        public float StressContribution { get; protected set; }

        public EnemyPool(EnemyController enemyPrefab) {
            prefab = enemyPrefab;
            StressContribution = enemyPrefab.StressContribution;
            pool = new ObjectPool<EnemyController>(OnCreate, OnGet, OnRelease, OnDestroy);
        }

        private EnemyController OnCreate() {
            EnemyController created = Instantiate(prefab);
            created.ParentPool = pool;
            return created;
        }

        private void OnGet(EnemyController controller) {
            controller.gameObject.SetActive(true);
            controller.Actor.enabled = true;
        }

        private void OnRelease(EnemyController controller) {
            controller.Actor.enabled = false;
            controller.gameObject.SetActive(false);
        }

        private void OnDestroy(EnemyController controller) {
            Destroy(controller.gameObject);
        }

        public EnemyController Get() {
            return pool.Get();
        }
    }

}

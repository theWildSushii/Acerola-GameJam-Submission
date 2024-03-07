using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class EnemyManager : MonoBehaviour {

    [SerializeField] private float tickRate = Mathf.PI;
    [SerializeField] private float targetStress = 10f;
    [SerializeField] private float streesOverTime = 0.01f;
    [SerializeField] private EnemyController[] enemyPrefabs;
    [SerializeField] private Transform[] spawnPoints;

    private List<EnemyPool> pools = new List<EnemyPool>();

    private float timeToNextTick = 0f;

    private void Awake() {
        foreach(EnemyController prefab in enemyPrefabs) {
            pools.Add(new EnemyPool(prefab));
        }
    }

    private void Update() {
        if(Time.time < timeToNextTick) {
            return;
        }
        timeToNextTick = Time.time + ( 1f / tickRate );
        if(EnemyController.CurrentStress < targetStress) {
            EnemyPool pool = pools.Random();
            if(EnemyController.CurrentStress + pool.StressContribution <= targetStress) {
                EnemyController spawned = pool.Get();
                System.Array.Sort(spawnPoints, (a, b) => {
                    float sqrDistanceA = ( PlayerController.Instance.Position - (Vector2)a.transform.position ).sqrMagnitude;
                    float sqrDistanceB = ( PlayerController.Instance.Position - (Vector2)b.transform.position ).sqrMagnitude;
                    return -sqrDistanceA.CompareTo(sqrDistanceB);
                });
                float random = Random.value;
                random = random * random * random;
                spawned.transform.position = (Vector2)spawnPoints[Mathf.FloorToInt(random * ( spawnPoints.Length - 1 ))].position;
                spawned.SearchPoint(PlayerController.Instance.Position);
            }
        }
        targetStress += streesOverTime * Time.deltaTime;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos() {
        if(spawnPoints.Length <= 0) {
            return;
        }
        Gizmos.color = Color.green;
        foreach(Transform spawnPoint in spawnPoints) {
            Gizmos.matrix = spawnPoint.localToWorldMatrix;
            Gizmos.DrawWireSphere(Vector3.zero, 0.5f);
        }
    }
#endif

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

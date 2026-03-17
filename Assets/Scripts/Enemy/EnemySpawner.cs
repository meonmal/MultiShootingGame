using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    private float spawnTime;
    [SerializeField]
    private Enemy[] enemyPrefabs;

    private Collider2D coll;

    private IObjectPool<Enemy> pool;

    private void Awake()
    {
        coll = GetComponent<Collider2D>();
        pool = new ObjectPool<Enemy>(
            SpawnEnemy,
            OnGet,
            OnRelease,
            OnDestroyEnemy,
            true,
            20,
            200);
    }

    private void Start()
    {
        StartCoroutine(Spawn());
    }

    private IEnumerator Spawn()
    {
        while (true)
        {
            Enemy enemy = pool.Get();
            enemy.transform.position = SpawnPosition();

            yield return new WaitForSeconds(spawnTime);
        }
    }

    private Enemy SpawnEnemy()
    {
        int index = Random.Range(0, enemyPrefabs.Length);
        Enemy clone = Instantiate(enemyPrefabs[index]);
        clone.SetPool(pool);

        return clone;
    }

    private Vector2 SpawnPosition()
    {
        Bounds bounds = coll.bounds;

        float randomY = Random.Range(bounds.min.y, bounds.max.y);

        Vector2 spawnposition = new Vector2(transform.position.x, randomY);

        return spawnposition;
    }

    private void OnGet(Enemy enemy)
    {
        enemy.gameObject.SetActive(true);
    }

    private void OnRelease(Enemy enemy)
    {
        enemy.gameObject.SetActive(false);
    }

    private void OnDestroyEnemy(Enemy enemy)
    {
        Destroy(enemy.gameObject);
    }
}

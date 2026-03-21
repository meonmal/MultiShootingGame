using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class BulletSpawner : MonoBehaviour
{
    [SerializeField]
    private Bullet bulletPrefab;

    private Player player;
    private float spawnTime;

    private IObjectPool<Bullet> pool;

    private void Awake()
    {
        player = GetComponentInParent<Player>();

        pool = new ObjectPool<Bullet>(
            SpawnBullet,
            OnGet,
            OnRelease,
            DestroyBullet,
            true,
            20,
            maxSize: 100);
    }

    private IEnumerator Spawn()
    {
        while (true)
        {
            spawnTime = player.GetStats(StatType.CoolTime);

            Bullet bullet = pool.Get();

            SoundManager.Instance.PlaySfx(SfxType.PlayerShoot);

            bullet.transform.position = SpawnPosition();

            yield return new WaitForSeconds(spawnTime);
        }
    }

    private void Start()
    {
        StartCoroutine(Spawn());
    }

    private Bullet SpawnBullet()
    {
        Bullet bullet = Instantiate(bulletPrefab, transform);
        bullet.SetPool(pool);

        return bullet;
    }

    private Vector2 SpawnPosition()
    {
        Vector2 playerPosition = transform.position;

        return playerPosition;
    }

    public void OnGet(Bullet bullet)
    {
        bullet.gameObject.SetActive(true);
    }

    public void OnRelease(Bullet bullet)
    {
        bullet.gameObject.SetActive(false);
    }

    public void DestroyBullet(Bullet bullet)
    {
        Destroy(bullet.gameObject);
    }
}

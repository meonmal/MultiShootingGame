using System.Collections.Generic;
using UnityEngine;

public class BossBulletPool : MonoBehaviour
{
    [SerializeField] private BossBullet bulletPrefab;
    [SerializeField] private int initialSize = 20;
    [SerializeField] private Transform bulletParent;

    private Queue<BossBullet> bulletQueue = new Queue<BossBullet>();

    private void Awake()
    {
        CreatePool();
    }

    private void CreatePool()
    {
        for (int i = 0; i < initialSize; i++)
        {
            BossBullet bullet = Instantiate(bulletPrefab, bulletParent);
            bullet.gameObject.SetActive(false);
            bullet.Init(this);

            bulletQueue.Enqueue(bullet);
        }
    }

    public BossBullet GetBullet()
    {
        if (bulletQueue.Count == 0)
        {
            BossBullet bullet = Instantiate(bulletPrefab, bulletParent);
            bullet.gameObject.SetActive(false);
            bullet.Init(this);

            return bullet;
        }

        return bulletQueue.Dequeue();
    }

    public void ReturnBullet(BossBullet bullet)
    {
        bullet.gameObject.SetActive(false);
        bulletQueue.Enqueue(bullet);
    }
}

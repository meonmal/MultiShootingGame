using System.Collections.Generic;
using UnityEngine;

public class BossBulletPool : MonoBehaviour
{
    /// <summary>
    /// 보스가 발사할 보스 총알 프리팹.
    /// </summary>
    [SerializeField]
    private BossBullet bulletPrefab;
    /// <summary>
    /// 초기 생성 개수.
    /// </summary>
    [SerializeField]
    private int initialSize = 20;
    /// <summary>
    /// 총알의 부모 정보.
    /// </summary>
    [SerializeField]
    private Transform bulletParent;

    /// <summary>
    /// 보스의 총알은 IObjectPool이 아닌 Queue로 관리한다.
    /// </summary>
    private readonly Queue<BossBullet> bulletQueue = new Queue<BossBullet>();

    private void Awake()
    {
        CreatePool();
    }

    /// <summary>
    /// 풀의 초기 개수만큼 총알을 생성하는 함수.
    /// </summary>
    private void CreatePool()
    {
        for (int i = 0; i < initialSize; i++)
        {
            // 초기 숫자만큼 보스의 총알을 생성한다.
            BossBullet bullet = Instantiate(bulletPrefab, bulletParent);
            // 다만 생성해도 당장은 필요가 없으니 비활성화 해주고
            bullet.gameObject.SetActive(false);
            // 총알을 초기화한다.
            bullet.Init(this);

            // 방금 생성한 총알은 큐에 저장한다.
            bulletQueue.Enqueue(bullet);
        }
    }

    /// <summary>
    /// 총알이 필요해질 때 생성하는 함수.
    /// </summary>
    /// <returns></returns>
    public BossBullet GetBullet()
    {
        // 현재 큐에 저장된 총알이 없다면 실행.
        if (bulletQueue.Count == 0)
        {
            // 총알을 생성하고 게임 오브젝트는 비활성화한다.
            BossBullet bullet = Instantiate(bulletPrefab, bulletParent);
            bullet.gameObject.SetActive(false);
            bullet.Init(this);

            return bullet;
        }

        // 생성한 총알은 큐에서 삭제한다.
        return bulletQueue.Dequeue();
    }

    /// <summary>
    /// 총알을 풀로 되돌리는 함수.
    /// </summary>
    /// <param name="bullet">풀로 되돌릴 총알.</param>
    public void ReturnBullet(BossBullet bullet)
    {
        // 총알 게임 오브젝트를 비활성화한다.
        bullet.gameObject.SetActive(false);
        // 비활성화한 총알 오브젝트를 큐에 저장한다.
        bulletQueue.Enqueue(bullet);
    }
}

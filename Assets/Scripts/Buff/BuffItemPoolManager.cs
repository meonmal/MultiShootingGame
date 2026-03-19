using UnityEngine;
using UnityEngine.Pool;

public class BuffItemPoolManager : MonoBehaviour
{
    [SerializeField]
    private BuffItem buffItemPrefab;
    [SerializeField]
    private int defaultCapacity = 10;
    [SerializeField]
    private int maxSize = 50;

    private IObjectPool<BuffItem> pool;

    private void Awake()
    {
        pool = new ObjectPool<BuffItem>(
            CreateItem,
            OnGetItem,
            OnReleaseItem,
            OnDestroyItem,
            true,
            defaultCapacity,
            maxSize
        );
    }

    private BuffItem CreateItem()
    {
        BuffItem item = Instantiate(buffItemPrefab, transform);
        item.gameObject.SetActive(false);
        return item;
    }

    private void OnGetItem(BuffItem item)
    {
        item.gameObject.SetActive(true);
    }

    private void OnReleaseItem(BuffItem item)
    {
        item.gameObject.SetActive(false);
    }

    private void OnDestroyItem(BuffItem item)
    {
        Destroy(item.gameObject);
    }

    public void Spawn(BuffData data, Vector3 position)
    {
        BuffItem item = pool.Get();
        item.transform.position = position;
        item.Init(data, this);
    }

    public void Release(BuffItem item)
    {
        pool.Release(item);
    }
}

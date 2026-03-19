using UnityEngine;

public class BuffItem : MonoBehaviour
{
    [SerializeField]
    private float lifeTime = 5f;

    private BuffData buffData;
    private float currentLifeTime;
    private BuffItemPoolManager poolManager;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Init(BuffData data, BuffItemPoolManager manager)
    {
        buffData = data;
        poolManager = manager;
        currentLifeTime = lifeTime;

        if (spriteRenderer != null && buffData != null)
        {
            spriteRenderer.sprite = buffData.icon;
        }

        gameObject.SetActive(true);
    }

    private void Update()
    {
        currentLifeTime -= Time.deltaTime;

        if (currentLifeTime <= 0f)
        {
            ReturnToPool();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Player player = other.GetComponent<Player>();

        if (player == null)
            return;

        player.AddBuff(buffData);
        ReturnToPool();
    }

    private void ReturnToPool()
    {
        if (poolManager != null)
        {
            poolManager.Release(this);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}

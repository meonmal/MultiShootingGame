using UnityEngine;

public class BuffDropManager : MonoBehaviour
{
    [SerializeField]
    private BuffItemPoolManager poolManager;
    [SerializeField, Range(0f, 1f)]
    private float dropChance = 0.1f;
    [SerializeField]
    private BuffData[] buffDatas;

    public void TryDrop(Vector3 position)
    {
        if (poolManager == null || buffDatas == null || buffDatas.Length == 0)
            return;

        if (Random.value > dropChance)
            return;

        BuffData selected = GetRandomBuffDataByWeight();

        if (selected == null)
            return;

        poolManager.Spawn(selected, position);
    }

    private BuffData GetRandomBuffDataByWeight()
    {
        int totalWeight = 0;

        for (int i = 0; i < buffDatas.Length; i++)
        {
            if (buffDatas[i] != null)
                totalWeight += buffDatas[i].dropWeight;
        }

        if (totalWeight <= 0)
            return null;

        int roll = Random.Range(0, totalWeight);
        int current = 0;

        for (int i = 0; i < buffDatas.Length; i++)
        {
            if (buffDatas[i] == null)
                continue;

            current += buffDatas[i].dropWeight;

            if (roll < current)
                return buffDatas[i];
        }

        return null;
    }
}

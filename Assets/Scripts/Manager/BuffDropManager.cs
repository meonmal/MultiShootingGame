using UnityEngine;

public class BuffDropManager : MonoBehaviour
{
    /// <summary>
    /// 버프 아이템을 관리할 버프 풀 매니저.
    /// </summary>
    [SerializeField]
    private BuffItemPoolManager poolManager;
    /// <summary>
    /// 버프가 뜰 확률
    /// 0이면 0%, 0.1이면 10%, 1이면 100%다.
    /// </summary>
    [SerializeField, Range(0f, 1f)]
    private float dropChance = 0.1f;
    /// <summary>
    /// 버프 데이터.
    /// </summary>
    [SerializeField]
    private BuffData[] buffDatas;

    /// <summary>
    /// 일정 확률로 버프를 생성하는 함수.
    /// 드롭 확률을 통과하면 가중치를 기반으로 버프를 하나 선택하여 생성한다.
    /// </summary>
    /// <param name="position">버프가 생성될 위치.</param>
    public void TryDrop(Vector3 position)
    {
        // 뭐 안 넣었으면 실행 종료.
        if (poolManager == null || buffDatas == null || buffDatas.Length == 0)
        {
            return;
        }

        // 확률 안 되면 실행 종료.
        if (Random.value > dropChance)
        {
            return;
        }

        // 가중치를 기반으로 버프 하나 선택.
        BuffData selected = GetRandomBuffDataByWeight();

        // 선택된 버프가 없으면 실행 종료.
        if (selected == null)
        {
            return;
        }

        // 선택된 버프를 해당 위치에 생성.
        poolManager.Spawn(selected, position);
    }

    /// <summary>
    /// 각 버프의 dropWeight를 기반으로
    /// 랜덤하게 하나의 BuffData를 선택하는 함수.
    /// </summary>
    /// <returns></returns>
    private BuffData GetRandomBuffDataByWeight()
    {
        int totalWeight = 0;

        // 전체 dropWeight의 합을 계산.
        // 이 값이 전체 확률 범위가 된다.
        for (int i = 0; i < buffDatas.Length; i++)
        {
            if (buffDatas[i] != null)
            {
                totalWeight += buffDatas[i].dropWeight;
            }
        }

        // 유효한 weight가 없으면 선택 불가.
        if (totalWeight <= 0)
        {
            return null;
        }

        // 0 ~ totalWeight 범위에서 랜덤 값 생성.
        int roll = Random.Range(0, totalWeight);
        int current = 0;

        // 누적 weight를 이용하여 선택.
        for (int i = 0; i < buffDatas.Length; i++)
        {
            if (buffDatas[i] == null)
            {
                continue;
            }

            // 현재 버프의 weight를 누적.
            current += buffDatas[i].dropWeight;

            // 랜덤 값이 현재 누적값보다 작으면 해당 버프 선택.
            if (roll < current)
            {
                return buffDatas[i];
            }
        }

        // 사실 없어도 되는데 일단 안전빵으로 넣음.
        return null;
    }
}

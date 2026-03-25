using System.Collections.Generic;
using UnityEngine;

public class BuffUI : MonoBehaviour
{
    /// <summary>
    /// 버프를 표시할 슬롯 배열.
    /// 최대 표시 가능한 버프 개수 = 슬롯 개수 (현재 12개)
    /// </summary>
    [SerializeField]
    private BuffSlotUI[] buffSlots;

    /// <summary>
    /// 현재 적용중인 버프 목록.
    /// ActiveBuff는 남은 시간, 데이터 등을 관리하는 런타임 객체이다.
    /// readonly로 선언하여 리스트 자체의 참조 변경은 막고,
    /// 내부 요소(Add, Remove)는 허용한다.
    /// </summary>
    private readonly List<ActiveBuff> activeBuffs = new();

    private void Start()
    {
        // 초기 UI 상태를 비워준다.
        RefreshUI();
    }

    private void Update()
    {
        // 활성 버프가 없디면 아무런 처리도 하지 않는다.
        // 불필요한 연산 방지.
        if (activeBuffs.Count == 0)
        {
            return;
        }

        // 모든 버프의 남은 시간을 감소시키고
        // 끝난 버프가 있는지 체크한다.
        bool isRemoved = UpdateBuffTimes(Time.deltaTime);

        // 시간 변화가 있기 때문에 매 프레인 UI를 갱신한다.
        RefreshUI();
    }

    /// <summary>
    /// 새로운 버프를 추가하는 함수.
    /// 이미 동일한 버프가 있다면 시간만 갱신하고,
    /// 없다면 새로 추가한다.
    /// </summary>
    /// <param name="buffData">UI에 갱신할 버프.</param>
    public void AddBuff(BuffData buffData)
    {
        // 잘못된 데이터 방어 코드.
        if (buffData == null)
        {
            return;
        }

        // 동일한 버프가 이미 존재하는지 검사한다.
        for (int i = 0; i < activeBuffs.Count; i++)
        {
            if (activeBuffs[i].Data == buffData)
            {
                // 같은 버프면 새로 생성해서 시간만 초기화.
                activeBuffs[i] = new ActiveBuff(buffData);
                RefreshUI();
                return;
            }
        }

        // 만약 최대 슬롯 개수보다 현재 먹은 버프가 더 많으면
        // 추가는 불가능하다.
        if (activeBuffs.Count >= buffSlots.Length)
        {
            return;
        }

        // 새로운 버프 생성 후 리스트에 추가.
        ActiveBuff newBuff = new ActiveBuff(buffData);
        activeBuffs.Add(newBuff);

        RefreshUI();
    }

    /// <summary>
    /// 버프의 남은 시간을 감소시키고
    /// 시간이 끝난 버프는 리스트에서 제거한다.
    /// </summary>
    /// <param name="deltaTime">감소 시간.</param>
    /// <returns>버프가 끝났는지 여부 확인.</returns>
    private bool UpdateBuffTimes(float deltaTime)
    {
        bool removed = false;

        // 뒤에서부터 순회한다. (RemoveAt 안정성 확보)
        for (int i = activeBuffs.Count - 1; i >= 0; i--)
        {
            bool isFinished = activeBuffs[i].UpdateTime(deltaTime);

            if (isFinished)
            {
                // 버프 종료 -> 리스트에서 제거.
                activeBuffs.RemoveAt(i);
                removed = true;
            }
        }

        return removed;
    }

    /// <summary>
    /// 현재 activeBuffs 상태를 기반으로 UI를 갱신한다.
    /// 버프 개수만큼 슬롯을 채우고 나머지는 비워둔다.
    /// </summary>
    private void RefreshUI()
    {
        for (int i = 0; i < buffSlots.Length; i++)
        {
            if (i < activeBuffs.Count)
            {
                ActiveBuff buff = activeBuffs[i];

                // 아이콘 + 남은 시간 비율 반영.
                buffSlots[i].SetBuff(buff.Data.icon, buff.NormalizedTime);
            }
            else
            {
                // 남는 슬롯은 비워서 UI 정리
                buffSlots[i].Clear();
            }
        }
    }
}

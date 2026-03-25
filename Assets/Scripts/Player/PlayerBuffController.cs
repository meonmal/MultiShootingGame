using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어에게 적용된 버프를 관리하는 컨트롤러.
/// 
/// 역할:
/// - 버프 추가 및 만료 관리
/// - 스탯별 버프 값을 계산
/// - 캐싱을 통해 성능 최적화
/// </summary>
public class PlayerBuffController : MonoBehaviour
{
    /// <summary>
    /// 현재 적용 중인 버프 목록.
    /// ActiveBuff는 남은 시간과 BuffData를 포함하고 있다.
    /// </summary>
    private readonly List<ActiveBuff> activeBuffs = new List<ActiveBuff>();

    /// <summary>
    /// 스탯별 "더하기 버프" 값을 캐싱하는 Dictionary.
    /// ex) 공격력 +10, 이동속도 +2 등
    /// </summary>
    private readonly Dictionary<StatType, float> addCache = new Dictionary<StatType, float>();

    /// <summary>
    /// 스탯별 "곱하기 버프" 값을 캐싱하는 Dictionary.
    /// ex) 공격력 x1.2, 속도 x1.5 등
    /// </summary>
    private readonly Dictionary<StatType, float> multiplyCache = new Dictionary<StatType, float>();

    /// <summary>
    /// 캐시가 최신 상태인지 여부.
    /// 
    /// true:
    /// - 버프가 변경됨 (추가 or 만료)
    /// - 캐시를 다시 계산해야 함
    /// 
    /// false:
    /// - 캐시가 최신 상태
    /// - 재계산 없이 바로 사용 가능
    /// </summary>
    private bool isDirty = true;

    /// <summary>
    /// 매 프레임마다 버프 시간을 감소시키고,
    /// 만료된 버프를 제거하는 함수.
    /// </summary>
    private void Update()
    {
        // 뒤에서부터 순회해야 RemoveAt 시 인덱스 문제가 발생하지 않는다.
        for (int i = activeBuffs.Count - 1; i >= 0; i--)
        {
            // 버프의 남은 시간을 감소시키고 만료 여부를 반환받는다.
            bool isExpired = activeBuffs[i].UpdateTime(Time.deltaTime);

            // 버프가 끝났다면 리스트에서 제거
            if (isExpired)
            {
                activeBuffs.RemoveAt(i);

                // 버프 목록이 변경되었으므로 캐시를 다시 계산해야 한다.
                isDirty = true;
            }
        }
    }

    /// <summary>
    /// 새로운 버프를 추가하는 함수.
    /// </summary>
    /// <param name="buffData">추가할 버프 데이터</param>
    public void AddBuff(BuffData buffData)
    {
        // null 방어
        if (buffData == null)
            return;

        // 새로운 ActiveBuff 생성 후 리스트에 추가
        activeBuffs.Add(new ActiveBuff(buffData));

        // 버프가 추가되었으므로 캐시 무효화
        isDirty = true;
    }

    /// <summary>
    /// 특정 스탯의 "더하기 버프 값"을 반환한다.
    /// ex) 공격력 +값
    /// </summary>
    public float GetAddValue(StatType statType)
    {
        // 필요할 경우 캐시 재계산
        RebuildCacheIfNeeded();

        // 해당 스탯이 존재하면 값 반환
        if (addCache.TryGetValue(statType, out float value))
            return value;

        // 없으면 기본값 0 반환 (더하기는 0이 기본)
        return 0f;
    }

    /// <summary>
    /// 특정 스탯의 "곱하기 버프 값"을 반환한다.
    /// ex) 공격력 x값
    /// </summary>
    public float GetMultiplyValue(StatType statType)
    {
        RebuildCacheIfNeeded();

        if (multiplyCache.TryGetValue(statType, out float value))
            return value;

        // 없으면 기본값 1 반환 (곱하기는 1이 기본)
        return 1f;
    }

    /// <summary>
    /// 캐시가 더티 상태일 때만 버프 값을 다시 계산하는 함수.
    /// 
    /// 핵심:
    /// - 매 프레임 계산하지 않고
    /// - "버프 변경 시점"에만 계산
    /// → 성능 최적화
    /// </summary>
    private void RebuildCacheIfNeeded()
    {
        // 캐시가 최신이면 아무 작업도 하지 않는다.
        if (!isDirty)
            return;

        // 기존 캐시 초기화
        addCache.Clear();
        multiplyCache.Clear();

        // 모든 활성 버프를 순회하면서 스탯별 값을 계산
        foreach (ActiveBuff activeBuff in activeBuffs)
        {
            StatType statType = activeBuff.Data.statType;

            // 초기값 설정
            if (!addCache.ContainsKey(statType))
                addCache[statType] = 0f;

            if (!multiplyCache.ContainsKey(statType))
                multiplyCache[statType] = 1f;

            // 버프 타입에 따라 계산 방식이 다름
            if (activeBuff.Data.valueType == BuffValueType.Add)
            {
                // 더하기 버프는 누적 합산
                addCache[statType] += activeBuff.Data.value;
            }
            else if (activeBuff.Data.valueType == BuffValueType.Multiply)
            {
                // 곱하기 버프는 누적 곱
                multiplyCache[statType] *= activeBuff.Data.value;
            }
        }

        // 캐시가 최신 상태가 되었으므로 Dirty 해제
        isDirty = false;
    }
}
using UnityEngine;

/// <summary>
/// 버프 값을 어떤 방식으로 적용할지 결정하는 타입
/// </summary>
public enum BuffValueType
{
    // 플레이어의 스탯에서 값을 더한다.
    Add,
    // 플레이어의 스탯에서 값을 곱한다.
    Multiply,
}

[CreateAssetMenu(fileName = "BuffData", menuName = "Scriptable Objects/BuffData")]
public class BuffData : ScriptableObject
{
    /// <summary>
    /// 플레이어의 스탯 종류.
    /// </summary>
    public StatType statType;
    /// <summary>
    /// 버프의 값을 올리는 종류.
    /// </summary>
    public BuffValueType valueType;

    /// <summary>
    /// 버프 적용 값
    /// Add 타입이면 실제 증가량,
    /// Multiply 타입이면 배율 값으로 사용.
    /// ex. Add 10 -> + 10, Multiply 1.2f -> 20% 증가
    /// </summary>
    public float value;
    /// <summary>
    /// 버프의 총 지속시간.
    /// </summary>
    public float duration = 5f;

    /// <summary>
    /// 버프의 이미지.
    /// </summary>
    public Sprite icon;

    /// <summary>
    /// 버프 가중치. 최소 값은 1이다.
    /// 이 값이 높으면 해당 버프가 다른 버프들에 비해 더 자주 나타난다.
    /// </summary>
    [Min(1)]
    public int dropWeight = 1;

    /// <summary>
    /// 버프 설명 문구를 생성한다.
    /// Add 타입이면 고정 수치 증가량을,
    /// Multiply 타입이면 퍼센트 증가값을 문자열로 반환한다.
    /// </summary>
    /// <returns>버프의 증가 or 감소값을 문자열로 반환.</returns>
    public string GetDescription()
    {
        // 만약 버프타입이 Add라면 
        if (valueType == BuffValueType.Add)
        {
            // 버프의 내용 + 증가값으로 반환한다.
            return $"{GetStatName()} +{FormatNumber(value)}";
        }

        // 배율 값을 퍼센트로 변환해서 보여준다.
        // ex. 1.2f = 20% 증가, 0.8f = 20% 감소.
        float percent = (value - 1f) * 100f;
        // +0 : 양수면 +를 붙이고 -0 : 음수면 -를 붙인다. 0이면 그냥 0.
        return $"{GetStatName()} {percent:+0;-0;0}%";
    }

    /// <summary>
    /// 버프의 증가값을 문자열 형태로 반환한다.
    /// 정수라면 소수점 없이 표시하고, 소수라면 최대 2자리까지만 표시한다.
    /// </summary>
    /// <param name="number">버프의 증가 값</param>
    /// <returns></returns>
    private string FormatNumber(float number)
    {
        // number를 1로 나눈 나머지가 0에 가깝다면
        // 소수점이 없는 정수값으로 판단한다.
        // 굳이 Approximately를 쓰는 이유 : 
        // float의 오차 때문이다. 예를 들어 가끔 10.00000001f 이런 숫자가 나오는데
        // 그런 오차가 게임에 표시되는 것을 방지하기 위함이다.
        if (Mathf.Approximately(number % 1f, 0f))
        {
            return ((int)number).ToString();
        }

        // 소수점은 최대 2자리까지만 표시한다.
        return number.ToString("0.##");
    }

    /// <summary>
    /// 버프 타입에 따른 스탯 설명 문구.
    /// </summary>
    /// <returns></returns>
    private string GetStatName()
    {
        switch (statType)
        {
            case StatType.MoveSpeed:
                return "이동속도 증가!";
            case StatType.PlayerDamage:
                return "공격력 증가!";
            case StatType.BulletSpeed:
                return "총알속도 증가!";
            case StatType.CoolTime:
                return "쿨타임 감소!";
            default:
                return "능력치";
        }
    }
}

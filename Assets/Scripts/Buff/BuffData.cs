using UnityEngine;

public enum BuffValueType
{
    Add,
    Multiply,
}

[CreateAssetMenu(fileName = "BuffData", menuName = "Scriptable Objects/BuffData")]
public class BuffData : ScriptableObject
{
    public StatType statType;
    public BuffValueType valueType;

    public float value;
    public float duration = 5f;

    public Sprite icon;

    [Min(1)]
    public int dropWeight = 1;

    public string GetDescription()
    {
        if (valueType == BuffValueType.Add)
        {
            return $"{GetStatName()} +{FormatNumber(value)}";
        }

        float percent = (value - 1f) * 100f;
        return $"{GetStatName()} {percent:+0;-0;0}%";
    }

    private string FormatNumber(float number)
    {
        if (Mathf.Approximately(number % 1f, 0f))
        {
            return ((int)number).ToString();
        }

        return number.ToString("0.##");
    }

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

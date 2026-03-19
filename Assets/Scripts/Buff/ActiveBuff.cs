using UnityEngine;

public class ActiveBuff
{
    public BuffData Data { get; private set; }
    public float RemainingTime { get; private set; }
    public float Duration { get; private set; } // 추가

    public ActiveBuff(BuffData data)
    {
        Data = data;
        Duration = data.duration; // 추가
        RemainingTime = data.duration;
    }

    public bool UpdateTime(float deltaTime)
    {
        RemainingTime -= deltaTime;
        return RemainingTime <= 0f;
    }

    public float NormalizedTime
    {
        get
        {
            if (Duration <= 0f) return 0f;
            return RemainingTime / Duration;
        }
    }
}

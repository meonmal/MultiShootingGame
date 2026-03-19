using UnityEngine;

public class ActiveBuff
{
    public BuffData Data { get; private set; }
    public float RemainingTime { get; private set; }

    public ActiveBuff(BuffData data)
    {
        Data = data;
        RemainingTime = data.duration;
    }

    public bool UpdateTime(float deltaTime)
    {
        RemainingTime -= deltaTime;
        return RemainingTime <= 0f;
    }
}

using UnityEngine;

public class ActiveBuff
{
    /// <summary>
    /// 게임에서 사용되는 버프들의 데이터.
    /// SciriptableObject다.
    /// </summary>
    public BuffData Data { get; private set; }
    /// <summary>
    /// 현재 버프의 남은 시간.
    /// </summary>
    public float RemainingTime { get; private set; }
    /// <summary>
    /// 버프 지속시간.
    /// 초기 값이며 변하지 않는 기준값이다.
    /// </summary>
    public float Duration { get; private set; }

    /// <summary>
    /// ActiveBuff의 생성자.
    /// 다른 곳에서 new ActiveBuff를 하면 자동으로 실행된다.
    /// </summary>
    /// <param name="data">버프들의 데이터</param>
    public ActiveBuff(BuffData data)
    {
        // 버프 데이터를 안 넣었으면 실행될 조건문.
        if (data == null)
        {
            Debug.LogError("버프 데이터가 없음");
            return;
        }

        // 데이터 초기화.
        Data = data;
        // 버프의 총 시간은 BuffData의 버프 지속시간으로 맞춘다.
        Duration = data.duration;
        // 버프의 남은 시간은 BuffData의 버프 지속시간으로 맞춘다.
        RemainingTime = data.duration;
    }


    /// <summary>
    /// 버프의 남은 시간을 판별하는 함수.
    /// </summary>
    /// <param name="deltaTime">감소하는 시간.</param>
    /// <returns></returns>
    public bool UpdateTime(float deltaTime)
    {
        // 버프 지속시간 감소.
        RemainingTime -= deltaTime;

        // 현재 남은 시간이 0 아래로 내려가지 않게 만든다.
        if(RemainingTime < 0f)
        {
            // 0 이하면 0으로 설정.
            RemainingTime = 0f;
        }

        // 지속시간이 끝났는지의 여부를 반환한다.
        // 지속시간이 0과 같거나 작으면 true.
        return RemainingTime <= 0f;
    }

    /// <summary>
    /// 현재 버프의 남은 시간을 반환하는 함수.
    /// UI에 쓸 예정이다.
    /// </summary>
    public float NormalizedTime
    {
        get
        {
            // 혹시 모를 안전장치.
            if (Duration <= 0f)
            {
                // 버프의 총 지속시간이 0 이하면.
                // 그냥 0을 반환하게 만든다.(버프가 끝났다는 의미다.)
                return 0f;
            }
            // 버프의 (남은 시간 / 총 지속 시간)을 반환한다.
            return RemainingTime / Duration;
        }
    }
}

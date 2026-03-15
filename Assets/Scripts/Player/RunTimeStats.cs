using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RunTimeStats
{
    /// <summary>
    /// 스탯의 현재 레벨
    /// </summary>
    private int currentLevel;

    /// <summary>
    /// 스탯의 값
    /// IReadOnlyList : 읽기 전용 컬렉션. 말 그대로 특정 컬렉션을 읽기 전용으로 만든다.
    /// </summary>
    private IReadOnlyList<float> values;

    /*
    생성자. 
    클래스가 생성될 때(new 할 때) 자동으로 실행되는 함수다.
    이름이 클래스의 이름과 같고 반환 타입이 없다.
    또한 생성자는 여러개 만들 수 있다.
     */

    /// <summary>
    /// 스탯 객체를 처음 만들 때 초기 상태를 설정하는 함수.
    /// ex. new StatRuntime(data.moveSpeed) -> values = moveSpeed 리스트
    /// </summary>
    /// <param name="values">스탯의 값</param>
    public RunTimeStats(IReadOnlyList<float> values)
    {
        this.values = values;
        currentLevel = 0;
    }

    /// 스탯 값에 대한 프로퍼티.
    /// currentLevel이 배열 범위를 벗어나더라도
    /// Mathf.Clamp를 사용하여 안전한 인덱스로 접근한다.
    public float Values => values[Mathf.Clamp(currentLevel, 0, values.Count - 1)];

    /// 현재 레벨이 최대 레벨인지 여부.
    /// currentLevel이 values.Count - 1 이상이면 최대 레벨로 간주한다.
    public bool IsMax => currentLevel >= values.Count - 1;

    /// 스탯 레벨을 1 증가시킨다.
    /// Mathf.Clamp를 사용하여
    /// 0 미만 또는 최대 레벨(values.Count - 1)을 초과하지 않도록 제한한다.
    public void LevelUp()
    {
        currentLevel = Mathf.Clamp(currentLevel + 1, 0, values.Count - 1);
    }
}

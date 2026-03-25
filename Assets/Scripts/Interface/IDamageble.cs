using UnityEngine;

public interface IDamageble
{
    /// <summary>
    /// 데미지를 받는 함수.
    /// </summary>
    /// <param name="damage">받는 데미지.</param>
    void TakeDamage(float damage);
}

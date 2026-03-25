using UnityEngine;
using UnityEngine.UI;

public class BuffSlotUI : MonoBehaviour
{
    /// <summary>
    /// 버프 슬롯에 보일 버프 이미지.
    /// </summary>
    [SerializeField]
    private Image iconImage;
    /// <summary>
    /// 버프 남은 시간을 표시하는 Fill 이미지.
    /// fillAmount를 이용해 쿨타임처럼 표현한다.
    /// </summary>
    [SerializeField]
    private Image cooldownFillImage;

    private void Awake()
    {
        // 초기화 작업.
        Clear();
    }

    /// <summary>
    /// 데이터를 UI에 반영하는 함수.
    /// </summary>
    /// <param name="icon">슬롯에 보일 이미지</param>
    /// <param name="normalizedTime">버프의 남은 시간 비율.
    /// 1이면 시작 상태, 0이면 종료 상태이다.</param>
    public void SetBuff(Sprite icon, float normalizedTime)
    {
        // 아이콘 이미지를 icon으로 설정.
        iconImage.sprite = icon;
        // 남은 시간은 Fill을 이용해서 보이게 만든다.
        cooldownFillImage.fillAmount = 1f - normalizedTime;

        // 알파값 조정.
        SetAlpha(iconImage, 1f);
        SetAlpha(cooldownFillImage, 1f);
    }

    /// <summary>
    /// 초기화 함수.
    /// </summary>
    public void Clear()
    {
        iconImage.sprite = null;
        cooldownFillImage.fillAmount = 0f;

        SetAlpha(iconImage, 0f);
        SetAlpha(cooldownFillImage, 0f);
    }

    /// <summary>
    /// 알파값을 조정하는 함수.
    /// </summary>
    /// <param name="image"></param>
    /// <param name="alpha"></param>
    private void SetAlpha(Image image, float alpha)
    {
        Color color = image.color;
        color.a = alpha;
        image.color = color;
    }
}

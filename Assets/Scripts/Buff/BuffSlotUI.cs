using UnityEngine;
using UnityEngine.UI;

public class BuffSlotUI : MonoBehaviour
{
    [SerializeField]
    private Image iconImage;
    [SerializeField]
    private Image cooldownFillImage;

    private void Awake()
    {
        Clear();
    }

    public void SetBuff(Sprite icon, float normalizedTime)
    {
        iconImage.sprite = icon;
        cooldownFillImage.fillAmount = 1f - normalizedTime;

        SetAlpha(iconImage, 1f);
        SetAlpha(cooldownFillImage, 1f);
    }

    public void Clear()
    {
        iconImage.sprite = null;
        cooldownFillImage.fillAmount = 0f;

        SetAlpha(iconImage, 0f);
        SetAlpha(cooldownFillImage, 0f);
    }

    private void SetAlpha(Image image, float alpha)
    {
        Color color = image.color;
        color.a = alpha;
        image.color = color;
    }
}

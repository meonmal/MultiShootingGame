using UnityEngine;

public class TitleButton : MonoBehaviour
{
    private Vector2 originScale;
    private Vector2 bigScale;

    private void Start()
    {
        originScale = transform.localScale;
        bigScale = new Vector2(transform.localScale.x * 1.2f, transform.localScale.y * 1.2f);
    }

    public void MouseUp()
    {
        SoundManager.Instance.PlaySfx(SfxType.ButtonClick);
        transform.localScale = bigScale;
    }

    public void MouseDown()
    {
        transform.localScale = originScale;
    }
}

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Joystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    /// <summary>
    /// Joystick의 바깥 원
    /// </summary>
    [SerializeField]
    private Image background;
    /// <summary>
    /// Joystick의 핸들러
    /// </summary>
    [SerializeField]
    private Image handler;

    float joystickRadius;
    /// <summary>
    /// 처음으로 터치를 한 위치
    /// </summary>
    private Vector2 touchPosition;
    private Vector2 moveDir;

    public Vector2 MoveDir => moveDir;

    private void Start()
    {
        joystickRadius = background.gameObject.GetComponent<RectTransform>().sizeDelta.y / 2;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 touchDir = (eventData.position - touchPosition);

        float moveDist = Mathf.Min(touchDir.magnitude, joystickRadius);
        moveDir = touchDir.normalized;
        Vector2 newPosition = touchPosition + moveDir * moveDist;
        handler.transform.position = newPosition;
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        background.transform.position = eventData.position;
        handler.transform.position = eventData.position;
        touchPosition = eventData.position;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        handler.transform.position = touchPosition;
        moveDir = Vector2.zero;
    }
}

using TMPro;
using UnityEngine;

public class BuffPopup : MonoBehaviour
{
    /// <summary>
    /// 버프 획득 메시지를 화면에 표시하는 TMP 텍스트 컴포넌트.
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI messageText;
    /// <summary>
    /// 버프 아이템 텍스트가 화면에 표시되는 지속시간.
    /// </summary>
    [SerializeField]
    private float lifeTime = 1f;
    /// <summary>
    /// 버프 아이템이 위로 올라가는 속도.
    /// </summary>
    [SerializeField]
    private float moveSpeed = 1f;

    /// <summary>
    /// 버프 팝업이 화면에 남아 있을 남은 시간.
    /// 이걸 감소시킬 예정이다.
    /// </summary>
    private float timer;
    /// <summary>
    /// 버프 아이템 텍스트가 존재할 위치.
    /// </summary>
    private Vector3 worldPosition;
    /// <summary>
    /// 메인 카메라.
    /// </summary>
    private Camera mainCamera;

    /// <summary>
    /// 버프 팝업을 표시하기 전에 호출하는 초기화 함수.
    /// 표시할 메시지, 시작 위치, 남은 시간을 설정한다.
    /// </summary>
    /// <param name="message">버프 아이템 텍스트.</param>
    /// <param name="startWorldPosition">텍스트가 뜰 위치.</param>
    public void Init(string message, Vector3 startWorldPosition)
    {
        // 초기화 작업.
        messageText.text = message;
        worldPosition = startWorldPosition;
        timer = lifeTime;
        mainCamera = Camera.main;

        // 변경된 월드 좌표를 화면 좌표로 변환하여 UI 위치를 갱신한다.
        UpdateScreenPosition();
    }

    private void Update()
    {
        // 버프 텍스트가 존재하는 현재 시간을 감소시킨다.
        timer -= Time.deltaTime;
        // 버프 아이템 텍스트의 위치를 위로 천천히 올린다.
        worldPosition += Vector3.up * moveSpeed * Time.deltaTime;

        // 변경된 월드 좌표를 화면 좌표로 변환하여 UI 위치를 갱신한다.
        UpdateScreenPosition();

        // 만약 시간이 다 되었다면 실행.
        if (timer <= 0f)
        {
            // 버프 아이템 텍스트 삭제.
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 현재 월드 좌표를 화면 좌표로 변환하여
    /// UI 오브젝트의 위치를 갱신하는 함수.
    /// </summary>
    private void UpdateScreenPosition()
    {
        // 메인 카메라가 없으면 좌표 변환을 할 수 없으므로 종료한다.
        if (mainCamera == null)
        {
            return;
        }

        // 월드 좌표를 화면 좌표로 변환한 뒤
        // 현재 UI 오브젝트의 위치에 적용한다.
        transform.position = mainCamera.WorldToScreenPoint(worldPosition);
    }
}

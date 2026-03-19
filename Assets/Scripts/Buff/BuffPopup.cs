using TMPro;
using UnityEngine;

public class BuffPopup : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI messageText;
    [SerializeField]
    private float lifeTime = 1f;
    [SerializeField]
    private float moveSpeed = 1f;

    private float timer;
    private Vector3 worldPosition;
    private Camera mainCamera;

    public void Init(string message, Vector3 startWorldPosition)
    {
        messageText.text = message;
        worldPosition = startWorldPosition;
        timer = lifeTime;
        mainCamera = Camera.main;

        UpdateScreenPosition();
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        worldPosition += Vector3.up * moveSpeed * Time.deltaTime;

        UpdateScreenPosition();

        if (timer <= 0f)
        {
            Destroy(gameObject);
        }
    }

    private void UpdateScreenPosition()
    {
        if (mainCamera == null)
            return;

        transform.position = mainCamera.WorldToScreenPoint(worldPosition);
    }
}

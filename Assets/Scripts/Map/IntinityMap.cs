using UnityEngine;

public class IntinityMap : MonoBehaviour
{
    [SerializeField]
    private float speed;

    private Vector2 offset;
    private Material material;

    private void Awake()
    {
        material = GetComponent<Renderer>().material;
    }

    private void Update()
    {
        offset.y -= speed * Time.deltaTime;

        // 혹시나 게임 한판이 길어질 경우
        // offset.y 가 너무 길어질 수 있기에
        // -1 ~ 0 사이를 오가게 만듦
        if(offset.y <= -1f)
        {
            offset.y += 1f;
        }

        material.mainTextureOffset = offset;
    }
}

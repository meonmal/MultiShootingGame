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

        if(offset.y <= -1f)
        {
            offset.y += 1f;
        }

        material.mainTextureOffset = offset;
    }
}

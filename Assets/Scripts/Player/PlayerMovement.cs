using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float xLimit;
    [SerializeField]
    private float yLimit;

    private Player player;
    private float speed;

    private Rigidbody2D rigid;

    private void Awake()
    {
        player = GetComponent<Player>();
        rigid = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        Movement();
    }

    private void Movement()
    {
        speed = player.GetStats(StatType.MoveSpeed);

        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        Vector2 moveDirection = new Vector2(moveX, moveY).normalized;

        rigid.linearVelocity = moveDirection * speed;

        Vector2 pos = rigid.position;
        pos.x = Mathf.Clamp(pos.x, -xLimit, xLimit);
        pos.y = Mathf.Clamp(pos.y, -yLimit, yLimit);

        rigid.position = pos;
    }
}

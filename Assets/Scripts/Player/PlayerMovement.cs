using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private Joystick joystick;

    private Player player;
    private float speed;

    private Rigidbody2D rigid;

    private void Awake()
    {
        player = GetComponent<Player>();
        rigid = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        speed = player.GetStats(StatType.MoveSpeed);
    }

    private void FixedUpdate()
    {
        Movement();
    }

    private void Movement()
    {
        Vector2 input = joystick.MoveDir;
        rigid.linearVelocity = input * speed;
    }

}

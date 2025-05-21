using UnityEngine;

public class MiddleSlimeMove : MonoBehaviour
{
    public float jumpPower = 7f;
    public float movePower = 2f;
    public float jumpDelay = 0.5f;
    public LayerMask groundLayer; // 바닥 판별용

    private Rigidbody2D rigid;
    private Animator anim;
    private bool movingRight = false;
    private bool isGrounded = false;
    private float jumpTimer = 0f;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        if (isGrounded)
        {
            jumpTimer += Time.fixedDeltaTime;

            if (jumpTimer >= jumpDelay)
            {
                // 점프하기 전 앞에 땅이 없으면 방향 전환
                if (!IsGroundAhead())
                {
                    movingRight = !movingRight;
                }

                Jump();
                jumpTimer = 0f;
            }
        }

        FlipDirection(); // 반전 처리
    }

    private void Jump()
    {
        float moveDir = movingRight ? 1f : -1f;
        Vector2 jumpForce = new Vector2(moveDir * movePower, jumpPower);

        rigid.velocity = jumpForce;
        isGrounded = false;

        anim.SetBool("isGrounded", false);
        anim.SetTrigger("Jump");
    }

    private bool IsGroundAhead()
    {
        Vector2 origin = transform.position + new Vector3(movingRight ? 0.5f : -0.5f, 0, 0);
        Vector2 direction = Vector2.down;
        float distance = 1.5f;

        // 앞쪽 아래로 Ray 쏴서 바닥 있는지 확인
        RaycastHit2D hit = Physics2D.Raycast(origin, direction, distance, groundLayer);
        Debug.DrawRay(origin, direction * distance, Color.red); // 씬 뷰 디버깅용
        return hit.collider != null;
    }

    private void FlipDirection()
    {
        transform.localScale = new Vector3(movingRight ? 1 : -1, 1, 1);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            anim.SetBool("isGrounded", true);
        }
    }
}

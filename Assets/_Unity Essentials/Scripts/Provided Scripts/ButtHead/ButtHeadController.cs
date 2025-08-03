using System.Diagnostics;
using UnityEngine;

public class ButtHeadController : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 2.5f;
    public float jumpForce = 3f;

    [Header("Rotation")]
    public float rotationAngle = 120f; // Angle to rotate the player


    [Header("Animation")]
    public string floorTag = "Floor";
    public string idleAnimation = "isIdle";
    public string movementAnimation = "movement";

    private Rigidbody2D rb;
    private Animator playerAnimator;
    private bool canMoveRight = true;
    private bool canMoveLeft = true;
    private bool isGrounded = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        playerAnimator = GetComponent<Animator>();

        // SetInitialRotation();
    }

    void Update()
    {

        SetIdleAnimation(Input.GetAxisRaw("Horizontal") == 0);

        playerAnimator.SetFloat(movementAnimation, Input.GetAxisRaw("Horizontal"));

        if (Input.GetAxisRaw("Horizontal") > 0 && canMoveRight)
        {
            Move();
        }

        if (Input.GetAxisRaw("Horizontal") < 0 && canMoveLeft)
        {
            Move();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
    }

    void FixedUpdate()
    {
        // SetFallingAnimation();
    }

    void Move()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");

        rb.linearVelocity = new Vector2(horizontalInput * speed, rb.linearVelocity.y);

        setMoveAnimation();
    }

    void Jump()
    {
        if (!isGrounded)
        {
            return; // Prevent jumping if not grounded
        }
       
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

        isGrounded = false;

        SetJumpAnimation();
    }

    void SetIdleAnimation(bool isIdle)
    {
        playerAnimator.SetBool(idleAnimation, isIdle);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(floorTag))
        {
            isGrounded = true;

            foreach (ContactPoint2D contact in collision.contacts)
            {
                // Si la normal apunta hacia arriba, es un aterrizaje
                if (contact.normal.x < -0.7f)
                {
                    canMoveRight = false;
                }

                if (contact.normal.x > 0.7f)
                {
                    canMoveLeft = false;
                }
            }
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(floorTag))
        {
            canMoveRight = true;
            canMoveLeft = true;
        }
    }

    private bool IsLandingFromAbove(Collision2D collision)
    {
        bool isLandingFromAbove = false;

        foreach (ContactPoint2D contact in collision.contacts)
        {
            // Si la normal apunta hacia arriba, es un aterrizaje
            if (contact.normal.y > 0.7f) // 0.7f es un buen umbral
            {
                return true;
            }
        }

        return isLandingFromAbove;
    }

    void SetInitialRotation()
    {
        Vector3 currentRotation = transform.eulerAngles;
        transform.rotation = Quaternion.Euler(currentRotation.x, rotationAngle, currentRotation.z);
    }

    void setMoveAnimation()
    {
        bool isMovingFoward = Input.GetAxisRaw("Horizontal") > 0;

        if (isMovingFoward)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, rotationAngle, 0);
        }

        playerAnimator.SetFloat("movement", Input.GetAxisRaw("Horizontal"));
    }

    void SetFallingAnimation()
    {
        bool isFreeFalling = rb.linearVelocity.y < -0.5f;
        // playerAnimator.SetBool("FreeFall", isFreeFalling);
    }

    void SetJumpAnimation()
    {
    //    playerAnimator.SetBool("Grounded", false);
      //  playerAnimator.SetTrigger("Jump");
    }

}
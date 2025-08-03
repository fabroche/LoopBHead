using System.Diagnostics;
using UnityEngine;

public class PlayerController2DLeftRight : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 2.5f;
    public float motionSpeed = 2.5f;
    public float jumpForce = 3f;

    [Header("Rotation")]
    public float rotationAngle = 120f; // Angle to rotate the player


    [Header("Animation")]
    public string floorTag = "Floor";

    private Rigidbody2D rb;
    private Animator playerAnimator;
    private bool canMoveRight = true;
    private bool canMoveLeft = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        playerAnimator = GetComponent<Animator>();

        // SetInitialRotation();
    }

    void Update()
    {

        SetIdleAnimation();
        //       UnityEngine.Debug.Log($"isLookingRight: {isLookingRight}"); // Log to check if Update is running.

        if (Input.GetAxisRaw("Horizontal") == 0)
        {
            SetIdleAnimation();
        }

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
        SetFallingAnimation();
    }

    void Move()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");

        rb.linearVelocity = new Vector2(horizontalInput * speed, rb.linearVelocity.y);

        setMoveAnimation();
    }

    void Jump()
    {
        if (!playerAnimator.GetBool("Grounded"))
        {
           return; // Prevent jumping if not grounded
        }

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

        SetJumpAnimation();
    }

    void SetIdleAnimation()
    {
        playerAnimator.SetFloat("Speed", 0);
        playerAnimator.SetFloat("MotionSpeed", motionSpeed);
    }

    public void OnLand()
    {
        // playerAnimator.SetTrigger("Grounded");
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(floorTag))
        {
            if (IsLandingFromAbove(collision))
            {
                playerAnimator.SetBool("Jump", false); // Set grounded state to true when landing
                playerAnimator.SetBool("FreeFall", false);
                playerAnimator.SetBool("Grounded", true); // Set grounded state to true when landing
            }

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

    public void OnFootstep()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");

        rb.linearVelocity = new Vector2(horizontalInput * speed, rb.linearVelocity.y);
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
            transform.rotation = Quaternion.Euler(0, rotationAngle, 0);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, -rotationAngle, 0);
        }

        playerAnimator.SetFloat("Speed", speed);
        playerAnimator.SetFloat("MotionSpeed", motionSpeed);

    }

    void SetFallingAnimation()
    {
        bool isFreeFalling = rb.linearVelocity.y < -0.5f;
        playerAnimator.SetBool("FreeFall", isFreeFalling);
    }
    
    void SetJumpAnimation()
    {
        playerAnimator.SetBool("Grounded", false);
        playerAnimator.SetTrigger("Jump");
    }

}
using System.Diagnostics;
using TMPro;
using UnityEngine;

public class ButtHeadController : MonoBehaviour
{
    [Header("Movement")] public float speed = 2.5f;
    public float jumpForce = 3f;

    [Header("Rotation")] public float rotationAngle = 120f; // Angle to rotate the player

    [Header("Attack Settings")] public KeyCode attackKey = KeyCode.J;
    public KeyCode insteractionKey = KeyCode.E;
    [SerializeField] private string weaponTag = "Weapon";

    [Header("Animation")] public string floorTag = "Floor";
    public string idleAnimation = "isIdle";
    public string movementAnimation = "movement";
    public string attackAnimation = "isAttacking";

    [Header("Enemy Tag")] public string enemyTag = "Monster";
    public bool haveWeapon = false;
    public bool haveBattery = false;

    [Header("InteractionFlags")] public bool isCarryingMonsterEgg = false;
    
    [Header("UI")]
    
    private Rigidbody2D rb;
    private Animator playerAnimator;
    private bool canMoveRight = true;
    private bool canMoveLeft = true;
    private bool isGrounded = true;
    private float lifePoints = 1f;

    public GameObject timeJumpControlInfoUI;
    private TextMeshPro _timeJumpControlInfoUIScript;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        _timeJumpControlInfoUIScript = timeJumpControlInfoUI.GetComponent<TextMeshPro>();
        playerAnimator = GetComponent<Animator>();

        // SetInitialRotation();
    }

    void Update()
    {
        HandleIsAlive();
        
        HandleWinStage();
        
        SetAttackAnimation(haveWeapon && Input.GetKey(attackKey));

        SetIdleAnimation(Input.GetAxisRaw("Horizontal") == 0);

        playerAnimator.SetFloat(movementAnimation, Input.GetAxisRaw("Horizontal"));

        // HandleControlInfoUIStatus(!isCarryingMonsterEgg);

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

    private void HandleControlInfoUIStatus(bool isHableToUse)
    {
        
        if (isHableToUse && timeJumpControlInfoUI.GetComponent<UIControlInfoController>().actionName == "Time Jump")
        {
            _timeJumpControlInfoUIScript.color = Color.red;
        }
        else
        {
            _timeJumpControlInfoUIScript.color = Color.white;
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
        if (collision.gameObject.CompareTag(enemyTag))
        {
            lifePoints -= 1f;
        }
        
        if (collision.gameObject.CompareTag(weaponTag))
        {
            UnityEngine.Debug.Log($"Weapon collected {collision.gameObject}");
            haveWeapon = true;
            playerAnimator.SetBool("haveWeapon", true);
        }

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

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Abyss")) {
            lifePoints -= 1f;
        }
    }

    void SetEnableAttack(bool isAttackEnabled)
    {
        haveWeapon = isAttackEnabled;
        playerAnimator.SetBool("haveWeapon", isAttackEnabled);
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
    private void HandleIsAlive()
    {
        if (lifePoints <= 0)
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.GameOver();
            }
        }
    }

    void SetAttackAnimation(bool isAttacking)
    {
        playerAnimator.SetBool(attackAnimation, isAttacking);
    }

    void SetJumpAnimation()
    {
        //    playerAnimator.SetBool("Grounded", false);
        //  playerAnimator.SetTrigger("Jump");
    }
    
    private void HandleWinStage()
    {
        if (!haveBattery) return;
        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.YouWin();
        }
    }
}
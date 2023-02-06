using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour, IUpdateStats
{
    [Header("Player Colliders")]
    [SerializeField] Collider2D feetCollider;
    [SerializeField] Collider2D bodyCollider;

    [Header("Speed")]
    float moveSpeed;
    [SerializeField] float jumpSpeed;
    [SerializeField] float rollSpeed;
    [SerializeField] AnimationClip rollAnimation;

    [Header("Miscs")]
    [SerializeField] float maxVelocity;
    [SerializeField] GameObject jumpVFX;
    [SerializeField] GameObject rollVFX;
    [SerializeField] ParticleSystem fireVFX;
    [SerializeField] AnimatorOverrideController crownAnimator;
    [SerializeField] GameObject crownVFX;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Animator animator;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] bool disableJump;
    [SerializeField] bool disableRoll;
    [SerializeField] AudioSource rollAS;

    [HideInInspector] public bool disableControl = false;
    [HideInInspector] public bool isRolling = false;

    CatchReferences references;
    PlayerInputSystem inputActions;

    const float slowDownTime = 1.5f;

    bool isInActivity;
    bool isRoomReversed;
    bool cursed;
    

    private void Awake()
    {
        references = FindObjectOfType<CatchReferences>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        inputActions = new();
        inputActions.PlayerMovement.Jump.performed += ctx => OnJump();
        inputActions.PlayerMovement.Jump.canceled += ctx => OnJumpCanceled();
        inputActions.PlayerMovement.Roll.performed += ctx => OnRoll();

        inputActions.PlayerMovement.Enable();
    }

    private void OnDisable()
    {
        inputActions.PlayerMovement.Disable();
    }

    private void Update()
    {
        if (references.GetPlayerHealth().isDead) return;
        if (disableControl) return;

        Movement();
        FlipSprite();
        ClampVelocity();
    }

    
    private void Movement()
    {
        if (isRolling)
        {
            if ((rb.velocity.x < 0 && GetHorizontalAxis() > 0) || (rb.velocity.x > 0 && GetHorizontalAxis() < 0))
            {
                isRolling = false;

                StopAllCoroutines();
            }
            else
            {
                return;
            }
        }

        Fall();
        RightAndLeft();
    }

    private void Fall()
    {
        if (!feetCollider.IsTouchingLayers(LayerMask.GetMask("Ground")) && bodyCollider.IsTouchingLayers(LayerMask.GetMask("Ground")))
        {
            if (rb.velocity.y > 0)
            {
                animator.SetBool("fall", false);
                animator.SetBool("jump", true);
            }
            else
            {
                animator.SetBool("fall", true);
                animator.SetBool("jump", false);
            }
            
        }
        else if (!feetCollider.IsTouchingLayers(LayerMask.GetMask("Ground")) && rb.velocity.y > 0)
        {
            animator.SetBool("fall", false);
            animator.SetBool("jump", true);
        }
        else if (!feetCollider.IsTouchingLayers(LayerMask.GetMask("Ground")) && rb.velocity.y < 0)
        {
            animator.SetBool("fall", true);
            animator.SetBool("jump", false);
        }
        else
        {
            animator.SetBool("fall", false);
            animator.SetBool("jump", false);
        }
    }

    private void OnJump()
    {
        if (disableJump) return;
        if (references.GetPlayerHealth().isDead) return;
        if (disableControl) return;
        if (!feetCollider.IsTouchingLayers(LayerMask.GetMask("Ground"))) return;
        if (isRolling) return;

        if (jumpVFX)
        {
            Instantiate(jumpVFX, new Vector2(transform.position.x - 0.145f, transform.position.y - 0.266f), Quaternion.identity);
        }

        rb.AddForce(new Vector2(0, jumpSpeed));
    }

    private void OnJumpCanceled()
    {
        if (feetCollider.IsTouchingLayers(LayerMask.GetMask("Ground"))) return;
        if (rb.velocity.y < 0) return;

        rb.velocity = new(rb.velocity.x, 0);
    }

    public void OnRoll()
    {
        if (disableRoll) return;
        if (references.GetPlayerHealth().isDead) return;
        if (disableControl) return;
        if (isRolling) return;
        if (!feetCollider.IsTouchingLayers(LayerMask.GetMask("Ground"))) return;

        StartCoroutine(Roll());
    }

    private IEnumerator Roll()
    {
        isRolling = true;

        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        animator.Rebind();

        animator.SetTrigger("roll");

        rollAS.Play();

        if (rollVFX)
        {
            Instantiate(rollVFX, new Vector2(transform.position.x, transform.position.y - 0.317f), Quaternion.identity);
        }

        if (!spriteRenderer.flipX)
        {
            rb.velocity += new Vector2(rollSpeed, rb.velocity.y);
        }
        else if (spriteRenderer.flipX)
        {
            rb.velocity += new Vector2(-rollSpeed, rb.velocity.y);
        }

        yield return new WaitForSeconds(rollAnimation.length);

        isRolling = false;
    }

    private void RightAndLeft()
    {
        if (GetHorizontalAxis() != 0 && feetCollider.IsTouchingLayers(LayerMask.GetMask("Ground")))
        {
            rb.constraints = RigidbodyConstraints2D.None | RigidbodyConstraints2D.FreezeRotation;

            if (references.GetGameManager().GetKeyboardControl())
            {
                rb.velocity = new Vector2(Mathf.Sign(GetHorizontalAxis()) * moveSpeed, rb.velocity.y);
            }
            else
            {
                rb.velocity = new Vector2(GetHorizontalAxis() * moveSpeed, rb.velocity.y);
            }
            
            animator.SetBool("run", true);
        }
        else if (GetHorizontalAxis() == 0 && feetCollider.IsTouchingLayers(LayerMask.GetMask("Ground")) && !isRolling)
        {
            rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;

            rb.velocity = new Vector2();
            animator.SetBool("run", false);
        }

        if (GetHorizontalAxis() != 0 && !feetCollider.IsTouchingLayers(LayerMask.GetMask("Ground")))
        {
            rb.constraints = RigidbodyConstraints2D.None | RigidbodyConstraints2D.FreezeRotation;

            rb.velocity = new Vector2(GetHorizontalAxis() * moveSpeed, rb.velocity.y);

            animator.SetBool("run", true);
        }
    }

    private void ClampVelocity()
    {
        if (rb.velocity.x > maxVelocity)
        {
            rb.velocity = new Vector2(maxVelocity, rb.velocity.y);
        }
        else if (rb.velocity.x < -maxVelocity)
        {
            rb.velocity = new Vector2(-maxVelocity, rb.velocity.y);
        }

        if (rb.velocity.y > maxVelocity)
        {
            rb.velocity = new Vector2(rb.velocity.x, maxVelocity);
        }
        else if (rb.velocity.y < -maxVelocity)
        {
            rb.velocity = new Vector2(rb.velocity.x, -maxVelocity);
        }
    }

    private void FlipSprite()
    {
        if (GetHorizontalAxis() < 0)
        {
            spriteRenderer.flipX = true;
        }
        else if (GetHorizontalAxis() > 0)
        {
            spriteRenderer.flipX = false;
        }
    }

    public void StopPlayer(bool rebindAnimator)
    {
        if (rebindAnimator)
        {
            animator.Rebind();
        }
        
        rb.velocity = new Vector2();
    }

    public float GetHorizontalAxis()
    {
        float axis;

        if (references.GetGameManager().GetKeyboardControl())
        {
            axis =  Input.GetAxis("Horizontal");
        }
        else
        {
            axis =  Input.GetAxis("Horizontal Controller");
        }

        if (isRoomReversed)
        {
            axis *= -1;
        }

        return axis;
    }

    public void ReverseRoom(bool state)
    {
        isRoomReversed = state;
    }

    public void LavaVFX(bool activate)
    {
        if (activate)
        {
            spriteRenderer.color = new Color(1f, 0.3915094f, 0.3915094f);

            if (!fireVFX.isPlaying)
            {
                fireVFX.Play();
            }
        }
        else
        {
            spriteRenderer.color = Color.white;

            if (fireVFX.isPlaying)
            {
                fireVFX.Stop();
            }
        }
    }

    public void PlayerCrown()
    {
        cursed = true;

        references.GetCurseDisplay().EnableCurse(true);

        Instantiate(crownVFX, new Vector2(transform.position.x, transform.position.y + 0.833f), Quaternion.identity);

        animator.runtimeAnimatorController = crownAnimator;
    }

    public bool GetCursed()
    {
        return cursed;
    }

    public Collider2D GetBodyCollider()
    {
        return bodyCollider;
    }

    public Collider2D GetFeetCollider()
    {
        return feetCollider;
    }

    public Rigidbody2D GetRigidbody2D()
    {
        return rb;
    }

    public bool GetIsInActivity()
    {
        return isInActivity;
    }

    public void SlowDown(bool spiderSlowDown)
    {
        moveSpeed = 3;

        if (spiderSlowDown)
        {
            Invoke(nameof(EndSlowDown), slowDownTime);
        }
    }

    public void EndSlowDown()
    {
        moveSpeed = 6;
    }

    public void SetIsInActivity(bool state)
    {
        isInActivity = state;
    }

    public void EnableRoll()
    {
        disableRoll = false;
    }

    public void EnableJump()
    {
        disableJump = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (feetCollider.IsTouchingLayers(LayerMask.GetMask("Hazards")) || bodyCollider.IsTouchingLayers(LayerMask.GetMask("Hazards")))
        {
            float damage = Random.Range(4, 8) * (1 + references.GetGameManager().GetRoomClearMultiplier(false));

            if (collision.CompareTag("BleedingHazards"))
            {
                references.GetPlayerHealth().TakeDamage(damage, false, false, true, true, true);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (feetCollider.IsTouchingLayers(LayerMask.GetMask("Hazards")) || bodyCollider.IsTouchingLayers(LayerMask.GetMask("Hazards")))
        {
            float damage = Random.Range(4, 8) * (1 + references.GetGameManager().GetRoomClearMultiplier(false));

            if (collision.CompareTag("BleedingHazards"))
            {
                references.GetPlayerHealth().TakeDamage(damage, false, false, true, true, true);
            }
        }
    }

    public void UpdateStats()
    {
        moveSpeed = references.GetPlayerStatistics().GetMoveSpeed();
    }
}

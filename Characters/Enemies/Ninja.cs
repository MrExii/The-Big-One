using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ninja : EnemyController
{
    [Header("Ninja Settings")]
    [SerializeField] RuntimeAnimatorController[] skins;
    [SerializeField] BoxCollider2D walkingZone;
    [SerializeField] BoxCollider2D feetCollider;
    [SerializeField] float idleSpeed;
    [SerializeField] float timeBetweenAttack;
    [SerializeField] float chaseDistance;
    [SerializeField] GameObject smokeVFX;

    Rigidbody2D rb;
    float timeSinceLastAttack = Mathf.Infinity;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        AddEnemyToList(gameObject);
    }

    private void OnEnable()
    {
        int animatorIndex = Random.Range(0, skins.Length);

        GetAnimator().runtimeAnimatorController = skins[animatorIndex];
    }

    private void Update()
    {
        if (GetEnemyHealth().GetIsDead() && !rb.isKinematic)
        {
            rb.isKinematic = true;

            return;
        }

        CheckDistance();

        if (rb.velocity.y < 0)
        {
            GetAnimator().SetBool("fall", true);
            GetAnimator().SetBool("run", false);
        }
        else
        {
            GetAnimator().SetBool("fall", false);
        }

        timeSinceLastAttack += Time.deltaTime;
    }

    private void CheckDistance()
    {
        if (chaseDistance > GetDistance() && !GetCanAttack())
        {
            SetCanAttack(true);
        }
    }

    public override void Attack()
    {
        if (timeSinceLastAttack > timeBetweenAttack)
        {
            timeSinceLastAttack = 0f;

            GetAnimator().SetTrigger("attack");
        }
        else
        {
            GetAnimator().SetBool("run", false);
        }

        FlipSprite(GetTarget().transform.position.x);
    }

    //Use in attack animation
    public void Teleport()
    {
        if (GetTarget().GetComponentInChildren<SpriteRenderer>().flipX)
        {
            transform.position = new(GetTarget().transform.position.x + 1f, GetTarget().transform.position.y);
        }
        else
        {
            transform.position = new(GetTarget().transform.position.x + -1f, GetTarget().transform.position.y);
        }

        Instantiate(smokeVFX, new Vector2(transform.position.x, transform.position.y + 1f), Quaternion.identity);

        FlipSprite(GetTarget().transform.position.x);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chaseDistance);
    }
#endif
}

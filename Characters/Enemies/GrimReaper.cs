using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrimReaper : EnemyController
{
    [Header("Grim Reaper Settings")]
    [SerializeField] float chaseDistance;
    [SerializeField] float attackSpeed;
    [SerializeField] float timeBetweenAttack;

    float timeSinceLastAttack = Mathf.Infinity;

    private void Start()
    {
        AddEnemyToList(gameObject);
    }

    private void Update()
    {
        if (GetIsStrongRoots())
        {
            SetCanAttack(true);

            return;
        }

        CheckDistance();

        timeSinceLastAttack += Time.deltaTime;
    }

    private void CheckDistance()
    {
        if (GetDistance() < chaseDistance)
        {
            SetCanAttack(true);
        }
    }

    public override void Idle()
    {
        if (GetSpriteRenderer().color.a != 0.5f)
        {
            if (GetTarget().GetComponent<PlayerHealth>().isDead) return;

            GetSpriteRenderer().color = new Color(GetSpriteRenderer().color.r, GetSpriteRenderer().color.g, GetSpriteRenderer().color.b, 0.5f);
        }

        FlipSprite(GetTarget().transform.position.x);

        GetAnimator().SetBool("move", false);
    }

    public override void Attack()
    {
        GetSpriteRenderer().color = new Color(GetSpriteRenderer().color.r, GetSpriteRenderer().color.g, GetSpriteRenderer().color.b, 1f);

        if (timeBetweenAttack < timeSinceLastAttack)
        {
            transform.position = Vector2.MoveTowards(transform.position, GetTarget().transform.position, Time.deltaTime * attackSpeed);

            GetAnimator().SetBool("move", true);
            GetAnimator().ResetTrigger("attack");

            if (GetDistance() < 0.8f)
            {
                GetAnimator().SetBool("move", false);
                GetAnimator().SetTrigger("attack");

                timeSinceLastAttack = 0f;
            }
        }

        FlipSprite(GetTarget().transform.position.x);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chaseDistance);
    }
#endif
}

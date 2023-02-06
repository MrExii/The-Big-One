using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snake : EnemyController
{
    [Header("Snake Settings")]
    [SerializeField] BoxCollider2D aggroZone;
    [SerializeField] float attackSpeed;
    [SerializeField] float timeBetweenAttack;
    [SerializeField] float timeBeforeHiding;

    float timeSinceLastAttack = Mathf.Infinity;
    float timeSinceLastHide;
    float startYPosition;

    private void Start()
    {
        AddEnemyToList(gameObject);

        startYPosition = transform.position.y;
    }

    private void Update()
    {
        CheckAggroZone();
    }

    public void CheckAggroZone()
    {
        if (aggroZone.IsTouching(GetPlayerController().GetBodyCollider()))
        {
            SetCanAttack(true);
        }
        else
        {
            SetCanAttack(false);
        }
    }

    public override void Idle()
    {
        if (timeSinceLastHide > timeBeforeHiding)
        {
            GetAnimator().SetBool("hide", true);

            timeSinceLastHide = 0f;
        }

        GetAnimator().SetBool("move", false);

        timeSinceLastHide += Time.deltaTime;
    }

    public override void Attack()
    {
        timeSinceLastHide = 0f;

        GetAnimator().SetBool("hide", false);

        if (GetDistance() < 1.2f && timeBetweenAttack < timeSinceLastAttack && !GetPlayerController().isRolling)
        {
            GetAnimator().SetBool("move", false);
            GetAnimator().SetTrigger("attack");

            timeSinceLastAttack = 0f;
        }
        else if ((GetDistance() > 1.2f || GetDistance() < 1.2f && GetPlayerController().isRolling) && timeBetweenAttack < timeSinceLastAttack)
        {
            SetEnableActivity(true);

            GetAnimator().SetBool("move", true);

            Vector2 targetPosition = new(GetTarget().transform.position.x, startYPosition);

            transform.position = Vector2.MoveTowards(transform.position, targetPosition, Time.deltaTime * attackSpeed);

            FlipSprite(GetTarget().transform.position.x);
        }
        else
        {
            GetAnimator().SetBool("move", false);
            GetAnimator().SetBool("hide", true);
        }

        timeSinceLastAttack += Time.deltaTime;
    }

    //Call on the hide animation
    private void HideEvent()
    {
        SetEnableActivity(false);
    }
}

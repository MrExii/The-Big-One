using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scorpion : EnemyController
{
    [Header("Scorpion Settings")]
    [SerializeField] BoxCollider2D aggroZone;
    [SerializeField] float idleSpeed;
    [SerializeField] float attackSpeed;
    [SerializeField] float timeBetweenAttack;

    Vector2 nextPosition;

    float startYPosition;
    float timeSinceLastAttack = Mathf.Infinity;

    bool pauseIdle;

    private void Start()
    {
        AddEnemyToList(gameObject);

        startYPosition = transform.position.y;
        nextPosition = transform.position;
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
        GetAnimator().SetBool("move", false);

        if (pauseIdle) return;

        FlipSprite(nextPosition.x);

        if (Mathf.Round(transform.position.x * 100) / 100 == Mathf.Round(nextPosition.x * 100) / 100)
        {
            float nextXPosition;
            int doPause = Random.Range(0, 2);

            if (doPause == 0)
            {
                pauseIdle = true;

                int pauseTime = Random.Range(2, 5);

                GetAnimator().SetBool("move", false);

                Invoke(nameof(UnpauseIdle), pauseTime);
            }

            nextXPosition = RandomPointInBounds(aggroZone.bounds);

            nextPosition = new Vector2(nextXPosition, transform.position.y);
        }

        transform.position = Vector2.MoveTowards(transform.position, nextPosition, Time.deltaTime * idleSpeed);

        GetAnimator().SetBool("move", true);
    }

    public override void Attack()
    {
        FlipSprite(GetTarget().transform.position.x);

        if (GetDistance() < 1.2f && timeBetweenAttack < timeSinceLastAttack && !GetPlayerController().isRolling)
        {
            GetAnimator().SetBool("move", false);
            GetAnimator().SetTrigger("attack");

            timeSinceLastAttack = 0f;
        }
        else if (GetDistance() > 1.2f && timeBetweenAttack < timeSinceLastAttack)
        {
            GetAnimator().SetBool("move", true);

            Vector2 targetPosition = new (GetTarget().transform.position.x, startYPosition);

            transform.position = Vector2.MoveTowards(transform.position, targetPosition, Time.deltaTime * attackSpeed);
        }
        else
        {
            GetAnimator().SetBool("move", false);
        }

        timeSinceLastAttack += Time.deltaTime;
    }

    private float RandomPointInBounds(Bounds bounds)
    {
        return Random.Range(bounds.min.x, bounds.max.x);
    }

    private void UnpauseIdle()
    {
        pauseIdle = false;
    }
}

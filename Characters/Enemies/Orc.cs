using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orc : EnemyController
{
    [Header("Orc Settings")]
    [SerializeField] RuntimeAnimatorController[] skins;
    [SerializeField] BoxCollider2D aggroZone;
    [SerializeField] float attackSpeed;
    [SerializeField] float idleSpeed;
    [SerializeField] float timeBetweenAttack;
    [SerializeField] AnimationClip attackAnimation;

    Vector2 nextPosition;
    Vector2 targetPosition;

    float startYPosition;
    float timeSinceLastAttack = Mathf.Infinity;

    bool pauseIdle;
    bool finishDash = true;
    bool block;

    private void Start()
    {
        AddEnemyToList(gameObject);

        startYPosition = transform.position.y;
        nextPosition = transform.position;
    }

    private void OnEnable()
    {
        int index = Random.Range(0, skins.Length);

        GetAnimator().runtimeAnimatorController = skins[index];
    }

    private void Update()
    {
        CheckWalkingZone();

        timeSinceLastAttack += Time.deltaTime;
    }

    private void CheckWalkingZone()
    {
        if (aggroZone.IsTouching(GetPlayerController().GetBodyCollider()))
        {
            SetCanAttack(true);
        }
        else if (finishDash)
        {
            SetCanAttack(false);
        }
    }

    public override void Idle()
    {
        if (Block()) return;

        GetAnimator().SetBool("run", false);

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

                GetAnimator().SetBool("run", false);

                Invoke(nameof(UnpauseIdle), pauseTime);
            }

            nextXPosition = RandomPointInBounds(aggroZone.bounds);

            nextPosition = new Vector2(nextXPosition, transform.position.y);
        }

        transform.position = Vector2.MoveTowards(transform.position, nextPosition, Time.deltaTime * idleSpeed);

        GetAnimator().SetBool("run", true);
    }

    public override void Attack()
    {
        if (timeBetweenAttack < timeSinceLastAttack)
        {
            if (Block()) return;

            finishDash = false;

            if (aggroZone.IsTouching(GetPlayerController().GetBodyCollider()))
            {
                targetPosition = GetTarget().transform.position;
            }

            Vector2 nextTargetPosition = new(targetPosition.x, transform.position.y);
            transform.position = Vector2.MoveTowards(transform.position, nextTargetPosition, Time.deltaTime * attackSpeed);

            GetAnimator().SetBool("run", true);
            GetAnimator().ResetTrigger("attack");
            
            if (Vector3.Distance(targetPosition, transform.position) < 1.5f)
            {
                GetAnimator().SetBool("run", false);

                if (GetDistance() < 1.5f)
                {
                    GetAnimator().SetTrigger("attack");
                }

                timeSinceLastAttack = 0f;

                pauseIdle = true;

                Invoke(nameof(UnpauseIdle), 2f);
            }

            FlipSprite(targetPosition.x);
        }
        else
        {
            finishDash = true;

            Idle();
        }
    }

    private bool Block()
    {
        if (GetPlayerController().isRolling && !block)
        {
            GetAnimator().SetBool("block", true);

            block = true;
        }
        else if (!GetPlayerController().isRolling && block)
        {
            GetAnimator().SetBool("block", false);

            block = false;
        }

        return block;
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

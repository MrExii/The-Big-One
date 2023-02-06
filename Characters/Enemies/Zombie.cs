using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : EnemyController
{
    [Header("Zombie Settings")]
    [SerializeField] RuntimeAnimatorController[] skins;
    [SerializeField] Collider2D aggroZone;
    [SerializeField] float attackSpeed;
    [SerializeField] float idleSpeed;
    [SerializeField] float timeBetweenAttack;
    [SerializeField] AnimationClip attackAnimation;
    [SerializeField] bool tutoZombie;

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

    private void OnEnable()
    {
        int index = Random.Range(0, skins.Length);

        GetAnimator().runtimeAnimatorController = skins[index];
    }

    private void Update()
    {
        CheckAggroZone();

        timeSinceLastAttack += Time.deltaTime;
    }

    private void CheckAggroZone()
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

    private float RandomPointInBounds(Bounds bounds)
    {
        return Random.Range(bounds.min.x, bounds.max.x);
    }

    public override void Attack()
    {
        if (tutoZombie)
        {
            GetAnimator().SetBool("run", false);
            FlipSprite(GetTarget().transform.position.x);
            return;
        }

        FlipSprite(GetTarget().transform.position.x);

        if (GetDistance() < 1f && timeBetweenAttack < timeSinceLastAttack)
        {
            GetAnimator().SetBool("run", false);
            GetAnimator().SetTrigger("attack");

            timeSinceLastAttack = 0f;
        }
        else if (GetDistance() > 1f && attackAnimation.length < timeSinceLastAttack)
        {
            GetAnimator().SetBool("run", true);

            Vector2 targetPosition = new(GetTarget().transform.position.x, startYPosition);

            transform.position = Vector2.MoveTowards(transform.position, targetPosition, Time.deltaTime * attackSpeed);
        }
        else
        {
            GetAnimator().SetBool("run", false);
        }
    }

    private void UnpauseIdle()
    {
        pauseIdle = false;
    }
}

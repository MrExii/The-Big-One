using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archer : EnemyController
{
    [Header("Archer Settings")]
    [SerializeField] RuntimeAnimatorController[] skins;
    [SerializeField] BoxCollider2D walkingZone;
    [SerializeField] BoxCollider2D fireZone;
    [SerializeField] float idleSpeed;
    [SerializeField] float timeBetweenAttack;

    Vector2 nextPosition;

    float startYPosition;
    float timeSinceLastAttack = Mathf.Infinity;
    float attackAnimationDuration;

    bool pauseIdle = false;

    private void Start()
    {
        AddEnemyToList(gameObject);

        GetAnimationClipDuration();

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
        CheckFireZone();
        CheckWalkingZone();

        timeSinceLastAttack += Time.deltaTime;
    }

    private void CheckFireZone()
    {
        if (fireZone.IsTouching(GetTarget().GetComponent<PlayerController>().GetBodyCollider()))
        {
            SetCanAttack(true);
        }
        else
        {
            SetCanAttack(false);
        }
    }

    private void CheckWalkingZone()
    {
        if (walkingZone.IsTouching(GetTarget().GetComponent<PlayerController>().GetBodyCollider()))
        {
            SetEnableActivity(true);
        }
        else
        {
            SetEnableActivity(false);
        }
    }

    public override void Idle()
    {
        if (attackAnimationDuration > timeSinceLastAttack) return;

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

            nextXPosition = RandomPointInBounds(walkingZone.bounds);

            nextPosition = new Vector2(nextXPosition, transform.position.y);
        }

        transform.position = Vector2.MoveTowards(transform.position, nextPosition, Time.deltaTime * idleSpeed);

        GetAnimator().SetBool("run", true);
    }

    private float RandomPointInBounds(Bounds bounds)
    {
        return Random.Range(bounds.min.x, bounds.max.x);
    }

    private void UnpauseIdle()
    {
        pauseIdle = false;
    }

    public override void Attack()
    {
        if (timeSinceLastAttack > timeBetweenAttack)
        {
            FlipSprite(GetTarget().transform.position.x);

            GetAnimator().SetBool("run", false);
            GetAnimator().SetTrigger("fire");

            timeSinceLastAttack = 0f;
        }
        else
        {
            GetAnimator().SetBool("run", false);
        }
    }

    private void GetAnimationClipDuration()
    {
        AnimationClip[] animationClips = GetAnimator().runtimeAnimatorController.animationClips;

        foreach (AnimationClip clip in animationClips)
        {
            if (clip.name == "Attack")
            {
                attackAnimationDuration = clip.length + 1f;
            }
        }
    }
}

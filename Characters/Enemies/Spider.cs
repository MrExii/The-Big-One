using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spider : EnemyController
{
    [Header("Spider Settings")]
    [SerializeField] BoxCollider2D walkingZone;
    [SerializeField] float moveSpeed;
    [SerializeField] bool wall;
    [SerializeField] float timeBetweenAttack;

    Activity[] activities;

    Vector2 nextPosition;

    bool pauseIdle = false;

    float timeSinceLastAttack;

    private void Start()
    {
        AddEnemyToList(gameObject);

        nextPosition = transform.position;

        if (GetSpriteRenderer().flipY)
        {
            GetBodyCollider().transform.localScale = new Vector3(1, -1, 1);
        }
    }

    private void Update()
    {
        if (GetIsStrongRoots())
        {
            SetCanAttack(true);
            SetEnableActivity(true);

            return;
        }

        if (!GetCanAttack())
        {
            SetCanAttack(true);
        }

        timeSinceLastAttack += Time.deltaTime;
    }

    public override void Attack()
    {
        GetAnimator().SetBool("run", false);

        if (!wall && timeSinceLastAttack > 2f)
        {
            Ground();
        }
        else if (timeSinceLastAttack > 2f)
        {
            Wall();
        }

        if (timeBetweenAttack < timeSinceLastAttack)
        {
            timeSinceLastAttack = 0f;

            FlipSprite(GetTarget().transform.position.x);

            GetAnimator().SetTrigger("attack");
        }

        ActivitiesCheck();
    }

    private void Ground()
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

            nextXPosition = RandomPointInBounds(walkingZone.bounds, false);

            nextPosition = new Vector2(nextXPosition, transform.position.y);
        }

        transform.position = Vector2.MoveTowards(transform.position, nextPosition, Time.deltaTime * moveSpeed);

        GetAnimator().SetBool("run", true);
    }

    private void Wall()
    {
        GetAnimator().SetBool("run", false);

        if (pauseIdle) return;

        FlipYSprite(-nextPosition.y);

        if (Mathf.Round(transform.position.y * 100) / 100 == Mathf.Round(nextPosition.y * 100) / 100)
        {
            float nextYPosition;
            int doPause = Random.Range(0, 2);

            if (doPause == 0)
            {
                pauseIdle = true;

                int pauseTime = Random.Range(2, 5);

                GetAnimator().SetBool("run", false);

                Invoke(nameof(UnpauseIdle), pauseTime);
            }

            nextYPosition = RandomPointInBounds(walkingZone.bounds, true);

            nextPosition = new Vector2(transform.position.x, nextYPosition);
        }

        transform.position = Vector2.MoveTowards(transform.position, nextPosition, Time.deltaTime * moveSpeed);

        GetAnimator().SetBool("run", true);
    }

    private bool ActivitiesCheck()
    {
        int i = 0;

        activities = FindObjectsOfType<Activity>();

        foreach (Activity activity in activities)
        {
            if (activity.GetIsActivityCompleted())
            {
                i++;
            }
        }
        
        if (i == activities.Length - 1)
        {
            SetEnableActivity(true);

            return true;
        }
        else
        {
            SetEnableActivity(false);

            return false;
        }
    }

    private float RandomPointInBounds(Bounds bounds, bool wall)
    {
        if (wall)
        {
            return Random.Range(bounds.min.y, bounds.max.y);
        }
        else
        {
            return Random.Range(bounds.min.x, bounds.max.x);
        }
    }

    private void UnpauseIdle()
    {
        pauseIdle = false;
    }

    public void FlipYSprite(float yPosition)
    {
        if (transform.position.y > yPosition)
        {
            GetSpriteRenderer().flipX = true;
        }
        else
        {
            GetSpriteRenderer().flipX = false;
        }
    }
}

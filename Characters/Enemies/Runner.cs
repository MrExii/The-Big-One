using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Runner : EnemyController
{
    [Header("Runner Settings")]
    [SerializeField] float moveSpeed;
    [SerializeField] Transform checkPoint;
    [SerializeField] BoxCollider2D aggroZone;

    Vector2 endPosition;

    bool switchSide = true;

    private void Start()
    {
        AddEnemyToList(gameObject);

        endPosition = transform.position + transform.InverseTransformPoint(checkPoint.position);

        SetCanAttack(true);
    }

    private void Update()
    {
        if (GetIsStrongRoots())
        {
            SetCanAttack(true);
            SetEnableActivity(true);

            return;
        }

        CheckAggroZone();
    }

    private void CheckAggroZone()
    {
        if (aggroZone.IsTouching(GetPlayerController().GetBodyCollider()))
        {
            SetEnableActivity(true);
        }
        else
        {
            SetEnableActivity(false);
        }
    }

    public override void Attack()
    {
        if (switchSide)
        {
            transform.position = Vector2.MoveTowards(transform.position, endPosition, Time.deltaTime * moveSpeed);

            FlipSprite(endPosition.x);

            if (transform.position.x == endPosition.x)
            {
                switchSide = false;
            }
        }
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, GetStartPosition(), Time.deltaTime * moveSpeed);

            FlipSprite(GetStartPosition().x);

            if (transform.position.x == GetStartPosition().x)
            {
                switchSide = true;
            }
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        endPosition = transform.position + transform.InverseTransformPoint(checkPoint.position);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, endPosition);
    }
#endif
}

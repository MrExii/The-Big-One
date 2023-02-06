using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bat : EnemyController
{
    [Header("Bat Settings")]
    [SerializeField] float attackSpeed;
    [SerializeField] float chaseDistance;

    private void Start()
    {
        AddEnemyToList(gameObject);
    }

    private void Update()
    {
        CheckDistance();
    }

    private void CheckDistance()
    {
        if (GetDistance() < chaseDistance)
        {
            SetCanAttack(true);
        }
    }

    public override void Attack()
    {
        GetAnimator().SetTrigger("move");

        FlipSprite(GetTarget().transform.position.x);

        transform.position = Vector2.MoveTowards(transform.position, GetTarget().transform.position, Time.deltaTime * attackSpeed);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chaseDistance);
    }
#endif
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gangster : EnemyController
{
    [Header("Gangster Settings")]
    [SerializeField] RuntimeAnimatorController[] skins;
    [SerializeField] BoxCollider2D aggroZone;
    [SerializeField] float timeBetweenAttack;

    float timeSinceLastAttack = Mathf.Infinity;

    private void Start()
    {
        AddEnemyToList(gameObject);

        SetCanAttack(true);
    }

    private void OnEnable()
    {
        int index = Random.Range(0, skins.Length);

        GetAnimator().runtimeAnimatorController = skins[index];
    }

    private void Update()
    {
        CheckAggroZone();

        FlipSprite(GetTarget().transform.position.x);

        timeSinceLastAttack += Time.deltaTime;
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
        if (timeSinceLastAttack > timeBetweenAttack)
        {
            timeSinceLastAttack = 0f;
            GetAnimator().SetTrigger("attack");
        }
    }

    //Call by attack event
    public void HitPlayer()
    {
        Vector2 direction;

        if (GetComponent<SpriteRenderer>().flipX)
        {
            direction = Vector2.right;
        }
        else
        {
            direction = Vector2.left;
        }

        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, direction, Mathf.Infinity);
        
        if (hits.Length > 0)
        {
            foreach (RaycastHit2D ray in hits)
            {
                if (ray.collider.CompareTag("Platform")) break;

                if (ray.collider.CompareTag("Player"))
                {
                    float damage = GetGenerateDamage().GetRandomDamage(GetAttackDamage(), false, false);
                    bool isCritical = GetGenerateDamage().GetIsCriticalDamage();

                    ray.collider.GetComponentInParent<PlayerHealth>().TakeDamage(damage, isCritical, true, true, true, true);
                }
            }
        }
    }
}

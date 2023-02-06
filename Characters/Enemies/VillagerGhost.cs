using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillagerGhost : EnemyController
{
    [Header("Villager Ghost")]
    [SerializeField] RuntimeAnimatorController[] skins;
    [SerializeField] float attackSpeed;

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

    public override void Attack()
    {
        FlipSprite(GetTarget().transform.position.x);

        transform.position = Vector2.MoveTowards(transform.position, GetTarget().transform.position, Time.deltaTime * attackSpeed);
    }
}

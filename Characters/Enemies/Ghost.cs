using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Ghost : EnemyController
{
    [Header("Ghost Settings")]
    [SerializeField] float attackSpeed;
    [SerializeField] float timeBetweenAttack;

    Keyboard keyboard;
    SpriteRenderer playerSpriteRenderer;

    const float fadeDuration = 2f;
    float timeSinceLastAttack = Mathf.Infinity;
    bool hiding;

    private void Start()
    {
        playerSpriteRenderer = GetTarget().GetComponentInChildren<SpriteRenderer>();

        GetSpriteRenderer().color = new Color(GetSpriteRenderer().color.r, GetSpriteRenderer().color.g, GetSpriteRenderer().color.b, 1);

        AddEnemyToList(gameObject);

        keyboard = GetComponentInChildren<Keyboard>();
    }

    private void Update()
    {
        if (GetIsStrongRoots())
        {
            GetSpriteRenderer().color = new Color(GetSpriteRenderer().color.r, GetSpriteRenderer().color.g, GetSpriteRenderer().color.b, 1);

            EnableActivity(true);

            return;
        }

        CheckIfPlayerLooking();

        timeSinceLastAttack += Time.deltaTime;
    }

    private void CheckIfPlayerLooking()
    {
        if ((!playerSpriteRenderer.flipX && !GetSpriteRenderer().flipX) ||
            playerSpriteRenderer.flipX && GetSpriteRenderer().flipX)
        {
            EnableActivity(false);

            if (GetSpriteRenderer().color.a != 0f && !hiding)
            {
                hiding = true;

                StartCoroutine(Hide());
            }

            return;
        }
        else
        {
            EnableActivity(true);

            StopAllCoroutines();

            hiding = false;
        }
    }

    private void EnableActivity(bool state)
    {
        SetCanAttack(state);

        SetEnableActivity(state);

        if (state && !GetEnemyHealth().GetIsDead())
        {
            GetComponent<EnemyHealth>().EnableHealthAndArmorTxt(state);
        }

        if (!state)
        {
            keyboard.DisableKeyboard();
        }
    }

    public override void Idle()
    {
        FlipSprite(GetTarget().transform.position.x);

        GetAnimator().SetBool("move", false);
    }

    public override void Attack()
    {
        GetSpriteRenderer().color = new Color(GetSpriteRenderer().color.r, GetSpriteRenderer().color.g, GetSpriteRenderer().color.b, 1);

        FlipSprite(GetTarget().transform.position.x);
        
        if (GetDistance() > 1.2f)
        {
            transform.position = Vector2.MoveTowards(transform.position, GetTarget().transform.position, Time.deltaTime * attackSpeed);

            GetAnimator().SetBool("move", true);
        }
        else if (GetDistance() < 1.2f && timeBetweenAttack < timeSinceLastAttack)
        {
            GetAnimator().SetBool("move", false);
            GetAnimator().SetTrigger("attack");

            timeSinceLastAttack = 0;
        }
        else
        {
            GetAnimator().SetBool("move", false);
        }
    }

    private IEnumerator Hide()
    {
        float alphaValue = GetSpriteRenderer().color.a;

        SetEnableActivity(false);

        GetComponent<EnemyHealth>().EnableHealthAndArmorTxt(false);

        while (GetSpriteRenderer().color.a > 0f)
        {
            alphaValue -= Time.deltaTime * fadeDuration;

            GetSpriteRenderer().color = new Color(GetSpriteRenderer().color.r, GetSpriteRenderer().color.g, GetSpriteRenderer().color.b, alphaValue);

            yield return null;
        }

        hiding = false;
    }
}

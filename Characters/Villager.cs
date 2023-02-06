using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Villager : MonoBehaviour
{
    [SerializeField] RuntimeAnimatorController[] skins;
    [SerializeField] BoxCollider2D walkingZone;
    [SerializeField] float moveSpeed;
    [SerializeField] Animator animator;
    [SerializeField] SpriteRenderer spriteRenderer;

    PlayerController playerController;

    Vector2 nextPosition;

    float startYPosition;

    bool pauseIdle;
    bool speakWithPlayer;

    private void Awake()
    {
        playerController = FindObjectOfType<PlayerController>();
    }

    private void Start()
    {
        startYPosition = spriteRenderer.transform.position.y;
        nextPosition = spriteRenderer.transform.position;

        int index = Random.Range(0, skins.Length);

        animator.runtimeAnimatorController = skins[index];
    }

    private void Update()
    {
        if (speakWithPlayer) return;

        Idle();
    }

    public void Idle()
    {
        animator.SetBool("move", false);

        if (pauseIdle) return;

        FlipSprite(nextPosition.x);

        if (Mathf.Round(spriteRenderer.transform.position.x * 100) / 100 == Mathf.Round(nextPosition.x * 100) / 100)
        {
            float nextXPosition;
            int doPause = Random.Range(0, 2);

            if (doPause == 0)
            {
                pauseIdle = true;

                int pauseTime = Random.Range(4, 9);

                animator.SetBool("move", false);

                Invoke(nameof(UnpauseIdle), pauseTime);
            }

            nextXPosition = RandomPointInBounds(walkingZone.bounds);

            nextPosition = new Vector2(nextXPosition, spriteRenderer.transform.position.y);
        }

        spriteRenderer.transform.position = Vector2.MoveTowards(spriteRenderer.transform.position, nextPosition, Time.deltaTime * moveSpeed);

        animator.SetBool("move", true);
    }

    private float RandomPointInBounds(Bounds bounds)
    {
        return Random.Range(bounds.min.x, bounds.max.x);
    }

    private void UnpauseIdle()
    {
        pauseIdle = false;
    }

    public void SpeakWithPlayer(bool state)
    {
        speakWithPlayer = state;

        animator.SetBool("move", false);

        if (!state) return;

        if (playerController.transform.position.x > transform.localPosition.x)
        {
            spriteRenderer.flipX = true;

            if (!playerController.GetComponentInChildren<SpriteRenderer>().flipX)
            {
                playerController.GetComponentInChildren<SpriteRenderer>().flipX = true;
            }
        }
        else
        {
            spriteRenderer.flipX = false;

            if (playerController.GetComponentInChildren<SpriteRenderer>().flipX)
            {
                playerController.GetComponentInChildren<SpriteRenderer>().flipX = false;
            }
        }
    }

    private void FlipSprite(float xPosition)
    {
        if (spriteRenderer.transform.position.x < xPosition)
        {
            spriteRenderer.flipX = true;
        }
        else
        {
            spriteRenderer.flipX = false;
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GlitchPickup : MonoBehaviour
{
    [SerializeField] Sprite[] glitchSprites;
    [SerializeField] float moveSpeed;
    [SerializeField] float minGlitch;
    [SerializeField] float maxGlitch;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] BoxCollider2D boxCollider;
    [SerializeField] AudioSource audioSource;

    CatchReferences references;

    float timeSinceSpawn;
    float timeBeforeMoving;
    int amountOfGlitch;

    private void Awake()
    {
        references = FindObjectOfType<CatchReferences>();
    }

    private void Start()
    {
        ChooseSkin();

        Physics2D.IgnoreCollision(boxCollider, references.GetPlayerController().GetBodyCollider());

        if (references.GetRoomPool())
        {
            transform.parent = references.GetRoomPool().GetCurrentRoom().GetPickupsPool().transform;
        }
        else
        {
            transform.parent = references.GetRoomPoolTest().GetCurrentRoom().GetPickupsPool().transform;
        }

        timeBeforeMoving = Random.Range(0.2f, 0.4f);

        amountOfGlitch = Random.Range(Mathf.CeilToInt(minGlitch), Mathf.CeilToInt(maxGlitch));
    }

    private void Update()
    {
        timeSinceSpawn += Time.deltaTime;

        if (timeBeforeMoving > timeSinceSpawn) return;

        MoveToPlayer();
    }

    private void ChooseSkin()
    {
        int index = Random.Range(0, glitchSprites.Length);

        spriteRenderer.sprite = glitchSprites[index];
    }

    private void MoveToPlayer()
    {
        rb.gravityScale = 0f;
        boxCollider.enabled = false;

        if (moveSpeed < 8)
        {
            moveSpeed += Time.deltaTime * (moveSpeed * 2);
        }

        transform.position = Vector2.MoveTowards(transform.position, references.GetPlayerController().transform.position, Time.deltaTime * moveSpeed);

        if (transform.position == references.GetPlayerController().transform.position)
        {
            references.GetGlitchDisplay().AddGlitch(amountOfGlitch);

            audioSource.Play();

            spriteRenderer.enabled = false;

            Destroy(gameObject); //destroy after audio played SFX
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<GlitchPickup>())
        {
            Physics2D.IgnoreCollision(boxCollider, collision.gameObject.GetComponent<BoxCollider2D>());
        }
    }
}

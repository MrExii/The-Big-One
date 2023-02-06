using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeakPlatform : MonoBehaviour
{
    [SerializeField] float timeToFall;
    [SerializeField] ParticleSystem debrisFalling;
    [SerializeField] ParticleSystem platformDebris;
    [SerializeField] Sprite[] weakPlaftormSprite;
    [SerializeField] Animator animator;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] BoxCollider2D boxCollider;
    [SerializeField] SpriteRenderer spriteRenderer;

    Vector3 startPosition;

    bool isFalling;

    private void Start()
    {
        startPosition = transform.position;

        int index = UnityEngine.Random.Range(0, weakPlaftormSprite.Length);

        spriteRenderer.sprite = weakPlaftormSprite[index];
    }

    private IEnumerator Falling()
    {
        debrisFalling.Play();

        isFalling = true;

        animator.SetTrigger("wiggle");

        yield return new WaitForSeconds(timeToFall);

        debrisFalling.Stop();

        rb.gravityScale = 1200f;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<WeakPlatform>())
        {
            Physics2D.IgnoreCollision(boxCollider, collision.gameObject.GetComponent<BoxCollider2D>());
        }

        if (collision.gameObject.CompareTag("Player"))
        {
            if (!isFalling)
            {
                StartCoroutine(Falling());
            }
        }

        if (collision.gameObject.CompareTag("Hazards") || boxCollider.IsTouchingLayers(LayerMask.GetMask("Ground")) && rb.gravityScale != 0)
        {
            EnableWeakPlatform(false);
        }
    }

    private void EnableWeakPlatform(bool state)
    {
        Instantiate(platformDebris, transform.position, Quaternion.identity, transform);

        GetComponentInChildren<SpriteRenderer>().enabled = state;
        boxCollider.enabled = state;

        if (state)
        {
            transform.position = startPosition;

            StopAllCoroutines();
        }

        isFalling = false;

        rb.gravityScale = 0f;

        animator.Rebind();
    }

    public void ReloadWeakPlatform()
    {
        EnableWeakPlatform(true);
    }
}

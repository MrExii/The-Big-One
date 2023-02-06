using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mushroom : MonoBehaviour
{
    [SerializeField] int bumpForce;
    [SerializeField] Animator animator;
    [SerializeField] AudioSource audioSource;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        if (collision.attachedRigidbody.velocity.y < -2)
        {
            animator.SetTrigger("bump");

            audioSource.Play();

            collision.attachedRigidbody.AddForce(new(0, bumpForce));
        }
    }
}

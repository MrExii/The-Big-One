using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Smear : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip[] attackSFX;
    [SerializeField] Animator animator;
    [SerializeField] SpriteRenderer spriteRenderer;

    AnimationClip smearClip;

    float timeToWait;
    float value;

    public void Start()
    {
        GetAnimationClip();

        int index = Random.Range(0, attackSFX.Length);

        audioSource.PlayOneShot(attackSFX[index]);

        if (attackSFX[index].length > smearClip.length)
        {
            timeToWait = attackSFX[index].length;
        }
        else
        {
            timeToWait = smearClip.length;
        }
    }

    private void Update()
    {
        value += Time.deltaTime;

        if (value > timeToWait)
        {
            Destroy(gameObject);
        }

        if (value > smearClip.length)
        {
            spriteRenderer.enabled = false;
        }
    }

    private void GetAnimationClip()
    {
        foreach (var clip in animator.runtimeAnimatorController.animationClips)
        {
            smearClip = clip;
        }
    }
}

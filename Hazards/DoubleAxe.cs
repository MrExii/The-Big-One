using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleAxe : MonoBehaviour
{
    [SerializeField] float timeBetweenSpikes;
    [SerializeField] float startDelay;
    [SerializeField] AnimationClip spikes;
    [SerializeField] Animator animator;

    float timeSinceLastAxe;
    float timeSinceStart;

    private void Update()
    {
        timeSinceStart += Time.deltaTime;

        if (timeSinceStart < startDelay) return;

        if (timeSinceLastAxe > timeBetweenSpikes)
        {
            timeSinceLastAxe = 0f;
            timeSinceLastAxe -= spikes.length;

            animator.SetTrigger("doubleAxe");
        }

        timeSinceLastAxe += Time.deltaTime;
    }
}

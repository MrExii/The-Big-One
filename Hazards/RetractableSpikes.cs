using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.UI;

public class RetractableSpikes : MonoBehaviour
{
    [SerializeField] float timeBetweenSpikes;
    [SerializeField] float startDelay;
    [SerializeField] AnimationClip spikes;
    [SerializeField] Animator animator;
    [SerializeField] PolygonCollider2D polygonCollider;

    float timeSinceLastSpikes;
    float timeSinceStart;

    private void Start()
    {
        polygonCollider.enabled = false;
    }

    private void Update()
    {
        timeSinceStart += Time.deltaTime;

        if (timeSinceStart < startDelay) return;

        if (timeSinceLastSpikes > timeBetweenSpikes)
        {
            timeSinceLastSpikes = 0f;
            timeSinceLastSpikes -= spikes.length;

            animator.SetTrigger("spikes");
        }

        timeSinceLastSpikes += Time.deltaTime;
    }

    //use in animation event
    public void EnableCollider()
    {
        polygonCollider.enabled = true;
    }

    //same as above
    public void DisableCollider()
    {
        polygonCollider.enabled = false;
    }
}

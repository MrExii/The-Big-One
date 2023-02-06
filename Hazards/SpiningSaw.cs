using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiningSaw : MonoBehaviour
{
    [SerializeField] float timeBetweenSpinning;
    [SerializeField] float spinningTime;
    [SerializeField] float startDelay;
    [SerializeField] bool halfSaw;
    [SerializeField] Sprite halfSawSprite;
    [SerializeField] Transform checkPoint;
    [SerializeField] float moveSpeed;
    [SerializeField] bool movingSaw;
    [SerializeField] Animator animator;
    [SerializeField] CircleCollider2D circleCollider;
    [SerializeField] SpriteRenderer spriteRenderer;

    Vector2 endPosition;
    Vector2 startPosition;

    float timeSinceLastSpinning;
    float timeBeforeStopSpinning;
    float timeSinceStart;

    bool switchSide = true;

    private void Awake()
    {
        if (halfSaw)
        {
            spriteRenderer.sprite = halfSawSprite;
        }
    }

    private void Start()
    {
        startPosition = transform.position;
        endPosition = transform.position + transform.InverseTransformPoint(checkPoint.position);

        if (movingSaw)
        {
            if (!halfSaw)
            {
                animator.SetBool("spinning", true);
            }
            else
            {
                animator.SetBool("halfSpinning", true);
            }
        }
    }

    private void Update()
    {
        timeSinceStart += Time.deltaTime;

        if (timeSinceStart < startDelay) return;

        if (movingSaw)
        {
            MovingSaw();
        }

        SawCollider();

        timeSinceLastSpinning += Time.deltaTime;
        timeBeforeStopSpinning += Time.deltaTime;
    }

    private void SawCollider()
    {
        if (timeSinceLastSpinning > timeBetweenSpinning)
        {
            timeSinceLastSpinning = 0f;
            timeBeforeStopSpinning = 0f;
            timeSinceLastSpinning -= spinningTime;

            if (!halfSaw)
            {
                animator.SetBool("spinning", true);
            }
            else
            {
                animator.SetBool("halfSpinning", true);
            }

            circleCollider.enabled = true;

        }

        if (timeBeforeStopSpinning > spinningTime)
        {
            if (!halfSaw)
            {
                animator.SetBool("spinning", false);
            }
            else
            {
                animator.SetBool("halfSpinning", false);
            }

            circleCollider.enabled = false;
        }
    }

    private void MovingSaw()
    {
        if (switchSide)
        {
            transform.position = Vector2.MoveTowards(transform.position, endPosition, Time.deltaTime * moveSpeed);

            if (transform.position.x == endPosition.x)
            {
                switchSide = false;
            }
        }
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, startPosition, Time.deltaTime * moveSpeed);

            if (transform.position.x == startPosition.x)
            {
                switchSide = true;
            }
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        endPosition = transform.position + transform.InverseTransformPoint(checkPoint.position);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, endPosition);
    }
#endif
}

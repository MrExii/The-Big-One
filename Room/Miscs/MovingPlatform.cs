using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] Transform[] waypoints;
    [SerializeField] GameObject plaftormWheel;
    [SerializeField] float wheelSpeed;
    [SerializeField] bool pauseBetweenWaypoint;
    [SerializeField] float timeBetweenWaypoint;
    [SerializeField] bool reversePlaftorm;

    Sector sector;

    List<Vector3> waypointsPosition = new();

    bool isFollowing;
    bool firstTravel = true;
    bool forward;

    private void Awake()
    {
        sector = FindObjectOfType<Sector>();
    }

    private void Start()
    {
        foreach (Transform item in waypoints)
        {
            waypointsPosition.Add(item.position);
        }
    }

    private void Update()
    {
        if (!isFollowing)
        {
            if (!reversePlaftorm)
            {
                StartCoroutine(FollowPath(waypointsPosition));
            }
            else
            {
                if (!firstTravel)
                {
                    waypointsPosition.Reverse();

                    forward = !forward;
                }

                StartCoroutine(FollowPath(waypointsPosition));

                firstTravel = false;
            }
        }
    }

    private IEnumerator FollowPath(List<Vector3> path)
    {
        isFollowing = true;

        for (int i = 0, y = 1; i < waypoints.Length; i++, y++)
        {
            if (y == waypoints.Length)
            {
                if (reversePlaftorm)
                {
                    break;
                }

                y = 0;
            }

            float travelPercente = 0f;

            Vector3 startPosition = path[i];
            Vector3 endPosition = path[y];

            while (travelPercente < 1)
            {
                travelPercente += Time.deltaTime;

                transform.position = Vector2.Lerp(startPosition, endPosition, travelPercente);

                if (forward)
                {
                    plaftormWheel.transform.Rotate(0, 0, Time.deltaTime * wheelSpeed);
                }
                else
                {
                    plaftormWheel.transform.Rotate(0, 0, Time.deltaTime * -wheelSpeed);
                }

                yield return new WaitForEndOfFrame();
            }

            if (pauseBetweenWaypoint)
            {
                yield return new WaitForSeconds(timeBetweenWaypoint);
            }
        }

        isFollowing = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;

        for (int i = 0, y = 1; i < waypoints.Length; i++, y++)
        {
            Gizmos.DrawSphere(waypoints[i].position, 0.1f);

            if (y == waypoints.Length)
            {
                if (reversePlaftorm)
                {
                    break;
                }

                y = 0;
            }

            Gizmos.DrawLine(waypoints[i].position, waypoints[y].position);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (GetComponent<Collider2D>().IsTouching(collision.gameObject.GetComponent<PlayerController>().GetFeetCollider()))
        {
            collision.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(sector.transform);
        }
    }
}

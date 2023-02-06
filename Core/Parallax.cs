using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Parallax : MonoBehaviour
{
    [SerializeField] Collider2D[] teleportationColliders;
    [SerializeField] Transform[] tilemaps;

    PlayerController playerController;

    int rightEntry;
    int leftEntry;

    bool isRightEntry;
    bool isLeftEntry;

    float timeSinceRightEntry;
    float timeSinceLeftEntry;

    private void Awake()
    {
        playerController = FindObjectOfType<PlayerController>();
    }

    private void Update()
    {
        if (playerController.GetIsInActivity()) return;
        if (playerController.disableControl) return;

        CheckTeleportationPoint();

        if (playerController.GetHorizontalAxis() > 0)
        {
            Vector3 translation = new(-0.25f * Time.deltaTime, 0, 0);

            transform.Translate(translation);
        }
        else if(playerController.GetHorizontalAxis() < 0)
        {
            Vector3 translation = new(0.25f * Time.deltaTime, 0, 0);

            transform.Translate(translation);
        }

        UpdateTimers();
    }

    private void UpdateTimers()
    {
        timeSinceRightEntry += Time.deltaTime;
        timeSinceLeftEntry += Time.deltaTime;

        if (timeSinceRightEntry > 0.1f && isRightEntry)
        {
            isRightEntry = false;
        }
        else if (timeSinceLeftEntry > 0.1f && isLeftEntry)
        {
            isLeftEntry = false;
        }
    }

    private void CheckTeleportationPoint()
    {
        if (playerController.GetBodyCollider().IsTouching(teleportationColliders[0]) && !isLeftEntry)
        {
            isLeftEntry = true;
            timeSinceLeftEntry = 0f;

            if (leftEntry == 0)
            {
                tilemaps[2].position = new(tilemaps[2].position.x - 80, 0);
            }
            else if (leftEntry == 1 || leftEntry == -1)
            {
                tilemaps[0].position = new(tilemaps[0].position.x - 80, 0);
            }

            tilemaps[1].position = new(tilemaps[1].position.x - 40, 0);

            teleportationColliders[0].transform.position = new(teleportationColliders[0].transform.position.x - 40, -1.5f);
            teleportationColliders[1].transform.position = new(teleportationColliders[1].transform.position.x - 40, -1.5f);

            leftEntry++;
            rightEntry--;

            if (leftEntry > 1)
            {
                leftEntry = 0;
            }

            if (rightEntry < -1)
            {
                rightEntry = 0;
            }
        }
        else if (playerController.GetBodyCollider().IsTouching(teleportationColliders[1]) && !isRightEntry)
        {
            isRightEntry = true;
            timeSinceRightEntry = 0f;

            if (rightEntry == 0)
            {
                tilemaps[0].position = new(tilemaps[0].position.x + 80, 0);
            }
            else if (rightEntry == 1 || rightEntry == -1)
            {
                tilemaps[2].position = new(tilemaps[2].position.x + 80, 0);
            }

            tilemaps[1].position = new(tilemaps[1].position.x + 40, 0);

            teleportationColliders[0].transform.position = new(teleportationColliders[0].transform.position.x + 40, -1.5f);
            teleportationColliders[1].transform.position = new(teleportationColliders[1].transform.position.x + 40, -1.5f);

            rightEntry++;
            leftEntry--;

            if (rightEntry > 1)
            {
                rightEntry = 0;
            }

            if (leftEntry < -1)
            {
                leftEntry = 0;
            }
        }
    }
}

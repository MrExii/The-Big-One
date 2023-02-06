using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour
{
    [SerializeField] Transform otherTeleport;
    [SerializeField] GameObject pressKey;

    CatchReferences references;
    PlayerInputSystem inputActions;

    private void Awake()
    {
        references = FindObjectOfType<CatchReferences>();

        inputActions = new();
        inputActions.PressKey.Teleport.performed += ctx => OnTeleport();

        inputActions.PressKey.Enable();
    }

    private void OnDestroy()
    {
        inputActions.PressKey.Disable();
    }

    private void Start()
    {
        pressKey.SetActive(false);
    }

    private void OnTeleport()
    {
        if (references.GetPlayerController().GetIsInActivity()) return;
        if (!pressKey.activeInHierarchy) return;

        StartCoroutine(StartTeleport());
    }

    private IEnumerator StartTeleport()
    {
        yield return pressKey.GetComponent<PressKey>().SwitchKeySprites();

        references.GetPlayerController().transform.position = otherTeleport.position;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            pressKey.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            pressKey.SetActive(false);
        }
    }
}

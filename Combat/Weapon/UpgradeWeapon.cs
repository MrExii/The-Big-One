using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeWeapon : MonoBehaviour
{
    [SerializeField] GameObject pressKey;
    [SerializeField] SpriteRenderer highlight;
    [SerializeField] AudioSource audioSource;

    Fighter fighter;
    PlayerInputSystem inputActions;

    private void Awake()
    {
        fighter = FindObjectOfType<Fighter>();

        inputActions = new();
        inputActions.PressKey.UpgradeWeapon.performed += ctx => OnUpgradeWeapon();

        inputActions.Enable();
    }

    private void Start()
    {
        if (fighter.GetIsWeaponUpgraded())
        {
            Destroy(gameObject);
        }
        else
        {
            pressKey.SetActive(false);
        }
    }

    private void OnUpgradeWeapon()
    {
        if (!pressKey) return;
        if (!pressKey.activeInHierarchy) return;

        inputActions.Disable();

        if (highlight)
        {
            highlight.enabled = false;
        }

        fighter.UpgradeWeapon(true);

        audioSource.Play();

        Destroy(pressKey);

        Invoke(nameof(DisableGameObject), audioSource.clip.length);
    }

    private void DisableGameObject()
    {
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!pressKey) return;

        pressKey.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!pressKey) return;

        pressKey.SetActive(false);
    }
}

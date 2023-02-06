using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] WeaponType weaponType;
    [SerializeField] float powerRecharge;
    [SerializeField] float epicPowerRecharge;
    [SerializeField] GameObject pressKey;
    [SerializeField] AnimationClip disappearAnimation;
    [SerializeField] SpriteRenderer lightSprite;
    [SerializeField] Animator animator;
    [SerializeField] AudioSource takenAS;

    CatchReferences references;
    PlayerInputSystem inputActions;
    TextInfoSpawner textInfoSpawner;

    bool isWeaponEpic;
    bool weaponTaken;

    private void Awake()
    {
        references = FindObjectOfType<CatchReferences>();
        textInfoSpawner = GetComponentInParent<TextInfoSpawner>();

        inputActions = new();
        inputActions.PressKey.TakeWeapon.performed += ctx => OnTakeWeapon();

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

    private void Update()
    {
        lightSprite.transform.Rotate(new(0, 0, 90 * Time.deltaTime));
    }

    private void OnTakeWeapon()
    {
        if (weaponTaken) return;
        if (!pressKey.activeInHierarchy) return;
        if (references.GetPauseMenu().GetInPauseMenu()) return;

        StartCoroutine(FadeLightSprite());

        weaponTaken = true;

        references.GetFighter().SetWeaponType(weaponType, powerRecharge, epicPowerRecharge);

        if (isWeaponEpic)
        {
            references.GetFighter().UpgradeWeapon(false);
        }

        animator.SetTrigger("disappear");

        takenAS.Play();

        textInfoSpawner.SpawnWeaponName(weaponType.ToString(), isWeaponEpic);

        references.GetWeaponDisplay().SetWeapon(weaponType, isWeaponEpic);

        StartCoroutine(DisableGameObject());
    }

    private IEnumerator FadeLightSprite()
    {
        float alphaValue = lightSprite.color.a;

        while (alphaValue > 0)
        {
            alphaValue -= Time.deltaTime;

            lightSprite.color = new(lightSprite.color.r, lightSprite.color.g, lightSprite.color.b, alphaValue);

            yield return null;
        }
    }

    private IEnumerator DisableGameObject()
    {
        yield return pressKey.GetComponent<PressKey>().SwitchKeySprites();

        Destroy(pressKey);

        yield return new WaitForSeconds(disappearAnimation.length);

        GetComponentInChildren<SpriteRenderer>().enabled = false;
    }

    public void SetWeaponEpic()
    {
        isWeaponEpic = true;

        lightSprite.GetComponent<SpriteRenderer>().color = new(1, 0.7098039f, 0, 0.7f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (weaponTaken) return;

        pressKey.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (weaponTaken) return;

        pressKey.SetActive(false);
    }
}

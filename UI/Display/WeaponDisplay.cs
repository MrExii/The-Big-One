using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponDisplay : MonoBehaviour
{
    [System.Serializable]
    private class WeaponSettings
    {
        public WeaponType weaponType;
        public Sprite weaponSprite;
        public Sprite epicWeaponSprite;
        public Sprite chargeWeaponSprite;
        public Sprite epicChargeWeaponSprite;
    }

    [SerializeField] Image weaponImg;
    [SerializeField] AnimationClip brokenSwordAnimation;
    [SerializeField] AnimationClip weaponUpgradeClip;
    [SerializeField] Animator animator;
    [SerializeField] Animator usableAnimator;
    [SerializeField] WeaponSettings[] weaponSettings;

    //This function is used to avoid the coroutine to stop when the weapon being destroy when we switch room to early in the animation
    public void SetWeapon(WeaponType weaponType, bool isEpic)
    {
        StartCoroutine(SwitchWeaponSprite(weaponType, isEpic));
    }

    public IEnumerator SwitchWeaponSprite(WeaponType weaponType, bool isEpic)
    {
        animator.enabled = true;

        yield return new WaitForSeconds(0.2f);

        animator.SetTrigger("BrokenSwordDisappear");

        yield return new WaitForSeconds(brokenSwordAnimation.length + 0.2f);

        if (isEpic)
        {
            animator.SetTrigger(weaponType.ToString() + "EpicAppear");
        }
        else
        {
            animator.SetTrigger(weaponType.ToString() + "Appear");
        }

        yield return new WaitForSeconds(brokenSwordAnimation.length);

        animator.enabled = false;
    }

    public IEnumerator SetWeaponEpic(WeaponType weaponType)
    {
        animator.enabled = true;

        animator.SetTrigger(weaponType.ToString() + "Upgrade");

        yield return new WaitForSeconds(weaponUpgradeClip.length);

        animator.enabled = false;
    }

    public void CanUseWeapon(bool isCharged, WeaponType weaponType, bool isWeaponUpgraded)
    {
        if (isCharged)
        {
            foreach (var weapon in weaponSettings)
            {
                if (weaponType == weapon.weaponType)
                {
                    if (isWeaponUpgraded)
                    {
                        weaponImg.sprite = weapon.epicChargeWeaponSprite;
                    }
                    else
                    {
                        weaponImg.sprite = weapon.chargeWeaponSprite;
                    }
                }
            }
        }
        else
        {
            foreach (var weapon in weaponSettings)
            {
                if (weaponType == weapon.weaponType)
                {
                    if (isWeaponUpgraded)
                    {
                        weaponImg.sprite = weapon.epicWeaponSprite;
                    }
                    else
                    {
                        weaponImg.sprite = weapon.weaponSprite;
                    }
                }
            }
        }

        if (weaponType == WeaponType.BrokenSword && !isWeaponUpgraded) return;

        usableAnimator.SetBool("usable", isCharged);
    }
}

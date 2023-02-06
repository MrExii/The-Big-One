using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;

public class TextInfo : MonoBehaviour
{
    [SerializeField] Color damageColor;
    [SerializeField] Color curseDamageColor;
    [SerializeField] Color criticalDamageColor;
    [SerializeField] Color healColor;
    [SerializeField] Color weaponColor;
    [SerializeField] TextMeshProUGUI infoTxt;
    [SerializeField] Animator animator;

    public void SetDamageTextInfo(float damage, bool isCriticalDamage, bool curseDamage)
    {
        if (curseDamage)
        {
            infoTxt.text = "+" + Mathf.RoundToInt(damage).ToString();
        }
        else
        {
            infoTxt.text = "-" + Mathf.RoundToInt(damage).ToString();
        }

        if (isCriticalDamage)
        {
            infoTxt.color = criticalDamageColor;
            animator.SetTrigger("criticalDamage");
        }
        else
        {
            if (curseDamage)
            {
                infoTxt.color = curseDamageColor;
            }
            else
            {
                infoTxt.color = damageColor;
            }

            int i = Random.Range(0, 2);

            if (i == 0)
            {
                animator.SetBool("damage", true);
            }
            else
            {
                animator.SetBool("damage", false);
            }
        }
    }

    public void SetHealTextInfo(float heal)
    {
        infoTxt.text = "+" + Mathf.RoundToInt(heal).ToString();

        animator.SetTrigger("heal");

        infoTxt.color = healColor;
    }

    public void SetWeaponTextInfo(string weaponName, bool isEpic)
    {
        var newWeaponName = Regex.Replace(weaponName, "([a-z])([A-Z])", "$1 $2");

        if (isEpic)
        {
            newWeaponName += "+";

            animator.SetTrigger("criticalDamage");

            infoTxt.color = criticalDamageColor;
        }
        else
        {
            animator.SetTrigger("heal");

            infoTxt.color = weaponColor;
        }

        infoTxt.text = newWeaponName;
    }

    public void SetPNJBonusTextInfo(string bonusName, Color textColor)
    {
        var newBonusName = Regex.Replace(bonusName, "([a-z])([A-Z])", "$1 $2");

        infoTxt.text = newBonusName;
        infoTxt.color = textColor;

        animator.SetTrigger("heal");
    }
}

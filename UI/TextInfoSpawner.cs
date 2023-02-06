using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextInfoSpawner : MonoBehaviour
{
    [SerializeField] GameObject textInfoPrefab;
    [SerializeField] BoxCollider2D boxCollider;

    public void SpawnDamage(float damage, bool isCriticalDamage, bool curseDamage)
    {
        if (damage <= 0 || damage == Mathf.Infinity) return;

        SpawnTextInfo().SetDamageTextInfo(damage, isCriticalDamage, curseDamage);
    }

    public void SpawnHeal(float heal)
    {
        if (heal <= 0) return;
        if (heal == Mathf.Infinity) return;

        SpawnTextInfo().SetHealTextInfo(heal);
    }

    public void SpawnWeaponName(string weaponName, bool isEpic)
    {
        if (string.IsNullOrEmpty(weaponName)) return;

        SpawnTextInfo().SetWeaponTextInfo(weaponName, isEpic);
    }

    public void SpawnNPCBonus(string bonusName, Color textColor)
    {
        if (string.IsNullOrEmpty(bonusName)) return;

        SpawnTextInfo().SetPNJBonusTextInfo(bonusName, textColor);
    }

    private TextInfo SpawnTextInfo()
    {
        TextInfo textInfo = Instantiate(textInfoPrefab, RandomPointInBounds(boxCollider.bounds), Quaternion.identity, transform).GetComponentInChildren<TextInfo>();

        return textInfo;
    }

    private Vector2 RandomPointInBounds(Bounds bounds)
    {
        return new Vector2(Random.Range(bounds.min.x, bounds.max.x), Random.Range(bounds.min.y, bounds.max.y));
    }
}

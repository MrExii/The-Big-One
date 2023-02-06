using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PressKey : MonoBehaviour
{
    [System.Serializable]
    private class WeaponSprite
    {
        public WeaponType weaponType;
        public Sprite weaponSprite;
        public Sprite epicWeaponSprite;
    }

    [SerializeField] GameObject keyboardPressKey;
    [SerializeField] GameObject controllerPressKey;
    [SerializeField] Image keyboardKeyImg;
    [SerializeField] Sprite[] keyboardKeySprites;
    [SerializeField] Image controllerKeyImg;
    [SerializeField] Sprite[] controllerKeySprites;
    [SerializeField] Image[] weaponImgs;
    [SerializeField] WeaponSprite[] weaponSprites;

    CatchReferences references;
    EnemyController enemyController;

    const float timeBetweenKeySpriteSwap = 0.05f;

    private void Awake()
    {
        references = FindObjectOfType<CatchReferences>();
        enemyController = GetComponentInParent<EnemyController>();
    }

    private void Start()
    {
        keyboardPressKey.SetActive(false);
        controllerPressKey.SetActive(false);

        SetWeaponSprite(references.GetFighter().GetWeaponType());
    }

    private void Update()
    {
        RefreshUI();

        if (references.GetFighter().GetCanUsePower() && references.GetFighter().GetCanUseWeapon() && enemyController)
        {
            EnableWeaponImgs(true);
        }
        else
        {
            EnableWeaponImgs(false);
        }
    }

    private void RefreshUI()
    {
        if (!keyboardPressKey.activeInHierarchy && references.GetGameManager().GetKeyboardControl())
        {
            keyboardPressKey.SetActive(true);
            controllerPressKey.SetActive(false);

            LayoutRebuilder.ForceRebuildLayoutImmediate(keyboardPressKey.GetComponent<RectTransform>());
        }
        else if (!controllerPressKey.activeInHierarchy && !references.GetGameManager().GetKeyboardControl())
        {
            keyboardPressKey.SetActive(false);
            controllerPressKey.SetActive(true);

            LayoutRebuilder.ForceRebuildLayoutImmediate(controllerPressKey.GetComponent<RectTransform>());
        }

        if (references.GetGameManager().GetChangeUI())
        {
            controllerKeyImg.sprite = controllerKeySprites[2];
        }
        else
        {
            controllerKeyImg.sprite = controllerKeySprites[0];
        }
    }

    public void EnableWeaponImgs(bool state)
    {
        weaponImgs[0].gameObject.SetActive(state);
        weaponImgs[1].gameObject.SetActive(state);
    }

    public void SetWeaponSprite(WeaponType weaponType)
    {
        foreach (var weapon in weaponSprites)
        {
            if (weapon.weaponType == weaponType)
            {
                if (references.GetFighter().GetIsWeaponUpgraded())
                {
                    weaponImgs[0].sprite = weapon.epicWeaponSprite;
                    weaponImgs[1].sprite = weapon.epicWeaponSprite;
                }
                else
                {
                    weaponImgs[0].sprite = weapon.weaponSprite;
                    weaponImgs[1].sprite = weapon.weaponSprite;
                }
            }
        }
    }

    public IEnumerator SwitchKeySprites()
    {
        if (references.GetGameManager().GetKeyboardControl())
        {
            keyboardKeyImg.sprite = keyboardKeySprites[1];

            yield return new WaitForSeconds(timeBetweenKeySpriteSwap);

            keyboardKeyImg.sprite = keyboardKeySprites[0];
        }
        else
        {
            if (references.GetGameManager().GetChangeUI())
            {
                controllerKeyImg.sprite = controllerKeySprites[3];

                yield return new WaitForSeconds(timeBetweenKeySpriteSwap);

                controllerKeyImg.sprite = controllerKeySprites[2];
            }
            else
            {
                controllerKeyImg.sprite = controllerKeySprites[0];

                yield return new WaitForSeconds(timeBetweenKeySpriteSwap);

                controllerKeyImg.sprite = controllerKeySprites[1];
            }
        }
    }
}

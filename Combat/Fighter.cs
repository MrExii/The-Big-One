using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fighter : MonoBehaviour, IUpdateStats
{
    [SerializeField] WeaponType weaponType;
    [SerializeField] GenerateDamage generateDamage;
    [SerializeField] ChargeBar chargeBar;
    [SerializeField] bool blockWeaponPower;

    CatchReferences references;
    PlayerInputSystem inputActions;

    float baseDamage;
    float powerRecharge;
    float epicPowerRecharge;
    float timeSinceLastPowerUse = Mathf.Infinity;

    bool isWeaponUpgraded;
    bool canUsePower = true;
    bool canUseWeapon;

    private void Awake()
    {
        references = FindObjectOfType<CatchReferences>();

        inputActions = new();
        inputActions.Player.UseWeapon.performed += ctx => OnUseWeapon();
        inputActions.Player.Enable();
    }

    private void OnDisable()
    {
        inputActions.Player.Disable();
    }

    private void Start()
    {
        if (chargeBar)
        {
            chargeBar.WeaponUpdateChargeBar(timeSinceLastPowerUse, epicPowerRecharge); //Refresh Broken Sword recharge UI
        }

        SetWeaponType(WeaponType.BrokenSword, 0f, 8f);
    }

    private void Update()
    {
        if (!chargeBar) return;
        if (canUsePower)
        {
            if (!references.GetWeaponDisplay()) return;

            references.GetWeaponDisplay().CanUseWeapon(true, weaponType, isWeaponUpgraded);
            return;
        }
        else
        {
            references.GetWeaponDisplay().CanUseWeapon(false, weaponType, isWeaponUpgraded);
        }
        if ((references.GetRoomPool() && references.GetRoomPool().GetCurrentRoom().GetCanExitRoom()) 
            || (references.GetRoomPoolTest() && references.GetRoomPoolTest().GetCurrentRoom().GetCanExitRoom()) 
            || references.GetPlayerController().GetIsInActivity()) return;

        timeSinceLastPowerUse += Time.deltaTime;

        if (isWeaponUpgraded)
        {
            chargeBar.WeaponUpdateChargeBar(timeSinceLastPowerUse, epicPowerRecharge);

            if (timeSinceLastPowerUse > epicPowerRecharge)
            {
                canUsePower = true;
            }
        }
        else if (weaponType != WeaponType.BrokenSword)
        {
            chargeBar.WeaponUpdateChargeBar(timeSinceLastPowerUse, powerRecharge);

            if (timeSinceLastPowerUse > powerRecharge)
            {
                canUsePower = true;
            }
        }
    }

    private void OnUseWeapon()
    {
        if (blockWeaponPower) return;
        if (!canUsePower) return;
        if (weaponType == WeaponType.BrokenSword) return;
        if (references.GetPlayerController().GetIsInActivity()) return;
        if (references.GetPlayerController().disableControl) return;

        canUseWeapon = true;
    }

    public bool CanUsePower()
    {
        if (canUsePower && canUseWeapon)
        {
            canUsePower = false;
            canUseWeapon = false;

            timeSinceLastPowerUse = 0;
            
            return true;
        }
        else
        {
            return false;
        }
    }

    public void ResetCanUsePower()
    {
        canUsePower = true;
        timeSinceLastPowerUse = Mathf.Infinity;
    }

    public float GetDamage(bool forceCritical)
    {
        if (references.GetPassiveObject().Purification())
        {
            return Mathf.Infinity;
        }
        else
        {
            return generateDamage.GetRandomDamage(baseDamage, true, forceCritical);
        }
    }

    public void UpdateStats()
    {
        baseDamage = references.GetPlayerStatistics().GetBaseDamage();
    }

    public void UpgradeWeapon(bool isBlacksmithUpgrade)
    {
        isWeaponUpgraded = true;
        
        if (isBlacksmithUpgrade)
        {
            StartCoroutine(references.GetWeaponDisplay().SetWeaponEpic(weaponType));

            references.GetGameManager().IncreaseNumberOfWeaponsUpgraded();

            CheckAchievement();
        }
    }

    private void CheckAchievement()
    {
        if (references.GetGameManager().GetNoobMode()) return;
        if (references.GetGameManager().GetDisableSteamworks()) return;

        if (SteamManager.Initialized)
        {
            int numberOfWeaponsUpgraded = references.GetGameManager().GetNumberOfWeaponsUpgraded();

            if (numberOfWeaponsUpgraded == 25)
            {
                SteamUserStats.GetAchievement("ACH_WEAPONS_UPGRADED", out bool achievementUnlock);

                if (achievementUnlock)
                {
                    references.GetAchievements().SetAchievement("ACH_WEAPONS_UPGRADED");
                    SteamUserStats.SetAchievement("ACH_WEAPONS_UPGRADED");
                }
            }

            SteamUserStats.StoreStats();
        }
    }

    public bool GetIsWeaponUpgraded()
    {
        return isWeaponUpgraded;
    }

    public bool GetCanUsePower()
    {
        return canUsePower;
    }

    public bool GetCanUseWeapon()
    {
        return canUseWeapon;
    }

    public void SetWeaponType(WeaponType weaponType, float powerRecharge, float epicPowerRecharge)
    {
        this.weaponType = weaponType;
        this.powerRecharge = powerRecharge;
        this.epicPowerRecharge = epicPowerRecharge;
    }

    public WeaponType GetWeaponType()
    {
        return weaponType;
    }

    public void UnBlockWeaponPower()
    {
        blockWeaponPower = false;
    }
}

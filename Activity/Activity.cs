using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class Activity : MonoBehaviour
{
    [SerializeField] GameObject pressKey;
    [SerializeField] KeySeriesSO keySeries;
    [SerializeField] int experienceGain;
    [SerializeField] Keyboard enemyKeyboard;
    [SerializeField] KeyCode keyToLaunchActivity;
    [SerializeField] KeyIndex controllerButtonName;
    [SerializeField] bool isEnemy = true;

    CatchReferences references;
    SpawnGlitch spawnGlitch;
    EnemyController enemyController;
    PlayerInputSystem inputActions;

    bool enemyCanBeSelected;
    bool isActivityCompleted;
    bool alreadyPressKey = true;

    private void Awake()
    {
        references = FindObjectOfType<CatchReferences>();
        spawnGlitch = GetComponentInChildren<SpawnGlitch>();
        enemyController = GetComponent<EnemyController>();

        inputActions = new();
        inputActions.PressKey.StartActivity.performed += ctx => OnStartActivity();
        inputActions.PressKey.StartActivity.started += ctx => OnReleaseKey();
    }

    private void Start()
    {
        pressKey.SetActive(false);
    }

    private void OnEnable()
    {
        inputActions.PressKey.Enable();
    }

    private void OnDestroy()
    {
        inputActions.PressKey.Disable();
    }

    private void Update()
    {
        if (!enemyCanBeSelected && isEnemy) return;
        if (isActivityCompleted) return;
        if (references.GetPlayerHealth().isDead) return;

        if (Input.GetKeyDown(keyToLaunchActivity) && references.GetGameManager().GetKeyboardControl() && !references.GetPauseMenu().GetInPauseMenu())
        {
            StartCoroutine(EnableKeyboard());
        }

        if (!isEnemy && references.GetPlayerController().GetIsInActivity() && pressKey.activeInHierarchy)
        {
            pressKey.SetActive(false);
        }
    }

    public void EnemyCanBeSelected(bool state)
    {
        if (references.GetPlayerController().GetIsInActivity())
        {
            pressKey.SetActive(false);
            enemyCanBeSelected = false;
        }
        else
        {
            pressKey.SetActive(state);
            enemyCanBeSelected = state;
        }
    }

    private IEnumerator EnableKeyboard()
    {
        if (!isEnemy && !pressKey.activeInHierarchy) yield break;

        yield return pressKey.GetComponent<PressKey>().SwitchKeySprites();

        if (isEnemy)
        {
            yield return new WaitForSeconds(0.01f);

            enemyKeyboard.EnableKeyboard(keySeries.GetTimeToCompleteActivity(), keySeries.GetMinTimeToCompleteActivity());
            enemyKeyboard.StartActivity(this, keySeries);

            if (references.GetPlayerController().GetIsInActivity()) yield break;
        }
        else
        {
            pressKey.SetActive(false);

            references.GetPlayerKeyboard().EnableKeyboard(keySeries.GetTimeToCompleteActivity(), keySeries.GetMinTimeToCompleteActivity());
            references.GetPlayerKeyboard().StartActivity(this, keySeries);
        }
    }

    private void OnStartActivity()
    {
        if (!pressKey.activeInHierarchy) return;
        if (!enemyCanBeSelected && isEnemy) return;
        if (isActivityCompleted) return;
        if (references.GetPlayerHealth().isDead) return;
        if (!alreadyPressKey) return;
        if (references.GetPauseMenu().GetInPauseMenu()) return;

        if (controllerButtonName != KeyIndex.rightShoulder && Gamepad.current.rightShoulder.isPressed) return;
        if (controllerButtonName != KeyIndex.leftShoulder && Gamepad.current.leftShoulder.isPressed) return;
        else if (controllerButtonName != KeyIndex.buttonNorth && Gamepad.current.buttonNorth.isPressed) return;
        else if (controllerButtonName != KeyIndex.buttonSouth && Gamepad.current.buttonSouth.isPressed) return;
        else if (controllerButtonName != KeyIndex.buttonEast && Gamepad.current.buttonEast.isPressed) return;
        else if (controllerButtonName != KeyIndex.buttonWest && Gamepad.current.buttonWest.isPressed) return;

        alreadyPressKey = false;

        StartCoroutine(EnableKeyboard());
    }

    private void OnReleaseKey()
    {
        if (references.GetPlayerController().GetIsInActivity()) return;

        alreadyPressKey = true;
    }

    public void EndActivity()
    {
        if (isEnemy)
        {
            float damage;
            bool isCriticalDamage;

            if (references.GetFighter().GetWeaponType() == WeaponType.BrokenSword && references.GetFighter().GetIsWeaponUpgraded() && 
                references.GetFighter().CanUsePower())
            {
                damage = references.GetFighter().GetDamage(true);
                isCriticalDamage = true;
            }
            else
            {
                damage = references.GetFighter().GetDamage(false);
                isCriticalDamage = references.GetPlayerController().GetComponent<GenerateDamage>().GetIsCriticalDamage();
            }

            enemyController.GetComponent<EnemyHealth>().TakeDamage(damage, isCriticalDamage, true);

            if (!enemyController.GetComponent<EnemyHealth>().GetIsDead())
            {
                if (references.GetPassiveObject().BloodForge())
                {
                    references.GetPlayerHealth().Heal(references.GetPassiveObject().BloodForgeHealAmount());
                }

                return;
            }
        }

        ActivityReward();
    }

    private void ActivityReward()
    {
        if (!isEnemy)
        {
            FinishActivity();
        }
        else
        {
            spawnGlitch.Spawn();
        }

        references.GetPlayerHealth().StopBleeding();

        GenerateExperienceGain();
    }

    private void GenerateExperienceGain()
    {
        references.GetExperience().GainExperience(Mathf.RoundToInt(experienceGain));
    }

    public KeyCode GetKeyboardKey()
    {
        return keyToLaunchActivity;
    }

    public KeyIndex GetControllerKey()
    {
        return controllerButtonName;
    }

    public bool GetIsActivityCompleted()
    {
        return isActivityCompleted;
    }

    public void ResetActivity()
    {
        if (isActivityCompleted)
        {
            isActivityCompleted = false;
        }
    }

    public void FinishActivity()
    {
        isActivityCompleted = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isEnemy && !isActivityCompleted && !references.GetPlayerController().GetIsInActivity() && collision.CompareTag("Player"))
        {
            pressKey.SetActive(true);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!isEnemy && !isActivityCompleted && !references.GetPlayerController().GetIsInActivity() && collision.CompareTag("Player"))
        {
            pressKey.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!isEnemy && !isActivityCompleted || references.GetPlayerController().GetIsInActivity() && collision.CompareTag("Player"))
        {
            pressKey.SetActive(false);
        }
    }
}

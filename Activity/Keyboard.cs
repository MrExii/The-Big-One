using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.UI;

public class Keyboard : MonoBehaviour
{
    [SerializeField] GameObject keyboard;
    [SerializeField] Image normalOrReverseImg;
    [SerializeField] Sprite[] normalOrReverseSprites;
    [SerializeField] GameObject touchs;
    [SerializeField] AudioSource audioSource;
    [SerializeField] SpawnGlitch spawnGlitch;
    [SerializeField] Key[] keys;
    [SerializeField] ActivityTimer activityTimer;
    [SerializeField] bool randomKeySeries;

    CatchReferences references;
    EnemyHealth enemyHealth;
    PlayerInputSystem inputActions;
    MonoBehaviour instigator;
    EnemyController enemyController;

    List<KeyIndex> keySeriesTouchs;
    KeyIndex goodTouch;

    bool canUsePower;
    bool alreadyChecking;
    bool isReverseKeySeries;

    const float timeForKeySpriteChange = 0.07f;
    float timeSinceActivityStarted; //Use for debugging

    private void Awake()
    {
        references = FindObjectOfType<CatchReferences>();
        enemyHealth = GetComponentInParent<EnemyHealth>();
        enemyController = GetComponentInParent<EnemyController>();

        inputActions = new();
        //Keyboard
        inputActions.Player.A.performed += ctx => OnAPress();
        inputActions.Player.Z.performed += ctx => OnZPress();
        inputActions.Player.E.performed += ctx => OnEPress();
        inputActions.Player.R.performed += ctx => OnRPress();
        inputActions.Player.Q.performed += ctx => OnQPress();
        inputActions.Player.S.performed += ctx => OnSPress();
        inputActions.Player.D.performed += ctx => OnDPress();
        inputActions.Player.F.performed += ctx => OnFPress();
        inputActions.Player.W.performed += ctx => OnWPress();
        inputActions.Player.X.performed += ctx => OnXPress();
        inputActions.Player.C.performed += ctx => OnCPress();
        inputActions.Player.V.performed += ctx => OnVPress();

        //Controller
        inputActions.Player.ButtonNorth.performed += ctx => OnButtonNorthPress();
        inputActions.Player.ButtonSouth.performed += ctx => OnButtonSouthPress();
        inputActions.Player.ButtonWest.performed += ctx => OnButtonWestPress();
        inputActions.Player.ButtonEast.performed += ctx => OnButtonEastPress();
        inputActions.Player.LeftShoulder.performed += ctx => OnLeftShoulderPress();
        inputActions.Player.RightShoulder.performed += ctx => OnRightShoulderPress();
        inputActions.Player.LeftStickButton.performed += ctx => OnLeftStickButtonPress();
        inputActions.Player.RightStickButton.performed += ctx => OnRightStickButtonPress();
    }

    private void Start()
    {
        keyboard.SetActive(false); //Don't use DisableKeyboard. That's reset isInActivity to false when enemies are spawing

        if (references.GetSimulationsPlaceHolder().GetReverseKeySeries() && enemyHealth)
        {
            IsReverseKeySeries();

            normalOrReverseImg.gameObject.SetActive(true);
        }
        else
        {
            normalOrReverseImg.gameObject.SetActive(false);
        }
    }

    private void LateUpdate()
    {
        if (enemyHealth)
        {
            if (enemyHealth.GetIsDead() || !enemyController.GetCanAttack() || !enemyController.GetEnableActivity())
            {
                StopAllCoroutines();
                DisableKeyboard();
            }
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());

        //timeSinceActivityStarted += Time.deltaTime; //Use for debugging
    }

    //Keyboard
    private void OnAPress()
    {
        if (alreadyChecking) return;
        if (!keyboard.activeInHierarchy) return;

        StartCoroutine(CheckKeyPressed(KeyIndex.A));
    }
    private void OnZPress()
    {
        if (alreadyChecking) return;
        if (!keyboard.activeInHierarchy) return;

        StartCoroutine(CheckKeyPressed(KeyIndex.Z));
    }
    private void OnEPress()
    {
        if (alreadyChecking) return;
        if (!keyboard.activeInHierarchy) return;

        StartCoroutine(CheckKeyPressed(KeyIndex.E));
    }
    private void OnRPress()
    {
        if (alreadyChecking) return;
        if (!keyboard.activeInHierarchy) return;

        StartCoroutine(CheckKeyPressed(KeyIndex.R));
    }
    private void OnQPress()
    {
        if (alreadyChecking) return;
        if (!keyboard.activeInHierarchy) return;

        StartCoroutine(CheckKeyPressed(KeyIndex.Q));
    }
    private void OnSPress()
    {
        if (alreadyChecking) return;
        if (!keyboard.activeInHierarchy) return;

        StartCoroutine(CheckKeyPressed(KeyIndex.S));
    }
    private void OnDPress()
    {
        if (alreadyChecking) return;
        if (!keyboard.activeInHierarchy) return;

        StartCoroutine(CheckKeyPressed(KeyIndex.D));
    }
    private void OnFPress()
    {
        if (alreadyChecking) return;
        if (!keyboard.activeInHierarchy) return;

        StartCoroutine(CheckKeyPressed(KeyIndex.F));
    }
    private void OnWPress()
    {
        if (alreadyChecking) return;
        if (!keyboard.activeInHierarchy) return;

        StartCoroutine(CheckKeyPressed(KeyIndex.W));
    }
    private void OnXPress()
    {
        if (alreadyChecking) return;
        if (!keyboard.activeInHierarchy) return;

        StartCoroutine(CheckKeyPressed(KeyIndex.X));
    }
    private void OnCPress()
    {
        if (alreadyChecking) return;
        if (!keyboard.activeInHierarchy) return;

        StartCoroutine(CheckKeyPressed(KeyIndex.C));
    }
    private void OnVPress()
    {
        if (alreadyChecking) return;
        if (!keyboard.activeInHierarchy) return;

        StartCoroutine(CheckKeyPressed(KeyIndex.V));
    }

    //Controller
    private void OnButtonNorthPress()
    {
        if (alreadyChecking) return;
        if (!keyboard.activeInHierarchy) return;

        StartCoroutine(CheckKeyPressed(KeyIndex.buttonNorth));
    }
    private void OnButtonSouthPress()
    {
        if (alreadyChecking) return;
        if (!keyboard.activeInHierarchy) return;

        StartCoroutine(CheckKeyPressed(KeyIndex.buttonSouth));
    }
    private void OnButtonWestPress()
    {
        if (alreadyChecking) return;
        if (!keyboard.activeInHierarchy) return;

        StartCoroutine(CheckKeyPressed(KeyIndex.buttonWest));
    }
    private void OnButtonEastPress()
    {
        if (alreadyChecking) return;
        if (!keyboard.activeInHierarchy) return;

        StartCoroutine(CheckKeyPressed(KeyIndex.buttonEast));
    }
    private void OnLeftStickButtonPress()
    {
        if (alreadyChecking) return;
        if (!keyboard.activeInHierarchy) return;

        StartCoroutine(CheckKeyPressed(KeyIndex.leftStickButton));
    }
    private void OnRightStickButtonPress()
    {
        if (alreadyChecking) return;
        if (!keyboard.activeInHierarchy) return;

        StartCoroutine(CheckKeyPressed(KeyIndex.rightStickButton));
    }
    private void OnLeftShoulderPress()
    {
        if (alreadyChecking) return;
        if (!keyboard.activeInHierarchy) return;

        StartCoroutine(CheckKeyPressed(KeyIndex.leftShoulder));
    }
    private void OnRightShoulderPress()
    {
        if (alreadyChecking) return;
        if (!keyboard.activeInHierarchy) return;

        StartCoroutine(CheckKeyPressed(KeyIndex.rightShoulder));
    }

    public void StartActivity(MonoBehaviour instigator, KeySeriesSO keySeries)
    {
        //timeSinceActivityStarted = 0; //Use for debugging

        this.instigator = instigator;

        references.GetPlayerController().SetIsInActivity(true);

        if (isReverseKeySeries && !randomKeySeries && !references.GetSimulationsPlaceHolder().GetRandomKeySeries())
        {
            keySeriesTouchs = keySeries.GetReverseKeySeries();

            if (references.GetFighter().GetWeaponType() == WeaponType.GlitchedWeapon && references.GetFighter().CanUsePower())
            {
                keySeriesTouchs.RemoveAt(keySeriesTouchs.Count - 1);
            }
        }
        else
        {
            if (randomKeySeries || references.GetSimulationsPlaceHolder().GetRandomKeySeries())
            {
                keySeriesTouchs = keySeries.GetRandomKeySeries();
            }
            else
            {
                keySeriesTouchs = keySeries.GetKeySeries();
            }

            if (references.GetFighter().GetWeaponType() == WeaponType.GlitchedWeapon && references.GetFighter().CanUsePower())
            {
                keySeriesTouchs.RemoveAt(keySeriesTouchs.Count - 1);
            }
        }

        PlayKeySeries();
    }

    private void IsReverseKeySeries()
    {
        int reversed = UnityEngine.Random.Range(0, 2);

        if (references.GetSimulationsPlaceHolder().GetReverseKeySeries() && reversed == 1)
        {
            normalOrReverseImg.sprite = normalOrReverseSprites[1];

            isReverseKeySeries = true;
        }
        else
        {
            normalOrReverseImg.sprite = normalOrReverseSprites[0];

            isReverseKeySeries = false;
        }
    }

    private void PlayKeySeries()
    {
        if (references.GetFighter().GetWeaponType() == WeaponType.GlitchedWeapon && references.GetFighter().GetIsWeaponUpgraded())
        {
            canUsePower = references.GetFighter().CanUsePower();
        }

        RefreshKeyboard();
    }

    private IEnumerator CheckKeyPressed(KeyIndex keyPressed)
    {
        alreadyChecking = true;

        if (references.GetPlayerHealth().isDead)
        {
            DisableKeyboard();
        }
        
        if (keyPressed != goodTouch)
        {
            if (!references.GetGameManager().GetNoobMode() && !references.GetGameManager().GetCantLoseNextKeySeries() 
                && !references.GetGameManager().GetCantLoseAllKeySeries())
            {
                if (keyPressed != KeyIndex.None)
                {
                    if (references.GetSimulationsPlaceHolder().GetDamageKeySeries())
                    {
                        float damage = UnityEngine.Random.Range(3, 7) * (1 + references.GetGameManager().GetRoomClearMultiplier(false));

                        references.GetPlayerHealth().TakeDamage(damage, false, true, false, true, true, true);
                    }

                    references.GetTrinket().RemoveCharge();

                    audioSource.Play();

                    DisableKeyboard();

                    yield break;
                }
            }
        }
        else if (keyPressed == goodTouch)
        {
            keys[0].SwitchKeySprites(goodTouch);

            audioSource.Play();

            if (references.GetPassiveObject().GlitchedKeys())
            {
                int spawnGlitchPercentage = UnityEngine.Random.Range(0, 100);

                if (spawnGlitchPercentage <= 4)
                {
                    spawnGlitch.Spawn();
                }
            }

            if (canUsePower)
            {
                enemyHealth.TakeDamage(1, false, false);
            }

            yield return new WaitForSeconds(timeForKeySpriteChange);

            if (keySeriesTouchs.Count == 0)
            {
                ActivityFinished();
            }
            else
            {
                RefreshKeyboard();
            }
        }

        alreadyChecking = false;
    }

    private void RefreshKeyboard()
    {
        if (keySeriesTouchs.Count == 0) return;

        goodTouch = keySeriesTouchs[0];

        for (int i = 0; i < keys.Length; i++)
        {
            if (i >= keySeriesTouchs.Count)
            {
                keys[i].EnableKey(false);

                continue;
            }
            else
            {
                keys[i].EnableKey(true);

                keys[i].SetKeySprite(keySeriesTouchs[i]);
            }
        }
        
        keySeriesTouchs.RemoveAt(0);
    }

    private void ActivityFinished()
    {
        Activity activity = null;
        HeartStalker heartStalker = null;
        EnemyHealth enemyHealth = null;

        if (instigator != null)
        {
            activity = instigator.GetComponent<Activity>();
            heartStalker = instigator.GetComponent<HeartStalker>();
            enemyHealth = instigator.GetComponent<EnemyHealth>();
        }

        if (activity)
        {
            activity.EndActivity();
        }
        else if (heartStalker)
        {
            heartStalker.EndActivity();
        }
        
        if (enemyHealth)
        {
            if (!enemyHealth.GetShieldUpgrade())
            {
                references.GetTrinket().AddCharge();
            }
        }
        else
        {
            references.GetTrinket().AddCharge();
        }
        

        canUsePower = false;

        //print(Mathf.Round(timeSinceActivityStarted * 100) / 100); //Use for debugging

        if (references.GetGameManager().GetCantLoseNextKeySeries())
        {
            references.GetGameManager().SetCantLoseNextKeySeries(false);
        }

        DisableKeyboard();
    }

    public void EnableKeyboard(float timer, float minTimer)
    {
        inputActions.Player.Enable();

        references.GetPlayerController().SetIsInActivity(true);

        if (references.GetSimulationsPlaceHolder().GetRandomKeySeries() && timer != -1)
        {
            activityTimer.SetTimeToCompleteActivity(1, 1);
        }
        else
        {
            activityTimer.SetTimeToCompleteActivity(timer, minTimer);
        }

        keyboard.SetActive(true);
    }

    public void DisableKeyboard()
    {
        if (keyboard.activeInHierarchy)
        {
            inputActions.Player.Disable();

            StopAllCoroutines();

            references.GetPlayerController().SetIsInActivity(false);

            alreadyChecking = false;

            IsReverseKeySeries();

            keyboard.SetActive(false);
        }
    }
}

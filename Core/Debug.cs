using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Debug : MonoBehaviour
{
//#if UNITY_EDITOR
    [SerializeField] KeySeriesSO testKeySeries;

    CatchReferences references;

    private void Awake()
    {
        references = FindObjectOfType<CatchReferences>();
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0) return;

        if (Input.GetKeyDown(KeyCode.Keypad0))
        {
            references.GetPlayerHealth().TakeDamage(1, false, true, true, true, true);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            references.GetPlayerHealth().TakeDamage(10, true, true, false, true, true);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            references.GetPlayerHealth().Heal(references.GetPlayerStatistics().GetBaseHealth());
        }
        else if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            references.GetPlayerHealth().RegainArmor(10);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad4) && !references.GetPlayerController().GetIsInActivity())
        {
            if (!testKeySeries) return;

            references.GetPlayerKeyboard().EnableKeyboard(testKeySeries.GetMinTimeToCompleteActivity(), testKeySeries.GetTimeToCompleteActivity());
            references.GetPlayerKeyboard().StartActivity(null, testKeySeries);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad5))
        {
            references.GetTrinket().AddCharge();
        }
        else if (Input.GetKeyDown(KeyCode.Keypad6))
        {
            references.GetCurse().AddCurse(10, true);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad7))
        {
            ScreenCapture.CaptureScreenshot(Application.persistentDataPath + ".png");
        }
        else if (Input.GetKeyDown(KeyCode.Keypad8))
        {
            SceneManager.LoadScene("Test Sector");
        }
        else if (Input.GetKeyDown(KeyCode.Keypad9))
        {
            references.GetPlayerHealth().SwitchInvincibility();
        }
    }
//#endif
}

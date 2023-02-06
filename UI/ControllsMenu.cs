using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ControllsMenu : MonoBehaviour
{
    [Header("Miscs")]
    [SerializeField] GameObject[] unselected;
    [SerializeField] Image[] panelImg;
    [SerializeField] Sprite[] panelSprites;
    [SerializeField] AudioSource selectionAS;

    [Header("Keyboard")]
    [SerializeField] Image keyboardKeys;
    [SerializeField] Sprite[] keyboardKeysSprites;

    [Header("Controller")]
    [SerializeField] Image controllerKeys;
    [SerializeField] Sprite[] controllerKeysSprites;

    [Header("UI Switch")]
    [SerializeField] Image changeKeyboardImg;
    [SerializeField] Sprite[] changeKeyboardSprites;
    [SerializeField] TextMeshProUGUI changeKeyboardTxt;

    [Header("Skip Dialogue")]
    [SerializeField] GameObject[] skipDialogues;
    [SerializeField] Image[] checkMarkImg;
    [SerializeField] Sprite[] checkMarkSprites;
    [SerializeField] Image[] keyToSkipDialogueImg;
    [SerializeField] Sprite[] keyToSkipDialogueSprites;

    CatchReferences references;
    PlayerInputSystem inputActions;

    const float timeBetweenKeySwap = 0.07f;
    bool isSwapKeySprite;

    private void Awake()
    {
        references = FindObjectOfType<CatchReferences>();

        inputActions = new();
        inputActions.UIControllsMenu.ChangeUI.started += ctx => OnChangeUI();
        inputActions.UIControllsMenu.QuitControllsMenu.performed += ctx => OnDisableControllsMenu();
        inputActions.UIControllsMenu.SkipDialogue.performed += ctx => OnSkipDialogue();
    }

    private void OnEnable()
    {
        inputActions.UIControllsMenu.Enable();

        RefreshSkipDialogueCheckMark();
        RefreshChangeUIText();
    }

    private void OnDisable()
    {
        inputActions.UIControllsMenu.Disable();
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }

    private void Update()
    {
        RefreshUI();
    }

    private void OnChangeUI()
    {
        if (!gameObject.activeInHierarchy) return;

        selectionAS.Play();

        references.GetGameManager().ChangeKeyboardType();

        RefreshChangeUIText();

        if (references.GetGameManager().GetKeyboardControl())
        {
            StartCoroutine(SwitchKeySprite(false));
        }
    }

    private void OnDisableControllsMenu()
    {
        if (!gameObject.activeInHierarchy) return;

        selectionAS.Play();

        if (references.GetPauseMenu())
        {
            references.GetPauseMenu().EnableControllsMenu(false);
        }
        else
        {
            references.GetMainMenu().EnableControllsMenu(false);
        }
    }

    private void OnSkipDialogue()
    {
        if (!gameObject.activeInHierarchy) return;

        references.GetGameManager().SkipDialogue();

        RefreshSkipDialogueCheckMark();

        StartCoroutine(SwitchKeySprite(true));
    }

    private void RefreshChangeUIText()
    {
        if (references.GetGameManager().GetAzertyControl())
        {
            changeKeyboardTxt.text = "hange UI (Azerty)";
        }
        else
        {
            changeKeyboardTxt.text = "hange UI (Qwerty)";
        }
    }

    private void RefreshSkipDialogueCheckMark()
    {
        if (references.GetGameManager().GetSkipDialogue())
        {
            checkMarkImg[1].sprite = checkMarkSprites[0];
            checkMarkImg[0].sprite = checkMarkSprites[0];
        }
        else
        {
            checkMarkImg[1].sprite = checkMarkSprites[1];
            checkMarkImg[0].sprite = checkMarkSprites[1];
        }
    }

    private void RefreshUI()
    {
        if (isSwapKeySprite) return;

        if (references.GetGameManager().GetKeyboardControl())
        {
            unselected[0].SetActive(false);
            unselected[1].SetActive(true);

            skipDialogues[0].SetActive(true);
            skipDialogues[1].SetActive(false);

            panelImg[0].sprite = panelSprites[0];
            panelImg[1].sprite = panelSprites[1];

            changeKeyboardImg.color = new(1, 1, 1, 1);
            changeKeyboardTxt.color = new(1, 1, 1, 1);

            if (references.GetGameManager().GetAzertyControl())
            {
                keyboardKeys.sprite = keyboardKeysSprites[1];
            }
            else
            {
                keyboardKeys.sprite = keyboardKeysSprites[0];
            }
        }
        else
        {
            unselected[0].SetActive(true);
            unselected[1].SetActive(false);

            skipDialogues[0].SetActive(false);
            skipDialogues[1].SetActive(true);

            panelImg[0].sprite = panelSprites[1];
            panelImg[1].sprite = panelSprites[0];

            changeKeyboardImg.color = new(1, 1, 1, 0.5f);
            changeKeyboardTxt.color = new(1, 1, 1, 0.5f);

            if (references.GetGameManager().GetChangeUI())
            {
                controllerKeys.sprite = controllerKeysSprites[1];

                keyToSkipDialogueImg[1].sprite = keyToSkipDialogueSprites[4];
            }
            else
            {
                controllerKeys.sprite = controllerKeysSprites[0];

                keyToSkipDialogueImg[1].sprite = keyToSkipDialogueSprites[2];
            }
        }
    }

    private IEnumerator SwitchKeySprite(bool skipDialogue)
    {
        isSwapKeySprite = true;

        if (references.GetPauseMenu())
        {
            Time.timeScale = 1f;
        }

        if (skipDialogue)
        {
            if (references.GetGameManager().GetKeyboardControl())
            {
                keyToSkipDialogueImg[0].sprite = keyToSkipDialogueSprites[1];

                yield return new WaitForSeconds(timeBetweenKeySwap);

                keyToSkipDialogueImg[0].sprite = keyToSkipDialogueSprites[0];
            }
            else
            {
                if (references.GetGameManager().GetChangeUI())
                {
                    keyToSkipDialogueImg[1].sprite = keyToSkipDialogueSprites[5];

                    yield return new WaitForSeconds(timeBetweenKeySwap);

                    keyToSkipDialogueImg[1].sprite = keyToSkipDialogueSprites[4];
                }
                else
                {
                    keyToSkipDialogueImg[1].sprite = keyToSkipDialogueSprites[3];

                    yield return new WaitForSeconds(timeBetweenKeySwap);

                    keyToSkipDialogueImg[1].sprite = keyToSkipDialogueSprites[2];
                }
            }
        }
        else
        {
            changeKeyboardImg.sprite = changeKeyboardSprites[1];

            yield return new WaitForSeconds(timeBetweenKeySwap);

            changeKeyboardImg.sprite = changeKeyboardSprites[0];
        }

        if (references.GetPauseMenu())
        {
            Time.timeScale = 0f;
        }

        isSwapKeySprite = false;
    }
}

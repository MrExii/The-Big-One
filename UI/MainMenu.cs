using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] AudioSource mainMusicAS;
    [SerializeField] AudioSource rainAS;
    [SerializeField] AudioSource selectionAS;
    [SerializeField] Fader[] faders;
    [SerializeField] GameObject controllsMenu;
    [SerializeField] Animator titleAnimator;
    [SerializeField] Sprite[] panels;

    [Header("Main Menu")]
    [SerializeField] GameObject mainMenuButtons;
    [SerializeField] TextMeshProUGUI[] playButtonTxt;
    [SerializeField] GameObject[] mainMenuKeyboardButton;
    [SerializeField] GameObject[] mainMenuControllerButton;
    [SerializeField] Image[] mainMenuButtonImgs;
    [SerializeField] Sprite[] mainMenuButtonSprites;

    [Header("Options")]
    [SerializeField] GameObject optionButtons;
    [SerializeField] GameObject[] optionKeyboardButton;
    [SerializeField] GameObject[] optionControllerButton;
    [SerializeField] TextMeshProUGUI[] optionDeleteSaveTxt;
    [SerializeField] Image[] optionImgs;
    [SerializeField] Sprite[] optionSprites;

    [Header("Volume Bar Settings")]
    [SerializeField] Image fillImg;
    [SerializeField] TextMeshProUGUI volumePercentageTxt;
    [SerializeField] Image[] rightArrowKeyImg;
    [SerializeField] Sprite[] rightArrowSprites;
    [SerializeField] Image[] leftArrowKeyImg;
    [SerializeField] Sprite[] leftArrowSprites;

    CatchReferences references;
    PlayerInputSystem inputActions;

    int selectionIndex;

    bool activateUpdate;
    bool fadersEnd;
    bool keySpriteSwitch;

    const float timeBetweenLetterSwitch = 0.07f;
    float timeSinceLastGlitching;

    private void Awake()
    {
        references = FindObjectOfType<CatchReferences>();

        inputActions = new();
        inputActions.UIMainMenu.PassFaders.performed += ctx => OnStopFaders();
        inputActions.UIMainMenu.NewGame.performed += ctx => OnNewGame();
        inputActions.UIMainMenu.DeleteSave.performed += ctx => OnDeleteSave();
        inputActions.UIMainMenu.OpenOptions.performed += ctx => OnOpenOptions();
        inputActions.UIMainMenu.Quit.performed += ctx => OnQuitApplication();
        inputActions.UIMainMenu.OpenControlls.performed += ctx => OnOpenControlls();
        inputActions.UIMainMenu.QuitOptions.performed += ctx => OnReturnMainMenu();
        inputActions.UIMainMenu.RightArrow.performed += ctx => OnAddSound();
        inputActions.UIMainMenu.LeftArrow.performed += ctx => OnRemoveSound();
        inputActions.UIMainMenu.Continue.performed += ctx => OnContinue();
        inputActions.UIMainMenu.SelectDown.performed += ctx => OnSelectDown();
        inputActions.UIMainMenu.SelectUp.performed += ctx => OnSelectUp();

        inputActions.UIMainMenu.Enable();
    }

    private void OnDisable()
    {
        inputActions.UIMainMenu.Disable();
    }

    private IEnumerator Start()
    {
        RefreshUI();
        RefreshCurrentSelection();

        fillImg.fillAmount = references.GetGameManager().GetCurrentAudioVolume();
        volumePercentageTxt.text = Mathf.CeilToInt(references.GetGameManager().GetCurrentAudioVolume() * 100).ToString() + "%";

        for (int i = 1; i < faders.Length; i++)
        {
            faders[i].SetCanvasGroupAlpha(0f);
        }

        if (references.GetGameManager().GetHasCurrentSave())
        {
            mainMenuButtonImgs[0].sprite = mainMenuButtonSprites[2];

            playButtonTxt[0].text = "ontinue";

            playButtonTxt[1].text = "Continue";
        }
        else
        {
            mainMenuButtonImgs[0].sprite = mainMenuButtonSprites[0];

            playButtonTxt[0].text = "ew Game";

            playButtonTxt[1].text = "New Game";
        }

        optionButtons.SetActive(false);

        yield return new WaitForSeconds(0.4f);

        yield return StartCoroutine(faders[0].FadeIn(3));

        yield return StartCoroutine(faders[1].FadeOut(1));

        yield return StartCoroutine(faders[2].FadeOut(1));

        fadersEnd = true;
        activateUpdate = true;

        yield return StartCoroutine(faders[3].FadeOut(1));

        GlitchingTitle();
    }

    private void LateUpdate()
    {
        RefreshUI();

        if (!activateUpdate) return;
        if (!fadersEnd) return;

        if (timeSinceLastGlitching == 0)
        {
            GlitchingTitle();
        }
    }

    private void OnStopFaders()
    {
        if (fadersEnd) return;

        StopAllCoroutines();

        foreach (Fader fader in faders)
        {
            fader.StopAllCoroutines();

            if (fader.CompareTag("Fader"))
            {
                fader.GetComponent<CanvasGroup>().alpha = 0f;
            }
            else
            {
                fader.GetComponent<CanvasGroup>().alpha = 1f;
            }
        }

        fadersEnd = true;
        activateUpdate = true;
    }

    private void OnContinue()
    {
        if (!mainMenuButtons.activeInHierarchy) return;
        if (!references.GetGameManager().GetHasCurrentSave()) return;
        if (selectionIndex != 0) return;
        if (!fadersEnd) return;
        if (!activateUpdate) return;

        StartCoroutine(SwitchKeySprite("C"));
    }

    private void OnNewGame()
    {
        if (!mainMenuButtons.activeInHierarchy) return;
        if (references.GetGameManager().GetHasCurrentSave()) return;
        if (selectionIndex != 0) return;
        if (!fadersEnd) return;
        if (!activateUpdate) return;

        StartCoroutine(SwitchKeySprite("N"));
    }

    private void OnOpenOptions()
    {
        if (!mainMenuButtons.activeInHierarchy) return;
        if (selectionIndex != 1) return;
        if (!fadersEnd) return;
        if (!activateUpdate) return;

        selectionIndex = 0;
        RefreshCurrentSelection();

        StartCoroutine(SwitchKeySprite("O"));
    }

    private void OnQuitApplication()
    {
        if (!mainMenuButtons.activeInHierarchy) return;
        if (selectionIndex != 2) return;
        if (!fadersEnd) return;
        if (!activateUpdate) return;

        StartCoroutine(SwitchKeySprite("Q"));
    }

    private void OnDeleteSave()
    {
        if (!optionButtons.activeInHierarchy) return;
        if (selectionIndex != 0) return;
        if (!activateUpdate) return;
        if (optionDeleteSaveTxt[0].text == "No Save") return;
        if (optionDeleteSaveTxt[0].text == "Save deleted") return;

        StartCoroutine(SwitchKeySprite("D"));
    }

    private void OnRemoveSound()
    {
        if (!optionButtons.activeInHierarchy) return;
        if (fillImg.fillAmount == 0) return;
        if (selectionIndex != 1) return;
        if (!activateUpdate) return;

        StartCoroutine(SwitchKeySprite("L"));

        ChangeVolume(-0.05f);
    }

    private void OnAddSound()
    {
        if (!optionButtons.activeInHierarchy) return;
        if (fillImg.fillAmount == 1) return;
        if (selectionIndex != 1) return;
        if (!activateUpdate) return;

        StartCoroutine(SwitchKeySprite("R"));

        ChangeVolume(0.05f);
    }

    private void OnOpenControlls()
    {
        if (!optionButtons.activeInHierarchy) return;
        if (selectionIndex != 2) return;
        if (!activateUpdate) return;

        StartCoroutine(SwitchKeySprite("C"));
    }

    private void OnReturnMainMenu()
    {
        if (!optionButtons.activeInHierarchy) return;
        if (!activateUpdate) return;

        selectionIndex = 0;
        RefreshCurrentSelection();

        selectionAS.Play();

        Return();
    }

    private void OnSelectDown()
    {
        if (controllsMenu.activeInHierarchy) return;
        if (!activateUpdate) return;

        selectionIndex++;

        if (selectionIndex > 2)
        {
            selectionIndex = 0;
        }

        selectionAS.Play();

        RefreshCurrentSelection();
    }

    private void OnSelectUp()
    {
        if (controllsMenu.activeInHierarchy) return;
        if (!activateUpdate) return;

        selectionIndex--;

        if (selectionIndex < 0)
        {
            selectionIndex = 2;
        }

        selectionAS.Play();

        RefreshCurrentSelection();
    }

    private void RefreshCurrentSelection()
    {
        //Keyboard
        for (int i = 0; i < mainMenuKeyboardButton.Length; i++)
        {
            if (i == selectionIndex)
            {
                mainMenuKeyboardButton[i].GetComponent<Image>().sprite = panels[0];
                mainMenuKeyboardButton[i].transform.localScale = new(0.98f, 0.98f, 1);
                mainMenuKeyboardButton[i].transform.GetChild(2).gameObject.SetActive(false);

                optionKeyboardButton[i].GetComponent<Image>().sprite = panels[0];
                optionKeyboardButton[i].transform.localScale = new(0.98f, 0.98f, 1);

                if (i == 1)
                {
                    optionKeyboardButton[i].transform.GetChild(5).gameObject.SetActive(false);
                }
                else
                {
                    optionKeyboardButton[i].transform.GetChild(2).gameObject.SetActive(false);
                }
            }
            else
            {
                mainMenuKeyboardButton[i].GetComponent<Image>().sprite = panels[1];
                mainMenuKeyboardButton[i].transform.localScale = new(1, 1, 1);
                mainMenuKeyboardButton[i].transform.GetChild(2).gameObject.SetActive(true);

                optionKeyboardButton[i].GetComponent<Image>().sprite = panels[1];
                optionKeyboardButton[i].transform.localScale = new(1, 1, 1);

                if (i == 1)
                {
                    optionKeyboardButton[i].transform.GetChild(5).gameObject.SetActive(true);
                }
                else
                {
                    optionKeyboardButton[i].transform.GetChild(2).gameObject.SetActive(true);
                }
            }
        }

        //Controller
        for (int i = 0; i < mainMenuControllerButton.Length; i++)
        {
            if (i == selectionIndex)
            {
                mainMenuControllerButton[i].GetComponent<Image>().sprite = panels[0];
                mainMenuControllerButton[i].transform.localScale = new(0.98f, 0.98f, 1);
                mainMenuControllerButton[i].transform.GetChild(2).gameObject.SetActive(false);

                optionControllerButton[i].GetComponent<Image>().sprite = panels[0];
                optionControllerButton[i].transform.localScale = new(0.98f, 0.98f, 1);
                
                if (i == 1)
                {
                    optionControllerButton[i].transform.GetChild(5).gameObject.SetActive(false);
                }
                else
                {
                    optionControllerButton[i].transform.GetChild(2).gameObject.SetActive(false);
                }
            }
            else
            {
                mainMenuControllerButton[i].GetComponent<Image>().sprite = panels[1];
                mainMenuControllerButton[i].transform.localScale = new(1, 1, 1);
                mainMenuControllerButton[i].transform.GetChild(2).gameObject.SetActive(true);

                optionControllerButton[i].GetComponent<Image>().sprite = panels[1];
                optionControllerButton[i].transform.localScale = new(1, 1, 1);

                if (i == 1)
                {
                    optionControllerButton[i].transform.GetChild(5).gameObject.SetActive(true);
                }
                else
                {
                    optionControllerButton[i].transform.GetChild(2).gameObject.SetActive(true);
                }
            }
        }
    }

    private void RefreshUI()
    {
        if (references.GetGameManager().GetKeyboardControl())
        {
            DisableKeyboardUI(false);
        }
        else
        {
            DisableKeyboardUI(true);

            if (references.GetGameManager().GetChangeUI() && !keySpriteSwitch)
            {
                mainMenuButtonImgs[3].sprite = mainMenuButtonSprites[10];
                mainMenuButtonImgs[4].sprite = mainMenuButtonSprites[12];
                mainMenuButtonImgs[5].sprite = mainMenuButtonSprites[14];

                optionImgs[2].sprite = optionSprites[8];
                optionImgs[3].sprite = optionSprites[10];

                rightArrowKeyImg[1].sprite = rightArrowSprites[4];
                leftArrowKeyImg[1].sprite = leftArrowSprites[4];
            }
            else if (!keySpriteSwitch)
            {
                mainMenuButtonImgs[3].sprite = mainMenuButtonSprites[4];
                mainMenuButtonImgs[4].sprite = mainMenuButtonSprites[6];
                mainMenuButtonImgs[5].sprite = mainMenuButtonSprites[8];

                optionImgs[2].sprite = optionSprites[4];
                optionImgs[3].sprite = optionSprites[6];

                rightArrowKeyImg[1].sprite = rightArrowSprites[2];
                leftArrowKeyImg[1].sprite = leftArrowSprites[2];
            }
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(mainMenuKeyboardButton[0].GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(mainMenuControllerButton[0].GetComponent<RectTransform>());
    }

    private void DisableKeyboardUI(bool state)
    {
        mainMenuKeyboardButton[0].SetActive(!state);
        mainMenuKeyboardButton[1].SetActive(!state);
        mainMenuKeyboardButton[2].SetActive(!state);
        mainMenuControllerButton[0].SetActive(state);
        mainMenuControllerButton[1].SetActive(state);
        mainMenuControllerButton[2].SetActive(state);

        optionKeyboardButton[0].SetActive(!state);
        optionKeyboardButton[2].SetActive(!state);
        optionControllerButton[0].SetActive(state);
        optionControllerButton[2].SetActive(state);

        rightArrowKeyImg[0].gameObject.SetActive(!state);
        rightArrowKeyImg[1].gameObject.SetActive(state);

        leftArrowKeyImg[0].gameObject.SetActive(!state);
        leftArrowKeyImg[1].gameObject.SetActive(state);
    }

    private void GlitchingTitle()
    {
        float time = UnityEngine.Random.Range(8f, 15f);
        timeSinceLastGlitching = 0;

        while (timeSinceLastGlitching < time)
        {
            timeSinceLastGlitching += Time.deltaTime;
        }

        titleAnimator.SetTrigger("glitching");

        Invoke(nameof(GlitchingTitle), time);
    }

    private void Play()
    {
        StartCoroutine(LoadGame());

        activateUpdate = false;
    }

    private void DeleteSave()
    {
        if (references.GetGameManager().GetHasCurrentSave())
        {
            FindObjectOfType<SavingWrapper>().Delete();

            references.GetGameManager().ResetSaveData();

            mainMenuButtonImgs[0].sprite = mainMenuButtonSprites[0];

            playButtonTxt[0].text = "ew Game";
            playButtonTxt[1].text = "New Game";

            optionDeleteSaveTxt[0].text = "Save deleted";
            optionDeleteSaveTxt[1].text = "Save deleted";
        }
        else
        {
            optionDeleteSaveTxt[0].text = "No Save";
            optionDeleteSaveTxt[1].text = "No Save";
        }

        optionKeyboardButton[0].transform.GetChild(0).gameObject.SetActive(false);
        optionControllerButton[0].transform.GetChild(0).gameObject.SetActive(false);
    }

    private void ChangeVolume(float volumeValue)
    {
        fillImg.fillAmount += volumeValue;

        references.GetGameManager().ChangeVolume(fillImg.fillAmount);

        volumePercentageTxt.text = Mathf.RoundToInt(references.GetGameManager().GetCurrentAudioVolume() * 100).ToString() + "%";
    }

    public void EnableControllsMenu(bool state)
    {
        controllsMenu.SetActive(state);
        optionButtons.SetActive(!state);
    }

    private void Return()
    {
        mainMenuButtons.SetActive(true);
        optionButtons.SetActive(false);
    }

    private void Quit()
    {
        Application.Quit();

        print("Application Quit");
    }

    private IEnumerator LoadGame()
    {
        FindObjectOfType<SavingWrapper>().Save();
        
        StartCoroutine(faders[0].FadeOut(1f));
        StartCoroutine(DisableMusic());

        yield return new WaitForSeconds(1f);

        if (!references.GetGameManager().GetTutoDone())
        {
            yield return SceneManager.LoadSceneAsync("Tuto");
        }
        else
        {
            yield return SceneManager.LoadSceneAsync("Home");
        }
    }

    private IEnumerator DisableMusic()
    {
        while (mainMusicAS.volume > 0)
        {
            mainMusicAS.volume -= Time.deltaTime;
            rainAS.volume -= Time.deltaTime / 0.7f;

            yield return new WaitForSeconds(Time.deltaTime);
        }
    }

    private void Options()
    {
        mainMenuButtons.SetActive(false);
        optionButtons.SetActive(true);
    }

    private IEnumerator SwitchKeySprite(string key)
    {
        keySpriteSwitch = true;

        if (references.GetGameManager().GetKeyboardControl())
        {
            if (key == "N")
            {
                mainMenuButtonImgs[0].sprite = mainMenuButtonSprites[1];

                yield return new WaitForSeconds(timeBetweenLetterSwitch);

                mainMenuButtonImgs[0].sprite = mainMenuButtonSprites[0];
                
                Play();
            }
            else if (key == "C")
            {
                if (optionButtons.activeInHierarchy)
                {
                    optionImgs[1].sprite = optionSprites[3];
                }
                else
                {
                    mainMenuButtonImgs[0].sprite = mainMenuButtonSprites[3];
                }

                yield return new WaitForSeconds(timeBetweenLetterSwitch);

                if (optionButtons.activeInHierarchy)
                {
                    optionImgs[1].sprite = optionSprites[2];

                    EnableControllsMenu(true);
                }
                else
                {
                    mainMenuButtonImgs[0].sprite = mainMenuButtonSprites[2];

                    Play();
                }
            }
            else if (key == "O")
            {
                mainMenuButtonImgs[1].sprite = mainMenuButtonSprites[19];

                yield return new WaitForSeconds(timeBetweenLetterSwitch);

                mainMenuButtonImgs[1].sprite = mainMenuButtonSprites[18];

                Options();
            }
            else if (key == "D")
            {
                optionImgs[0].sprite = optionSprites[1];

                yield return new WaitForSeconds(timeBetweenLetterSwitch);

                optionImgs[0].sprite = optionSprites[0];

                DeleteSave();
            }
            else if (key == "Q")
            {
                mainMenuButtonImgs[2].sprite = mainMenuButtonSprites[21];

                yield return new WaitForSeconds(timeBetweenLetterSwitch);

                mainMenuButtonImgs[2].sprite = mainMenuButtonSprites[20];

                Quit();
            }
            else if (key == "R")
            {
                rightArrowKeyImg[0].sprite = rightArrowSprites[1];

                yield return new WaitForSeconds(timeBetweenLetterSwitch);

                rightArrowKeyImg[0].sprite = rightArrowSprites[0];
            }
            else if (key == "L")
            {
                leftArrowKeyImg[0].sprite = leftArrowSprites[1];

                yield return new WaitForSeconds(timeBetweenLetterSwitch);

                leftArrowKeyImg[0].sprite = leftArrowSprites[0];
            }
        }
        else
        {
            if (references.GetGameManager().GetChangeUI())
            {
                if (key == "D")
                {
                    optionImgs[2].sprite = optionSprites[9];

                    yield return new WaitForSeconds(timeBetweenLetterSwitch);

                    optionImgs[2].sprite = optionSprites[8];

                    DeleteSave();
                }
                else if (key == "Q")
                {
                    mainMenuButtonImgs[5].sprite = mainMenuButtonSprites[15];

                    yield return new WaitForSeconds(timeBetweenLetterSwitch);

                    mainMenuButtonImgs[5].sprite = mainMenuButtonSprites[14];

                    Quit();
                }
                else if (key == "O")
                {
                    mainMenuButtonImgs[4].sprite = mainMenuButtonSprites[13];

                    yield return new WaitForSeconds(timeBetweenLetterSwitch);

                    mainMenuButtonImgs[4].sprite = mainMenuButtonSprites[12];

                    Options();
                }
                else if (key == "N")
                {
                    mainMenuButtonImgs[3].sprite = mainMenuButtonSprites[11];

                    yield return new WaitForSeconds(timeBetweenLetterSwitch);

                    mainMenuButtonImgs[3].sprite = mainMenuButtonSprites[10];

                    Play();
                }
                else if (key == "C")
                {
                    if (optionButtons.activeInHierarchy)
                    {
                        optionImgs[3].sprite = optionSprites[11];
                    }
                    else
                    {
                        mainMenuButtonImgs[3].sprite = mainMenuButtonSprites[11];
                    }

                    yield return new WaitForSeconds(timeBetweenLetterSwitch);

                    if (optionButtons.activeInHierarchy)
                    {
                        optionImgs[3].sprite = optionSprites[10];

                        EnableControllsMenu(true);
                    }
                    else
                    {
                        mainMenuButtonImgs[3].sprite = mainMenuButtonSprites[10];

                        Play();
                    }
                }
                else if (key == "R")
                {
                    rightArrowKeyImg[1].sprite = rightArrowSprites[5];

                    yield return new WaitForSeconds(timeBetweenLetterSwitch);

                    rightArrowKeyImg[1].sprite = rightArrowSprites[4];
                }
                else if (key == "L")
                {
                    leftArrowKeyImg[1].sprite = leftArrowSprites[5];

                    yield return new WaitForSeconds(timeBetweenLetterSwitch);

                    leftArrowKeyImg[1].sprite = leftArrowSprites[4];
                }
            }
            else
            {
                if (key == "D")
                {
                    optionImgs[2].sprite = optionSprites[5];

                    yield return new WaitForSeconds(timeBetweenLetterSwitch);

                    optionImgs[2].sprite = optionSprites[4];

                    DeleteSave();
                }
                else if (key == "N")
                {
                    mainMenuButtonImgs[3].sprite = mainMenuButtonSprites[5];

                    yield return new WaitForSeconds(timeBetweenLetterSwitch);

                    mainMenuButtonImgs[3].sprite = mainMenuButtonSprites[4];

                    Play();
                }
                else if (key == "Q")
                {
                    mainMenuButtonImgs[5].sprite = mainMenuButtonSprites[9];

                    yield return new WaitForSeconds(timeBetweenLetterSwitch);

                    mainMenuButtonImgs[5].sprite = mainMenuButtonSprites[8];

                    Quit();
                }
                else if (key == "O")
                {
                    mainMenuButtonImgs[4].sprite = mainMenuButtonSprites[7];

                    yield return new WaitForSeconds(timeBetweenLetterSwitch);

                    mainMenuButtonImgs[4].sprite = mainMenuButtonSprites[6];

                    Options();
                }
                else if (key == "C")
                {
                    if (optionButtons.activeInHierarchy)
                    {
                        optionImgs[3].sprite = optionSprites[7];
                    }
                    else
                    {
                        mainMenuButtonImgs[3].sprite = mainMenuButtonSprites[5];
                    }

                    yield return new WaitForSeconds(timeBetweenLetterSwitch);

                    if (optionButtons.activeInHierarchy)
                    {
                        optionImgs[3].sprite = optionSprites[6];

                        EnableControllsMenu(true);
                    }
                    else
                    {
                        mainMenuButtonImgs[3].sprite = mainMenuButtonSprites[4];

                        Play();
                    }
                }
                else if (key == "R")
                {
                    rightArrowKeyImg[1].sprite = rightArrowSprites[3];

                    yield return new WaitForSeconds(timeBetweenLetterSwitch);

                    rightArrowKeyImg[1].sprite = rightArrowSprites[2];
                }
                else if (key == "L")
                {
                    leftArrowKeyImg[1].sprite = leftArrowSprites[3];

                    yield return new WaitForSeconds(timeBetweenLetterSwitch);

                    leftArrowKeyImg[1].sprite = leftArrowSprites[2];
                }
            }
        }

        selectionAS.Play();

        keySpriteSwitch = false;
    }
}

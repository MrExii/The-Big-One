using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;
    [SerializeField] AudioSource selectionAS;
    [SerializeField] GameObject controllsMenu;
    [SerializeField] Image[] optionBackgrounds;
    [SerializeField] Sprite[] optionBackgroundSprites;
    [SerializeField] Image[] notSelectedImgs;

    [Header("Home Settings")]
    [SerializeField] GameObject homeGameObject;
    [SerializeField] GameObject[] home;
    [SerializeField] Image[] homeKeyImg;
    [SerializeField] Sprite[] homeSprites;

    [Header("Controlls Settings")]
    [SerializeField] GameObject[] options;
    [SerializeField] Image[] optionsKeyImg;
    [SerializeField] Sprite[] optionsSprites;

    [Header("Quit Settings")]
    [SerializeField] GameObject[] quit;
    [SerializeField] Image[] quitKeyImg;
    [SerializeField] Sprite[] quitSprites;

    [Header("Volume Bar Settings")]
    [SerializeField] GameObject[] volume;
    [SerializeField] Image fillImg;
    [SerializeField] TextMeshProUGUI volumePercentageTxt;
    [SerializeField] Image[] leftArrowKeyImg;
    [SerializeField] Sprite[] leftArrowSprites;
    [SerializeField] Image[] rightArrowKeyImg;
    [SerializeField] Sprite[] rightArrowSprites;

    [Header("Escape Settings")]
    [SerializeField] Image[] escapeKeyImg;
    [SerializeField] Sprite[] escapeSprites;
    [SerializeField] GameObject keyboardEscapeImg;
    [SerializeField] GameObject controllerEscapeImg;

    CatchReferences references;
    PlayerInputSystem inputActions;

    bool isSpriteSwitch;
    bool cantReturn;
    bool inPauseMenu;
    bool cantOpenPauseMenu;

    const float timeBetweenLetterSwitch = 0.07f;

    int selectionIndex;

    private void Awake()
    {
        references = FindObjectOfType<CatchReferences>();

        inputActions = new();
        inputActions.UIPauseMenu.CloseMenu.performed += ctx => OnDisableMenu();
        inputActions.UIPauseMenu.OpenMenu.performed += ctx => OnEnableMenu();
        inputActions.UIPauseMenu.ControllsMenu.performed += ctx => OnOpenControllsMenu();
        inputActions.UIPauseMenu.Quit.performed += ctx => OnQuit();
        inputActions.UIPauseMenu.ReturnHome.performed += ctx => OnReturnHome();
        inputActions.UIPauseMenu.LeftArrow.performed += ctx => OnRemoveVolume();
        inputActions.UIPauseMenu.RightArrow.performed += ctx => OnAddVolume();
        inputActions.UIPauseMenu.UpArrow.performed += ctx => OnGoUp();
        inputActions.UIPauseMenu.DownArrow.performed += ctx => OnGoDown();

        inputActions.UIPauseMenu.Enable();
    }

    private void OnDestroy()
    {
        inputActions.UIPauseMenu.Disable();
    }

    private void Start()
    {
        fillImg.fillAmount = references.GetGameManager().GetCurrentAudioVolume();
        volumePercentageTxt.text = Mathf.CeilToInt(references.GetGameManager().GetCurrentAudioVolume() * 100).ToString() + "%";

        pauseMenu.SetActive(false);

        keyboardEscapeImg.SetActive(false);
        controllerEscapeImg.SetActive(false);

        home[0].SetActive(false);
        home[1].SetActive(false);

        options[0].SetActive(false);
        options[1].SetActive(false);

        quit[0].SetActive(false);
        quit[1].SetActive(false);

        volume[0].SetActive(false);
        volume[1].SetActive(false);

        RefreshSelectedOption();
    }

    private void Update()
    {
        RefreshUI();
    }

    private void RefreshUI()
    {
        if (!home[0].activeInHierarchy && references.GetGameManager().GetKeyboardControl())
        {
            keyboardEscapeImg.SetActive(true);
            controllerEscapeImg.SetActive(false);

            home[0].SetActive(true);
            home[1].SetActive(false);

            options[0].SetActive(true);
            options[1].SetActive(false);

            quit[0].SetActive(true);
            quit[1].SetActive(false);

            volume[0].SetActive(true);
            volume[1].SetActive(false);
        }
        else if (!home[1].activeInHierarchy && !references.GetGameManager().GetKeyboardControl())
        {
            keyboardEscapeImg.SetActive(false);
            controllerEscapeImg.SetActive(true);

            home[0].SetActive(false);
            home[1].SetActive(true);

            options[0].SetActive(false);
            options[1].SetActive(true);

            quit[0].SetActive(false);
            quit[1].SetActive(true);

            volume[0].SetActive(false);
            volume[1].SetActive(true);
        }

        if (references.GetGameManager().GetChangeUI() && !isSpriteSwitch)
        {
            homeKeyImg[1].sprite = homeSprites[4];
            optionsKeyImg[1].sprite = optionsSprites[4];
            quitKeyImg[1].sprite = quitSprites[4];
            leftArrowKeyImg[1].sprite = leftArrowSprites[4];
            rightArrowKeyImg[1].sprite = rightArrowSprites[4];
            escapeKeyImg[1].sprite = escapeSprites[4]; 
        }
        else if (!isSpriteSwitch)
        {
            homeKeyImg[1].sprite = homeSprites[2];
            optionsKeyImg[1].sprite = optionsSprites[2];
            quitKeyImg[1].sprite = quitSprites[2];
            leftArrowKeyImg[1].sprite = leftArrowSprites[2];
            rightArrowKeyImg[1].sprite = rightArrowSprites[2];
            escapeKeyImg[1].sprite = escapeSprites[2];
        }
    }

    private void OnEnableMenu()
    {
        if (cantOpenPauseMenu) return;

        if (!references.GetPlayerController().disableControl && !references.GetPlayerController().GetIsInActivity())
        {
            EnablePauseMenu();
        }
    }

    private void OnAddVolume()
    {
        if (!pauseMenu.activeInHierarchy) return;
        if (fillImg.fillAmount == 1) return;
        if (selectionIndex != 2) return;

        StartCoroutine(SwitchKeySprite("Right"));

        ChangeVolume(0.05f);
    }

    private void OnRemoveVolume()
    {
        if (!pauseMenu.activeInHierarchy) return;
        if (fillImg.fillAmount == 0) return;
        if (selectionIndex != 2) return;

        StartCoroutine(SwitchKeySprite("Left"));

        ChangeVolume(-0.05f);
    }

    private void OnReturnHome()
    {
        if (!pauseMenu.activeInHierarchy) return;
        if (SceneManager.GetActiveScene().name == "Home") return;
        if (SceneManager.GetActiveScene().name == "Tuto") return;
        if (selectionIndex != 0) return;

        StartCoroutine(DisablePauseMenu("R"));
    }

    private void OnDisableMenu()
    {
        if (!pauseMenu.activeInHierarchy) return;

        if (!references.GetGameManager().GetKeyboardControl() && Gamepad.current.buttonEast.wasPressedThisFrame)
        {
            cantReturn = false;
        }

        if (cantReturn)
        {
            cantReturn = false;
            return;
        }

        StartCoroutine(DisablePauseMenu("Escape"));
    }

    private void OnOpenControllsMenu()
    {
        if (!pauseMenu.activeInHierarchy) return;
        if (selectionIndex != 1) return;

        StartCoroutine(SwitchKeySprite("C"));
    }

    private void OnQuit()
    {
        if (!pauseMenu.activeInHierarchy) return;
        if (selectionIndex != 3) return;

        StartCoroutine(DisablePauseMenu("Q"));
    }

    private void OnGoUp()
    {
        if (!pauseMenu.activeInHierarchy) return;

        selectionAS.Play();

        selectionIndex--;

        if (selectionIndex < 0)
        {
            selectionIndex = optionBackgrounds.Length - 1;
        }

        RefreshSelectedOption();
    }

    private void OnGoDown()
    {
        if (!pauseMenu.activeInHierarchy) return;

        selectionAS.Play();

        selectionIndex++;

        if (selectionIndex == optionBackgrounds.Length)
        {
            selectionIndex = 0;
        }

        RefreshSelectedOption();
    }

    private void RefreshSelectedOption()
    {
        for (int i = 0; i < optionBackgrounds.Length; i++)
        {
            if (i == selectionIndex)
            {
                optionBackgrounds[i].sprite = optionBackgroundSprites[0];
                notSelectedImgs[i].enabled = false;
            }
            else
            {
                optionBackgrounds[i].sprite = optionBackgroundSprites[1];
                notSelectedImgs[i].enabled = true;
            }
        }
    }

    public void EnableControllsMenu(bool state)
    {
        controllsMenu.SetActive(state);
        pauseMenu.SetActive(!state);
    }

    public void EnablePauseMenu()
    {
        cantReturn = true;
        inPauseMenu = true;

        selectionAS.Play();

        Time.timeScale = 0;

        pauseMenu.SetActive(true);

        references.GetPlayerController().disableControl = true;
        references.GetPlayerController().SetIsInActivity(true);
    }

    private IEnumerator DisablePauseMenu(string key)
    {
        yield return SwitchKeySprite(key);

        if (key == "Escape")
        {
            references.GetPlayerController().disableControl = false;
            references.GetPlayerController().SetIsInActivity(false);
        }

        inPauseMenu = false;

        pauseMenu.SetActive(false);
    }

    private void ChangeVolume(float volumeValue)
    {
        fillImg.fillAmount += volumeValue;

        references.GetGameManager().ChangeVolume(fillImg.fillAmount);

        volumePercentageTxt.text = Mathf.RoundToInt(references.GetGameManager().GetCurrentAudioVolume() * 100).ToString() + "%";
    }

    public bool GetInPauseMenu()
    {
        return inPauseMenu;
    }

    public void SwitchCantOpenPauseMenu(bool state)
    {
        cantOpenPauseMenu = state;
    }

    private IEnumerator SwitchKeySprite(string key)
    {
        isSpriteSwitch = true;

        if (references.GetGameManager().GetKeyboardControl())
        {
            if (key == "R")
            {
                Time.timeScale = 1f;

                homeKeyImg[0].sprite = homeSprites[1];

                yield return new WaitForSeconds(timeBetweenLetterSwitch);

                homeKeyImg[0].sprite = homeSprites[0];

                StartCoroutine(references.GetGameManager().ReturnHome(true));
            }
            else if (key == "Q")
            {
                Time.timeScale = 1f;

                quitKeyImg[0].sprite = quitSprites[1];
                
                yield return new WaitForSeconds(timeBetweenLetterSwitch);

                quitKeyImg[0].sprite = quitSprites[0];

                StartCoroutine(references.GetGameManager().Quit());
            }
            else if (key == "Escape")
            {
                Time.timeScale = 1f;

                escapeKeyImg[0].sprite = escapeSprites[1];

                yield return new WaitForSeconds(timeBetweenLetterSwitch);

                escapeKeyImg[0].sprite = escapeSprites[0];
            }
            else if (key == "C")
            {
                optionsKeyImg[0].sprite = optionsSprites[1];

                yield return new WaitForSecondsRealtime(timeBetweenLetterSwitch);

                optionsKeyImg[0].sprite = optionsSprites[0];

                EnableControllsMenu(true);
            }
            else if (key == "Left")
            {
                leftArrowKeyImg[0].sprite = leftArrowSprites[1];

                yield return new WaitForSecondsRealtime(timeBetweenLetterSwitch);

                leftArrowKeyImg[0].sprite = leftArrowSprites[0];
            }
            else if (key == "Right")
            {
                rightArrowKeyImg[0].sprite = rightArrowSprites[1];

                yield return new WaitForSecondsRealtime(timeBetweenLetterSwitch);

                rightArrowKeyImg[0].sprite = rightArrowSprites[0];
            }
        }
        else
        {
            if (key == "R")
            {
                Time.timeScale = 1f;

                if (references.GetGameManager().GetChangeUI())
                {
                    homeKeyImg[1].sprite = homeSprites[5];

                    yield return new WaitForSeconds(timeBetweenLetterSwitch);

                    homeKeyImg[1].sprite = homeSprites[4];
                }
                else
                {
                    homeKeyImg[1].sprite = homeSprites[3];

                    yield return new WaitForSeconds(timeBetweenLetterSwitch);

                    homeKeyImg[1].sprite = homeSprites[2];
                }

                StartCoroutine(references.GetGameManager().ReturnHome(true));
            }
            else if (key == "Q")
            {
                Time.timeScale = 1f;

                if (references.GetGameManager().GetChangeUI())
                {
                    quitKeyImg[1].sprite = quitSprites[5];

                    yield return new WaitForSeconds(timeBetweenLetterSwitch);

                    quitKeyImg[1].sprite = quitSprites[4];
                }
                else
                {
                    quitKeyImg[1].sprite = quitSprites[3];

                    yield return new WaitForSeconds(timeBetweenLetterSwitch);

                    quitKeyImg[1].sprite = quitSprites[2];
                }

                StartCoroutine(references.GetGameManager().Quit());
            }
            else if (key == "Escape")
            {
                Time.timeScale = 1f;

                if (references.GetGameManager().GetChangeUI())
                {
                    escapeKeyImg[1].sprite = escapeSprites[5];

                    yield return new WaitForSeconds(timeBetweenLetterSwitch);

                    escapeKeyImg[1].sprite = escapeSprites[4];
                }
                else
                {
                    escapeKeyImg[1].sprite = escapeSprites[3];

                    yield return new WaitForSeconds(timeBetweenLetterSwitch);

                    escapeKeyImg[1].sprite = escapeSprites[2];
                }
            }
            else if (key == "C")
            {
                if (references.GetGameManager().GetChangeUI())
                {
                    optionsKeyImg[1].sprite = optionsSprites[5];

                    yield return new WaitForSecondsRealtime(timeBetweenLetterSwitch);

                    optionsKeyImg[1].sprite = optionsSprites[4];
                }
                else
                {
                    optionsKeyImg[1].sprite = optionsSprites[3];

                    yield return new WaitForSecondsRealtime(timeBetweenLetterSwitch);

                    optionsKeyImg[1].sprite = optionsSprites[2];
                }

                EnableControllsMenu(true);
            }
            else if (key == "Left")
            {
                if (references.GetGameManager().GetChangeUI())
                {
                    leftArrowKeyImg[1].sprite = leftArrowSprites[5];

                    yield return new WaitForSecondsRealtime(timeBetweenLetterSwitch);

                    leftArrowKeyImg[1].sprite = leftArrowSprites[4];
                }
                else
                {
                    leftArrowKeyImg[1].sprite = leftArrowSprites[3];

                    yield return new WaitForSecondsRealtime(timeBetweenLetterSwitch);

                    leftArrowKeyImg[1].sprite = leftArrowSprites[2];
                }
            }
            else if (key == "Right")
            {
                if (references.GetGameManager().GetChangeUI())
                {
                    rightArrowKeyImg[1].sprite = rightArrowSprites[5];

                    yield return new WaitForSecondsRealtime(timeBetweenLetterSwitch);

                    rightArrowKeyImg[1].sprite = rightArrowSprites[4];
                }
                else
                {
                    rightArrowKeyImg[1].sprite = rightArrowSprites[3];

                    yield return new WaitForSecondsRealtime(timeBetweenLetterSwitch);

                    rightArrowKeyImg[1].sprite = rightArrowSprites[2];
                }
            }
        }

        selectionAS.Play();

        isSpriteSwitch = false;
    }
}

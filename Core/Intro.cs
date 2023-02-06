using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Intro : MonoBehaviour
{
    [SerializeField] Fader fader;
    [SerializeField] float fadeDuration;
    [SerializeField] float timeBetweenLetter;
    [SerializeField] TextMeshProUGUI dialogueText;
    [SerializeField] Image keyToPress;
    [SerializeField] Sprite[] keySprites;
    [SerializeField] AudioSource selectionAS;
    [SerializeField] AudioSource textAS;

    CatchReferences references;
    PlayerInputSystem inputActions;

    char[] charArray;
    const float timeBetweenKeySwap = 0.07f;
    bool keyPressed;

    private void Awake()
    {
        references = FindObjectOfType<CatchReferences>();

        inputActions = new();
        inputActions.UIIntro.KeyPressed.performed += ctx => OnKeyPressed();

        inputActions.UIIntro.Enable();

        keyToPress.enabled = false;
    }

    private void OnDestroy()
    {
        inputActions.UIIntro.Disable();
    }

    private void Update()
    {
        RefreshUI();
    }

    private void RefreshUI()
    {
        if (keyPressed) return;
        
        if (references.GetGameManager().GetKeyboardControl())
        {
            keyToPress.sprite = keySprites[0];
        }
        else
        {
            if (references.GetGameManager().GetChangeUI())
            {
                keyToPress.sprite = keySprites[4];
            }
            else
            {
                keyToPress.sprite = keySprites[2];
            }
        }
    }

    public IEnumerator IntroDialogue(int prisonnerNumber)
    {
        references.GetPlayerController().disableControl = true;

        references.GetPauseMenu().SwitchCantOpenPauseMenu(true);

        yield return DialogueAnimation("Building a new clone...");
        yield return WaitUntilKeyPress();

        string prinonnerNumberTxt = SetupPrisonnerNumber(prisonnerNumber);

        yield return DialogueAnimation(prinonnerNumberTxt);
        yield return WaitUntilKeyPress();

        references.GetPauseMenu().SwitchCantOpenPauseMenu(false);

        yield return LaunchFader();
    }

    private string SetupPrisonnerNumber(int prisonnerNumber)
    {
        string prisonnerNumberTxt = "Clone number ";

        if (prisonnerNumber < 10)
        {
            prisonnerNumberTxt += "00" + prisonnerNumber;
        }
        else if (prisonnerNumber < 99)
        {
            prisonnerNumberTxt += "0" + prisonnerNumber;
        }
        else
        {
            prisonnerNumberTxt += prisonnerNumber;
        }

        prisonnerNumberTxt += " ready !";

        return prisonnerNumberTxt;
    }

    private IEnumerator DialogueAnimation(string textToAnimate)
    {
        keyToPress.enabled = false;

        charArray = textToAnimate.ToCharArray();
        dialogueText.text = "";

        for (int i = 0; i < charArray.Length; i++)
        {
            dialogueText.text += charArray[i].ToString();

            if (!textAS.isPlaying)
            {
                textAS.Play();
            }

            keyPressed = false;

            yield return new WaitForSeconds(timeBetweenLetter);
        }

        keyToPress.enabled = true;
    }

    private IEnumerator LaunchFader()
    {
        dialogueText.enabled = false;
        keyToPress.enabled = false;
        references.GetPlayerController().disableControl = false;

        StartCoroutine(fader.FadeIn(fadeDuration));

        yield return new WaitForSeconds(0.2f);
    }

    private void OnKeyPressed()
    {
        if (keyToPress.enabled == false) return;

        keyPressed = true;
    }

    private IEnumerator WaitUntilKeyPress()
    {
        RefreshUI();

        while (!keyPressed)
        {
            yield return null;
        }

        yield return SwitchSprite();
    }

    private IEnumerator SwitchSprite()
    {
        if (references.GetGameManager().GetKeyboardControl())
        {
            keyToPress.sprite = keySprites[1];

            yield return new WaitForSeconds(timeBetweenKeySwap);

            keyToPress.sprite = keySprites[0];
        }
        else
        {
            if (references.GetGameManager().GetChangeUI())
            {
                keyToPress.sprite = keySprites[5];

                yield return new WaitForSeconds(timeBetweenKeySwap);

                keyToPress.sprite = keySprites[4];
            }
            else
            {
                keyToPress.sprite = keySprites[3];

                yield return new WaitForSeconds(timeBetweenKeySwap);

                keyToPress.sprite = keySprites[2];
            }
        }

        selectionAS.Play();

        keyPressed = false;
    }
}

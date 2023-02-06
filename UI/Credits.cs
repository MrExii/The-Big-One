using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Credits : MonoBehaviour
{
    [SerializeField] Animator creditAnimator;
    [SerializeField] AnimationClip scrollAnimation;
    [SerializeField] GameObject[] keyToSkip;
    [SerializeField] Image[] keyToSkipImgs;
    [SerializeField] Sprite[] keyToSkipSprites;
    [SerializeField] Fader fader;

    PlayerInputSystem inputActions;
    GameManager gameManager;

    bool keyPressed;
    bool canExit;

    readonly float fadingTime = 3f;
    float timeSinceStart;
    const float timeBetweenKeySwap = 0.07f;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    private void Start()
    {
        Invoke(nameof(StartScroll), 4f); //+- Fader timer

        StartCoroutine(ReturnHome());

        inputActions = new();
        inputActions.UICredits.Skip.performed += ctx => OnPressSkipButton();

        inputActions.UICredits.Enable();

        keyToSkip[0].SetActive(false);
        keyToSkip[1].SetActive(false);
    }

    private void Update()
    {
        RefreshUI();

        timeSinceStart += Time.deltaTime;

        if (fadingTime < timeSinceStart)
        {
            canExit = true;
        }
    }

    private void RefreshUI()
    {
        if (keyPressed) return;

        if (gameManager.GetKeyboardControl())
        {
            if (canExit)
            {
                keyToSkip[0].SetActive(true);
                keyToSkip[1].SetActive(false);
            }
        }
        else
        {
            if (canExit)
            {
                keyToSkip[0].SetActive(false);
                keyToSkip[1].SetActive(true);
            }

            if (gameManager.GetChangeUI())
            {
                keyToSkipImgs[1].sprite = keyToSkipSprites[4];
            }
            else
            {
                keyToSkipImgs[1].sprite = keyToSkipSprites[2]; 
            }
        }
    }

    private void OnPressSkipButton()
    {
        if (!canExit) return;

        StartCoroutine(ReturnHomeEarly());
    }

    private void StartScroll()
    {
        creditAnimator.SetTrigger("start");
    }

    private IEnumerator ReturnHome()
    {
        yield return new WaitForSeconds(scrollAnimation.length);

        inputActions.UICredits.Disable();

        yield return fader.FadeOut(fadingTime);

        StartCoroutine(gameManager.ReturnHome(false));
    }

    private IEnumerator ReturnHomeEarly()
    {
        StopCoroutine(ReturnHome());

        yield return SwitchKeySprites();

        inputActions.UICredits.Disable();

        yield return fader.FadeOut(1f);

        StartCoroutine(gameManager.ReturnHome(false));
    }

    private IEnumerator SwitchKeySprites()
    {
        keyPressed = true;

        if (gameManager.GetKeyboardControl())
        {
            keyToSkipImgs[0].sprite = keyToSkipSprites[1];

            yield return new WaitForSeconds(timeBetweenKeySwap);

            keyToSkipImgs[0].sprite = keyToSkipSprites[0];
        }
        else
        {
            if (gameManager.GetChangeUI())
            {
                keyToSkipImgs[1].sprite = keyToSkipSprites[5];

                yield return new WaitForSeconds(timeBetweenKeySwap);

                keyToSkipImgs[1].sprite = keyToSkipSprites[4];
            }
            else
            {
                keyToSkipImgs[1].sprite = keyToSkipSprites[3];

                yield return new WaitForSeconds(timeBetweenKeySwap);

                keyToSkipImgs[1].sprite = keyToSkipSprites[2];
            }
        }

        keyPressed = false;
    }
}

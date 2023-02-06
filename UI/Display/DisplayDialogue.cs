using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DisplayDialogue : MonoBehaviour
{
    [SerializeField] float timeBetweenLetter;
    [SerializeField] RectTransform dialogueBubbleRectTransform;
    [SerializeField] RectTransform textDialogueRectTransform;
    [SerializeField] TextMeshProUGUI dialogueText;
    [SerializeField] Image keyToPress;
    [SerializeField] Sprite[] keySprites;
    [SerializeField] Image[] escapeKeyImg;
    [SerializeField] Sprite[] escapeKeySprites;
    [SerializeField] AudioSource dialogueAS;
    [SerializeField] AudioSource selectionAS;
    [SerializeField] AudioSource textAS;
    [SerializeField] RectTransform dialogueRectTransform;

    GameManager gameManager;
    PlayerInputSystem inputActions;

    char[] charArray;

    const float timeBetweenKeySpriteSwap = 0.05f;

    bool isKeySwap;
    bool pressKey;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();

        inputActions = new();
        inputActions.Dialogues.NavDown.performed += ctx => OnPressKey();
    }

    private void OnDisable()
    {
        inputActions.Dialogues.Disable();
    }

    private void Start()
    {
        keyToPress.gameObject.SetActive(false);

        escapeKeyImg[0].enabled = false;
        escapeKeyImg[1].enabled = false;

        gameObject.SetActive(false);
    }

    private void Update()
    {
        RefreshUI();
    }

    private void RefreshUI()
    {
        if (isKeySwap) return;

        if (gameManager.GetKeyboardControl())
        {
            keyToPress.sprite = keySprites[0];
        }
        else
        {
            if (gameManager.GetChangeUI())
            {
                keyToPress.sprite = keySprites[4];
                escapeKeyImg[1].sprite = escapeKeySprites[1];
            }
            else
            {
                keyToPress.sprite = keySprites[2];
                escapeKeyImg[1].sprite = escapeKeySprites[0];
            }
        }
    }

    private void OnPressKey()
    {
        if (!gameObject.activeInHierarchy) return;

        pressKey = true;
    }

    public IEnumerator LaunchDialogue(string dialogueText, AudioClip vocalSFX, FontStyles fontStyle)
    {
        gameObject.SetActive(true);

        this.dialogueText.fontStyle = fontStyle;

        if (vocalSFX && !dialogueAS.isPlaying)
        {
            dialogueAS.clip = vocalSFX;
            dialogueAS.Play();
        }

        yield return DialogueAnimation(dialogueText);

        yield return WaitUntilKeyPress();

        gameObject.SetActive(false);

        pressKey = false;
    }

    public void DisableDisplayDialogue()
    {
        gameObject.SetActive(false);
    }

    private IEnumerator DialogueAnimation(string textToAnimate)
    {
        if (dialogueRectTransform.sizeDelta.x > 600)
        {
            dialogueRectTransform.sizeDelta = new(600, dialogueRectTransform.sizeDelta.y);
        }

        keyToPress.gameObject.SetActive(false);
        escapeKeyImg[0].enabled = false;
        escapeKeyImg[1].enabled = false;

        charArray = textToAnimate.ToCharArray();
        dialogueText.text = "";

        for (int i = 0; i < charArray.Length; i++)
        {
            dialogueText.text += charArray[i].ToString();

            LayoutRebuilder.ForceRebuildLayoutImmediate(textDialogueRectTransform);
            LayoutRebuilder.ForceRebuildLayoutImmediate(dialogueBubbleRectTransform);

            if (!textAS.isPlaying)
            {
                textAS.Play();
            }

            pressKey = false;

            yield return new WaitForSeconds(timeBetweenLetter);
        }

        
        dialogueRectTransform.sizeDelta = new(dialogueRectTransform.sizeDelta.x + 68.75f, dialogueRectTransform.sizeDelta.y);

        keyToPress.gameObject.SetActive(true);
    }

    private IEnumerator WaitUntilKeyPress()
    {
        inputActions.Dialogues.Enable();

        while (!pressKey)
        {
            yield return null;
        }

        yield return SwitchKeySprite();

        inputActions.Dialogues.Disable();
    }

    public IEnumerator SwitchKeySprite()
    {
        isKeySwap = true;

        selectionAS.Play();

        if (gameManager.GetKeyboardControl())
        {
            keyToPress.sprite = keySprites[1];

            yield return new WaitForSeconds(timeBetweenKeySpriteSwap);

            keyToPress.sprite = keySprites[0];
        }
        else
        {
            if (gameManager.GetChangeUI())
            {
                keyToPress.sprite = keySprites[5];

                yield return new WaitForSeconds(timeBetweenKeySpriteSwap);

                keyToPress.sprite = keySprites[4];
            }
            else
            {
                keyToPress.sprite = keySprites[3];

                yield return new WaitForSeconds(timeBetweenKeySpriteSwap);

                keyToPress.sprite = keySprites[2];
            }
        }

        keyToPress.gameObject.SetActive(false);
        isKeySwap = false;
    }

    public void EnableEscapeKey()
    {
        if (gameManager.GetKeyboardControl())
        {
            escapeKeyImg[0].enabled = true;
        }
        else
        {
            escapeKeyImg[1].enabled = true;
        }

        dialogueText.text = "Make your choice :";
    }
}

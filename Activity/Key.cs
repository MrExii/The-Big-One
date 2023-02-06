using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Key : MonoBehaviour
{
    [System.Serializable]
    private class KeySprites
    {
        public KeyIndex key;
        public Sprite[] keySprites;
    }

    [System.Serializable]
    private class ControllerSprites
    {
        public KeyIndex key;
        public Sprite[] controllerSprites;
    }

    [SerializeField] KeySprites[] keys;
    [SerializeField] ControllerSprites[] controllerKeys;
    [SerializeField] Image keyImg;

    GameManager gameManager;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    public void SetKeySprite(KeyIndex index)
    {
        if (gameManager.GetKeyboardControl())
        {
            foreach (KeySprites item in keys)
            {
                if (item.key == index)
                {
                    keyImg.sprite = item.keySprites[0];
                }
            }
        }
        else
        {
            if (gameManager.GetChangeUI())
            {
                foreach (ControllerSprites item in controllerKeys)
                {
                    if (item.key == index)
                    {
                        if (index == KeyIndex.leftStickButton || index == KeyIndex.rightStickButton)
                        {
                            keyImg.sprite = item.controllerSprites[0];
                        }
                        else
                        {
                            keyImg.sprite = item.controllerSprites[2];
                        }
                    }
                }
            }
            else
            {
                foreach (ControllerSprites item in controllerKeys)
                {
                    if (item.key == index)
                    {
                        keyImg.sprite = item.controllerSprites[0];
                    }
                }
            }
        }
    }

    public void SwitchKeySprites(KeyIndex index)
    {
        if (gameManager.GetKeyboardControl())
        {
            foreach (KeySprites item in keys)
            {
                if (item.key == index)
                {
                    keyImg.sprite = item.keySprites[1];
                }
            }
        }
        else
        {
            if (gameManager.GetChangeUI())
            {
                foreach (ControllerSprites item in controllerKeys)
                {
                    if (item.key == index)
                    {
                        if (index == KeyIndex.leftStickButton || index == KeyIndex.rightStickButton)
                        {
                            keyImg.sprite = item.controllerSprites[1];
                        }
                        else
                        {
                            keyImg.sprite = item.controllerSprites[3];
                        }
                    }
                }
            }
            else
            {
                foreach (ControllerSprites item in controllerKeys)
                {
                    if (item.key == index)
                    {
                        keyImg.sprite = item.controllerSprites[1];
                    }
                }
            }
        }
    }

    public void EnableKey(bool state)
    {
        keyImg.enabled = state;
    }
}

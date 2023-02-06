using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PotionsDisplay : MonoBehaviour
{
    [System.Serializable]
    private class PotionsSettings
    {
        public PotionsType potionType;
        public Sprite[] potionSprites;
    }

    [SerializeField] PotionsSettings[] potionsSettings;
    [SerializeField] Image potionImg;

    public void UpdatePotion(PotionsType potionType, float numberOfPotions)
    {
        foreach (var potion in potionsSettings)
        {
            if (potion.potionType == potionType)
            {
                potionImg.sprite = potion.potionSprites[(int)numberOfPotions];
            }
        }
    }
}

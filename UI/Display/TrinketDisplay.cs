using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrinketDisplay : MonoBehaviour
{
    [System.Serializable]
    private class TrinketSettings
    {
        public TrinketIndex trinketIndex;
        public Sprite trinketSprite;
        public Sprite chargeSprite;
    }

    [SerializeField] AnimationClip disappearAnimation;
    [SerializeField] Image trinketImg;
    [SerializeField] Animator trinketsAnimator;
    [SerializeField] Animator usableAnimator;
    [SerializeField] Animator addChargeAnimator;
    [SerializeField] TrinketSettings[] trinketSettings;

    TrinketIndex currentTrinket;

    public IEnumerator SetTrinketSprite(TrinketIndex trinketIndex)
    {
        trinketsAnimator.enabled = true;

        if (currentTrinket != TrinketIndex.None)
        {
            trinketsAnimator.SetTrigger(currentTrinket.ToString() + "Disappear");
        }

        yield return new WaitForSeconds(disappearAnimation.length);

        currentTrinket = trinketIndex;

        trinketsAnimator.SetTrigger(trinketIndex.ToString() + "Appear");

        yield return new WaitForSeconds(disappearAnimation.length);

        trinketsAnimator.enabled = false;

        foreach (var trinket in trinketSettings)
        {
            if (trinket.trinketIndex == trinketIndex)
            {
                trinketImg.sprite = trinket.trinketSprite;
            }
        }
    }

    public void TrinketCharged(bool isCharged, TrinketIndex trinketIndex)
    {
        usableAnimator.SetBool("usable", isCharged);

        if (isCharged)
        {
            foreach (var trinket in trinketSettings)
            {
                if (trinket.trinketIndex == trinketIndex)
                {
                    trinketImg.sprite = trinket.chargeSprite;
                }
            }
        }
        else
        {
            foreach (var trinket in trinketSettings)
            {
                if (trinket.trinketIndex == trinketIndex)
                {
                    trinketImg.sprite = trinket.trinketSprite;
                }
            }
        }
    }

    public void AddCharge()
    {
        addChargeAnimator.SetTrigger("addCharge");
    }
}

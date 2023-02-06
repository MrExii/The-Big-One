using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChargeBar : MonoBehaviour
{
    [SerializeField] Image chargeBar;
    [SerializeField] ColorGradient colorGradient;
    [SerializeField] RectTransform rectTransform;

    public void ShootChargeBar(float currentTime, float maxTimer)
    {
        float fillAmount = currentTime / maxTimer;

        chargeBar.fillAmount = fillAmount;

        chargeBar.color = colorGradient.UpdateColorGradient(fillAmount);
    }

    public void WeaponUpdateChargeBar(float currentTime, float maxTimer)
    {
        float fillAmount = currentTime / maxTimer;

        chargeBar.fillAmount = 1 - fillAmount;
    }

    public void PotionUpdateChargeBar(float currentTime, float maxTimer)
    {
        float fillAmount = currentTime / maxTimer;
        
        chargeBar.fillAmount = fillAmount;
    }

    public void FlipPosition()
    {
        Vector3 position = rectTransform.localPosition;

        rectTransform.localPosition = new Vector2(position.x * -1, position.y);
    }
}

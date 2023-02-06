using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CurseDisplay : MonoBehaviour
{
    [SerializeField] GameObject curse;
    [SerializeField] Image fillImg;
    [SerializeField] Image curseBackgroundImg;
    [SerializeField] TextMeshProUGUI curseTxt;
    [SerializeField] float speed;
    [SerializeField] float timeBeforeUpdateBackgroundImg;

    CatchReferences references;

    float currentCurseFillAmount = 0f;
    float timeSinceLastCurseBarUpdate;

    private void Awake()
    {
        references = FindObjectOfType<CatchReferences>();
    }

    private void Start()
    {
        fillImg.fillAmount = 0f;

        RefreshCurse(0);
        EnableCurse(false);
        DisableUI();
    }

    private void Update()
    {
        if (timeSinceLastCurseBarUpdate > timeBeforeUpdateBackgroundImg)
        {
            RefreshBackgroundCurse();
        }

        timeSinceLastCurseBarUpdate += Time.deltaTime;
    }

    private void DisableUI()
    {
        if (!references.GetSimulationsPlaceHolder().GetDisableUI()) return;

        curseTxt.text = "?";
        curseBackgroundImg.fillAmount = 1;
    }

    public void RefreshCurse(float currentCurse)
    {
        if (references.GetSimulationsPlaceHolder().GetDisableUI()) return;

        curseTxt.text = Mathf.CeilToInt(currentCurse) + "/100";
        curseBackgroundImg.fillAmount = currentCurse / 100;

        timeBeforeUpdateBackgroundImg = 0f;
    }

    private void RefreshBackgroundCurse()
    {
        if (fillImg.fillAmount < curseBackgroundImg.fillAmount)
        {
            float value = curseBackgroundImg.fillAmount - fillImg.fillAmount;

            currentCurseFillAmount += value * speed * Time.deltaTime;

            if (curseBackgroundImg.fillAmount > fillImg.fillAmount &&
                curseBackgroundImg.fillAmount < fillImg.fillAmount + 0.004f)
            {
                curseBackgroundImg.fillAmount = fillImg.fillAmount;
            }
            else
            {
                fillImg.fillAmount = currentCurseFillAmount;
            }
        }
        else
        {
            fillImg.fillAmount = curseBackgroundImg.fillAmount;

            currentCurseFillAmount = fillImg.fillAmount;
        }
    }

    public void EnableCurse(bool state)
    {
        curse.SetActive(state);
    }
}

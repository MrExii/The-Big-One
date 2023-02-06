using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fader : MonoBehaviour
{
    [SerializeField] bool fadeInStart;
    [SerializeField] float fadeInStartTimer;

    CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private IEnumerator Start()
    {
        if (fadeInStart)
        {
            yield return FadeIn(fadeInStartTimer);
        }
    }

    public IEnumerator FadeOut(float fadeDuration)
    {
        while (canvasGroup.alpha < 1f)
        {
            canvasGroup.alpha += Time.deltaTime / fadeDuration;

            yield return null;
        }
    }

    public IEnumerator FadeIn(float fadeDuration)
    {
        while (canvasGroup.alpha > 0f)
        {
            canvasGroup.alpha -= Time.deltaTime / fadeDuration;

            yield return null;
        }
    }

    public void SectorReload(float fade)
    {
        canvasGroup.alpha = fade;
    }

    public void SetCanvasGroupAlpha(float value)
    {
        canvasGroup.alpha = value;
    }
}

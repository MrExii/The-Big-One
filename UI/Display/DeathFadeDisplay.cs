using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeathFadeDisplay : MonoBehaviour
{
    [SerializeField] Image[] topAndBottom;
    [SerializeField] AudioSource audioSource;

    readonly float duration = 1.5f;

    public IEnumerator DeathFade()
    {
        audioSource.Play();

        while (topAndBottom[0].fillAmount < 1f)
        {
            topAndBottom[0].fillAmount += Time.deltaTime / duration;
            topAndBottom[1].fillAmount += Time.deltaTime / duration;

            yield return null;
        }
    }
}

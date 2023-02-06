using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndScene : MonoBehaviour
{
    [SerializeField] bool loadCredit;
    [SerializeField] GameManager gameManager;
    [SerializeField] Fader fader;

    private IEnumerator Start()
    {
        yield return fader.FadeIn(1f);
    }

    private void Update()
    {
        if (loadCredit)
        {
            loadCredit = false;

            StartCoroutine(gameManager.LoadCredits());
        }
    }
}

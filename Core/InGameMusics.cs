using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameMusics : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip[] inGameMusics;

    float musicLenght;

    private void Start()
    {
        StartCoroutine(EnableMusic());
        StartCoroutine(TakeMusic());
    }

    private IEnumerator EnableMusic()
    {
        float volume = audioSource.volume;
        audioSource.volume = 0;

        while (audioSource.volume < volume)
        {
            audioSource.volume += Time.deltaTime;

            if (audioSource.volume > volume)
            {
                audioSource.volume = volume;
            }

            yield return new WaitForSeconds(Time.deltaTime);
        }
    }

    private IEnumerator TakeMusic()
    {
        while (true)
        {
            int index = Random.Range(0, inGameMusics.Length);

            musicLenght = inGameMusics[index].length;

            audioSource.PlayOneShot(inGameMusics[index]);

            yield return new WaitForSeconds(musicLenght);
        }
    }
}

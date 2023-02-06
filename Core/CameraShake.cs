using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    [SerializeField] Transform camTransform;

    float shakeDuration;
    float shakeAmount;
    float decreaseFactor;

    Vector3 originalPos;

    void OnEnable()
    {
        originalPos = camTransform.localPosition;
    }

    void Update()
    {
        if (shakeDuration > 0)
        {
            camTransform.localPosition = originalPos + Random.insideUnitSphere * shakeAmount;

            shakeDuration -= Time.deltaTime * decreaseFactor;
        }
        else
        {
            shakeDuration = 0f;
            camTransform.localPosition = originalPos;
        }
    }

    public void Shake(float shakeDuration, float shakeAmount, float decreaseFactor)
    {
        this.shakeDuration = shakeDuration;
        this.shakeAmount = shakeAmount;
        this.decreaseFactor = decreaseFactor;
    }
}
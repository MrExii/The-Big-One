using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorGradient : MonoBehaviour
{
    [SerializeField] Color color1;
    [SerializeField] Color color2;
    [SerializeField] Color color3;

    Gradient gradient = new();

    private void Start()
    {
        SetupColorGradient();
    }

    private void SetupColorGradient()
    {
        GradientColorKey[] colorKey = new GradientColorKey[3];
        GradientAlphaKey[] alphaKey = new GradientAlphaKey[3];

        colorKey[0].color = color1;
        colorKey[0].time = 0f;

        colorKey[1].color = color2;
        colorKey[1].time = 0.5f;

        colorKey[2].color = color3;
        colorKey[2].time = 1f;

        alphaKey[0].alpha = 1f;
        alphaKey[0].time = 0f;

        alphaKey[1].alpha = 1f;
        alphaKey[1].time = 0.5f;

        alphaKey[2].alpha = 1f;
        alphaKey[2].time = 1f;

        gradient.SetKeys(colorKey, alphaKey);
    }

    public Color UpdateColorGradient(float value)
    {
        return gradient.Evaluate(value);
    }
}

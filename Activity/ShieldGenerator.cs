using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.Rendering.Universal;
using static UnityEngine.EventSystems.EventTrigger;

public class ShieldGenerator : MonoBehaviour
{
    [SerializeField] float shieldPercentage;
    [SerializeField] EnemyHealth[] enemies;
    [SerializeField] SpriteRenderer shieldEffect;
    [SerializeField] Light2D spotLight;
    [SerializeField] Animator shieldBaseAnimator;
    [SerializeField] Material defaultMaterial;
    [SerializeField] GameObject emptyGameObject;
    [SerializeField] Activity activity;

    LineRenderer[] lines;

    bool isShieldActivate = true;

    private void Start()
    {
        lines = new LineRenderer[enemies.Length];
        ColorUtility.TryParseHtmlString("#a3fffc", out Color lineColor);

        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i].IncreaseHealth(shieldPercentage / 100);

            GameObject emptyPrefab = Instantiate(emptyGameObject, transform.position, Quaternion.identity, transform);

            LineRenderer newLine = emptyPrefab.AddComponent<LineRenderer>();

            lines[i] = newLine;
        }

        foreach (var line in lines)
        {
            line.startColor = lineColor;
            line.endColor = lineColor;

            line.startWidth = 0.05f;
            line.endWidth = 0.05f;

            line.positionCount = 2;

            line.sortingLayerName = "LineRenderer";

            line.SetPosition(0, new Vector2(transform.position.x, transform.position.y + 0.2f));

            line.material = defaultMaterial;
        }
    }

    private void Update()
    {
        if (activity.GetIsActivityCompleted() && isShieldActivate)
        {
            isShieldActivate = false;

            StartCoroutine(DisableShield());
        }
        
        if (isShieldActivate)
        {
            UpdateLine();
        }
    }

    private void UpdateLine()
    {
        for (int i = 0; i < lines.Length; i++)
        {
            if (enemies[i].GetIsDead() && lines[i].gameObject.activeInHierarchy)
            {
                lines[i].gameObject.SetActive(false);
            }
            else
            {
                lines[i].SetPosition(1, enemies[i].transform.position);
            }
        }
    }

    private IEnumerator DisableShield()
    {
        float fraction = 1;

        foreach (var enemy in enemies)
        {
            if (enemy.GetIsDead()) continue;

            enemy.DecreaseHealth(shieldPercentage / 100);
        }

        foreach (var line in lines)
        {
            line.enabled = false;
        }

        while (shieldEffect.color.a > 0)
        {
            fraction -= Time.deltaTime;

            shieldEffect.color = new(shieldEffect.color.r, shieldEffect.color.g, shieldEffect.color.b, fraction);
            spotLight.intensity = fraction * 1.4f;

            yield return null;
        }

        shieldBaseAnimator.enabled = false;
    }
}

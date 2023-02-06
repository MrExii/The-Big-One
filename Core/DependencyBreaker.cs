using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DependencyBreaker : MonoBehaviour
{
    [SerializeField] string[] scriptNames;

    MonoBehaviour[] monoBehaviours;

    private void Awake()
    {
        monoBehaviours = new MonoBehaviour[scriptNames.Length];

        for (int i = 0; i < scriptNames.Length; i++)
        {
            Type type = Type.GetType(scriptNames[i]);

            monoBehaviours[i] = (MonoBehaviour)GetComponent(type);
        }
    }

    public Component GetMonoBehaviour(string scriptName)
    {
        foreach (MonoBehaviour script in monoBehaviours)
        {
            if (script.ToString() == scriptName)
            {
                return script;
            }
        }

        return null;
    }
}

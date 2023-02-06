using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[CreateAssetMenu(fileName = "New Key Series", menuName = "New Key Series", order = 0)]
public class KeySeriesSO : ScriptableObject
{
    [SerializeField] KeyIndex[] keySeries;
    [SerializeField] KeyIndex[] qwertyKeySeries;
    [SerializeField] KeyIndex[] controllerKeySeries;
    [SerializeField] KeyIndex[] keyboardRandomPool;
    [SerializeField] KeyIndex[] controllerRandomPool;
    [SerializeField] float timeToComplete;
    [SerializeField] float minTimeToComplete;

    public List<KeyIndex> GetKeySeries()
    {
        GameManager gameManager = FindObjectOfType<GameManager>();

        if (gameManager.GetKeyboardControl())
        {
            if (gameManager.GetAzertyControl())
            {
                return keySeries.ToList();
            }
            else
            {
                return qwertyKeySeries.ToList();
            }
        }
        else
        {
            return controllerKeySeries.ToList();
        }
    }

    public List<KeyIndex> GetReverseKeySeries()
    {
        KeyIndex[] reverseKeySeries;

        if (FindObjectOfType<GameManager>().GetKeyboardControl())
        {
            reverseKeySeries = new KeyIndex[keySeries.Length];

            for (int i = 0; i < keySeries.Length; i++)
            {
                reverseKeySeries[i] = keySeries[keySeries.Length - 1 - i];
            }
        }
        else
        {
            reverseKeySeries = new KeyIndex[controllerKeySeries.Length];

            for (int i = 0; i < controllerKeySeries.Length; i++)
            {
                reverseKeySeries[i] = controllerKeySeries[controllerKeySeries.Length - 1 - i];
            }
        }

        return reverseKeySeries.ToList();
    }

    public List<KeyIndex> GetRandomKeySeries()
    {
        if (FindObjectOfType<GameManager>().GetKeyboardControl())
        {
            int numberOfKeys = keySeries.Length - 1;
            int count = keyboardRandomPool.Length;
            int last = count - 1;

            for (int i = 0; i < last; ++i)
            {
                int index = UnityEngine.Random.Range(i, count);
                (keyboardRandomPool[index], keyboardRandomPool[i]) = (keyboardRandomPool[i], keyboardRandomPool[index]);
            }

            List<KeyIndex> newKeySeries = new();

            for (int i = 0; i < numberOfKeys; i++)
            {
                newKeySeries.Add(keyboardRandomPool[i]);
            }

            return newKeySeries;
        }
        else
        {
            int numberOfKeys = controllerKeySeries.Length - 1;
            int count = controllerRandomPool.Length;
            int last = count - 1;

            for (int i = 0; i < last; ++i)
            {
                int index = UnityEngine.Random.Range(i, count);
                (controllerRandomPool[index], controllerRandomPool[i]) = (controllerRandomPool[i], controllerRandomPool[index]);
            }

            List<KeyIndex> newKeySeries = new();

            for (int i = 0; i < numberOfKeys; i++)
            {
                newKeySeries.Add(controllerRandomPool[i]);
            }

            return newKeySeries;
        }
    }

    public float GetTimeToCompleteActivity()
    {
        return timeToComplete;
    }

    public float GetMinTimeToCompleteActivity()
    {
        return minTimeToComplete;
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Experience : MonoBehaviour, ISaveable
{
    [SerializeField] int currentExperience;
    int startGameExperience;

    private void Start()
    {
        startGameExperience = currentExperience;
    }

    public void GainExperience(int amount)
    {
        currentExperience += amount;
    }

    public int GetCurrentExperience()
    {
        return currentExperience;
    }

    public int GetExperienceGained()
    {
        return currentExperience - startGameExperience;
    }

    public object CaptureState()
    {
        return currentExperience;
    }

    public void RestoreState(object state)
    {
        currentExperience = (int)state;
    }
}

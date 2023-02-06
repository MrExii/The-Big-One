using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavingWrapper : MonoBehaviour
{
    SavingSystem savingSystem;

    const string saveFileName = "TheBigOne";

    private void Awake()
    {
        savingSystem = GetComponent<SavingSystem>();
    }

    public void Save()
    {
        savingSystem.Save(saveFileName);
    }

    public void Load()
    {
        savingSystem.Load(saveFileName);
    }

    public void LoadOneState(string uniqueIdentifier)
    {
        savingSystem.LoadOneState(saveFileName, uniqueIdentifier);
    }

    public void Delete()
    {
        savingSystem.Delete(saveFileName);
    }
}

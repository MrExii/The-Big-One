using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Temperatures Levels", menuName = "New Temperatures Levels", order = 0)]
public class SimulationSO : ScriptableObject
{
    [System.Serializable]
    private class Simulation
    {
        public SimulationIndex simulationIndex;
        public float[] simulationLvl;
    }

    [SerializeField] List<Simulation> simulations;

    public float GetSimulationLvl(SimulationIndex simulationIndex, int level)
    {
        foreach (Simulation simulation in simulations)
        {
            if (simulationIndex == simulation.simulationIndex)
            {
                return simulation.simulationLvl[level];
            }
        }

        return 0;
    }
}

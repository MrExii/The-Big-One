using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDependency : MonoBehaviour
{
    EnemyHealth enemyHealth;
    EnemyController enemyController;

    public EnemyHealth GetEnemyHealth()
    {
        if (!enemyHealth)
        {
            enemyHealth = GetComponent<EnemyHealth>();
        }

        return enemyHealth;
    }

    public EnemyController GetEnemyController()
    {
        if (!enemyController)
        {
            enemyController = GetComponent<EnemyController>();
        }

        return enemyController;
    }
}

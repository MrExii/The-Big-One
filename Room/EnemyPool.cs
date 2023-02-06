using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : MonoBehaviour
{
    [System.Serializable]
    private class KeyboardEnemies
    {
        public KeyCode keyboardKeycode;
        
        public List<GameObject> keyboardPool = new();
        

        public KeyboardEnemies(KeyCode keyboardKeycode, List<GameObject> keyboardPool)
        {
            this.keyboardKeycode = keyboardKeycode;
            this.keyboardPool = keyboardPool;
        }
    }

    [System.Serializable]
    private class ControllerEnemies
    {
        public KeyIndex controllerKeyCode;
        public List<GameObject> controllerPool = new();

        public ControllerEnemies(KeyIndex controllerKeyCode, List<GameObject> controllerPool)
        {
            this.controllerKeyCode = controllerKeyCode;
            this.controllerPool = controllerPool;
        }
    }

    [SerializeField] List<KeyboardEnemies> keyboardPool = new();
    [SerializeField] List<ControllerEnemies> controllerPool = new();

    readonly List<GameObject> baseList = new();

    GameManager gameManager;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    private void LateUpdate()
    {
        FindClosestEnemy();
    }

    private void FindClosestEnemy()
    {
        if (gameManager.GetKeyboardControl())
        {
            foreach (KeyboardEnemies item in keyboardPool)
            {
                float lastDistance = Mathf.Infinity;

                foreach (GameObject enemy in item.keyboardPool)
                {
                    if (!enemy) continue;

                    EnemyController enemyController = enemy.GetComponent<EnemyController>();

                    if (!enemyController.GetCanAttack()) continue;
                    if (enemyController.GetComponent<EnemyHealth>().GetIsDead()) continue;
                    if (!enemyController.GetEnableActivity()) continue;
                    if (enemyController.GetIsSleeping()) continue;

                    if (enemyController.GetDistance() < lastDistance && enemyController.gameObject.activeInHierarchy)
                    {
                        lastDistance = enemyController.GetDistance();
                    }
                }

                foreach (GameObject enemy in item.keyboardPool)
                {
                    EnemyController enemyController = enemy.GetComponent<EnemyController>();

                    if (enemyController.GetDistance() == lastDistance)
                    {
                        enemyController.GetComponent<Activity>().EnemyCanBeSelected(true);
                    }
                    else
                    {
                        enemyController.GetComponent<Activity>().EnemyCanBeSelected(false);
                    }
                }
            }
        }
        else
        {
            foreach (ControllerEnemies item in controllerPool)
            {
                float lastDistance = Mathf.Infinity;

                foreach (GameObject enemy in item.controllerPool)
                {
                    if (!enemy) continue;

                    EnemyController enemyController = enemy.GetComponent<EnemyController>();

                    if (!enemyController.GetCanAttack()) continue;
                    if (enemyController.GetComponent<EnemyHealth>().GetIsDead()) continue;
                    if (!enemyController.GetEnableActivity()) continue;
                    if (enemyController.GetIsSleeping()) continue;

                    if (enemyController.GetDistance() < lastDistance && enemyController.gameObject.activeInHierarchy)
                    {
                        lastDistance = enemyController.GetDistance();
                    }
                }

                foreach (GameObject enemy in item.controllerPool)
                {
                    EnemyController enemyController = enemy.GetComponent<EnemyController>();

                    if (enemyController.GetDistance() == lastDistance)
                    {
                        enemyController.GetComponent<Activity>().EnemyCanBeSelected(true);
                    }
                    else
                    {
                        enemyController.GetComponent<Activity>().EnemyCanBeSelected(false);
                    }
                }
            }
        }
    }

    private void RefreshEnemyPool()
    {
        keyboardPool.Clear();
        controllerPool.Clear();

        foreach (GameObject enemy in baseList)
        {
            if (!enemy.activeInHierarchy) continue;

            KeyCode keyboardKeyCode = enemy.GetComponent<Activity>().GetKeyboardKey();

            bool createNewList = true;

            if (keyboardPool.Capacity != 0)
            {
                foreach (KeyboardEnemies item in keyboardPool)
                {
                    if (item.keyboardKeycode == keyboardKeyCode)
                    {
                        if (!item.keyboardPool.Contains(enemy))
                        {
                            item.keyboardPool.Add(enemy);

                            createNewList = false;
                        }
                    }
                }
            }
            
            if (createNewList)
            {
                List<GameObject> enemyList = new();
                enemyList.Add(enemy);

                keyboardPool.Add(new KeyboardEnemies(keyboardKeyCode, enemyList));
            }
        }

        foreach (GameObject enemy in baseList)
        {
            if (!enemy.activeInHierarchy) continue;

            KeyIndex controllerKeyCode = enemy.GetComponent<Activity>().GetControllerKey();

            bool createNewList = true;

            if (controllerPool.Capacity != 0)
            {
                foreach (ControllerEnemies item in controllerPool)
                {
                    if (item.controllerKeyCode == controllerKeyCode)
                    {
                        if (!item.controllerPool.Contains(enemy))
                        {
                            item.controllerPool.Add(enemy);

                            createNewList = false;
                        }
                    }
                }
            }
            
            if (createNewList)
            {
                List<GameObject> enemyList = new();
                enemyList.Add(enemy);

                controllerPool.Add(new ControllerEnemies(controllerKeyCode, enemyList));
            }
        }
    }

    public void ClearList()
    {
        baseList.Clear();
    }

    public void AddEnemyToList(GameObject enemy)
    {
        baseList.Add(enemy);
        RefreshEnemyPool();
    }
}

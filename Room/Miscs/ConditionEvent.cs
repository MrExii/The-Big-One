using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionEvent : MonoBehaviour
{
    [SerializeField] GameObject conditionReward;
    [SerializeField] GameObject[] externalRewards;
    [SerializeField] GameObject rewardVFX;
    [SerializeField] GameObject lockdownVFX;
    [SerializeField] Collider2D lockdownCollider;
    [SerializeField] bool destroyingReward;
    [SerializeField] bool lockdown;
    [SerializeField] EnemyHealth[] enemies;
    [SerializeField] Activity[] activities;
    [SerializeField] AnimationClip detonatorClip;

    PlayerController playerController;

    int numberOfCondition;
    bool conditionCompleted;
    bool lockdownTrigger;

    private void Awake()
    {
        playerController = FindObjectOfType<PlayerController>();
    }

    private void Start()
    {
        if (!destroyingReward || lockdown)
        {
            conditionReward.SetActive(false);

            foreach (var item in externalRewards)
            {
                item.SetActive(false);
            }
        }

        numberOfCondition += enemies.Length;
        numberOfCondition += activities.Length;
    }

    private void Update()
    {
        if (conditionCompleted) return;

        CheckConditions();

        CheckCollision();
    }

    private void CheckCollision()
    {
        if (lockdownCollider.IsTouching(playerController.GetBodyCollider()) && !lockdownTrigger && lockdown)
        {
            lockdownTrigger = true;

            conditionReward.SetActive(true);

            foreach (var item in externalRewards)
            {
                item.SetActive(true);
            }

            foreach (var effect in lockdownVFX.GetComponentsInChildren<ParticleSystem>())
            {
                effect.Play();
            }
        }
    }

    private void CheckConditions()
    {
        int validateCondition = 0;

        foreach (EnemyHealth enemy in enemies)
        {
            if (enemy.GetIsDead())
            {
                validateCondition++;
            }
        }

        foreach (Activity activity in activities)
        {
            if (activity.GetIsActivityCompleted())
            {
                validateCondition++;
            }
        }
        
        if (validateCondition == numberOfCondition)
        {
            conditionCompleted = true;

            if (lockdown || destroyingReward)
            {
                Destroy(conditionReward);
            }
            else
            {
                conditionReward.SetActive(true);
                
                foreach (var item in externalRewards)
                {
                    item.SetActive(true);
                }
            }

            foreach (var effect in rewardVFX.GetComponentsInChildren<ParticleSystem>())
            {
                effect.Play();
            }
        }
    }
}

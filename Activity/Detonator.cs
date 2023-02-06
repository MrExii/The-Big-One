using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Detonator : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] Activity activity;

    bool isActivated;

    private void Update()
    {
        if (activity.GetIsActivityCompleted() && !isActivated)
        {
            isActivated = true;

            animator.SetTrigger("detonator");
        }
    }
}

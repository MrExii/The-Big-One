using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lava : MonoBehaviour
{
    [SerializeField] BoxCollider2D boxCollider;

    CatchReferences references;

    bool inFire;

    private void Awake()
    {
        references = FindObjectOfType<CatchReferences>();
    }

    private void Update()
    {
        if (inFire)
        {
            float damage = Random.Range(4, 8) * (1 + references.GetGameManager().GetRoomClearMultiplier(false));

            references.GetPlayerHealth().TakeDamage(damage, false, false, false, false, true);
        }
    }

    public bool IsInFire()
    {
        if (boxCollider.IsTouching(references.GetPlayerController().GetBodyCollider())) return true;
        else return false;
    }

    private void StopFire()
    {
        foreach (var lava in FindObjectsOfType<Lava>())
        {
            if (lava.IsInFire()) return;
        }

        foreach (var lava in FindObjectsOfType<Lava>())
        {
            lava.SetInFire();
        }

        references.GetPlayerController().LavaVFX(false);
    }

    public void SetInFire()
    {
        inFire = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            inFire = true;

            references.GetPlayerController().LavaVFX(true);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        foreach (var lava in FindObjectsOfType<Lava>())
        {
            if (lava.IsInFire()) return;
        }

        if (collision.gameObject.CompareTag("Player"))
        {
            Invoke(nameof(StopFire), 2f);
        }
    }
}

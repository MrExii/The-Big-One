using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : MonoBehaviour
{
    [SerializeField] Sprite[] rockSprites;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] ParticleSystem destroyVFX;

    CatchReferences references;

    private void Awake()
    {
        references = FindObjectOfType<CatchReferences>();
    }

    private void Start()
    {
        int spriteIndex = Random.Range(0, rockSprites.Length);

        spriteRenderer.sprite = rockSprites[spriteIndex];
    }

    private void Update()
    {
        transform.Rotate(new(0, 0, 90 * Time.deltaTime));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy")) return;
        if (collision.CompareTag("NonePlayer")) return;
        if (collision.CompareTag("Pickups")) return;
        if (collision.CompareTag("Platform")) return;

        if (collision.CompareTag("Player") && !references.GetPlayerController().isRolling)
        {
            float damage = Random.Range(4, 8) * (1 + references.GetGameManager().GetRoomClearMultiplier(false));

            references.GetPlayerHealth().TakeDamage(damage, false, false, false, true, true);

            Destroy(gameObject);
        }
        else if (collision.CompareTag("RockDestroyCollider"))
        {
            ParticleSystem vfx = Instantiate(destroyVFX, transform.position, Quaternion.identity);
            vfx.Play();
            vfx.gameObject.AddComponent<SelfDestruct>().SetTimeToWait(2f);

            Destroy(gameObject);
        }
    }
}

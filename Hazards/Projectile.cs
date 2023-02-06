using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] float speedTravel;
    [SerializeField] bool spiderShot;
    [SerializeField] GameObject spiderWeb;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] ParticleSystem canonBallVFX;

    GameObject instigator;
    CatchReferences references;

    Vector3 playerPosition;

    float hitDamage;
    bool isCritical;
    bool isVerticalShoot;

    private void Awake()
    {
        playerPosition = FindObjectOfType<PlayerController>().transform.position;
        references = FindObjectOfType<CatchReferences>();
    }

    private void Start()
    {
        if (!spiderShot)
        {
            MoveProjectile();
        }

        hitDamage *= references.GetSimulationsPlaceHolder().GetEnemyDamageMultiplier() + references.GetGameManager().GetRoomClearMultiplier(false);
    }

    private void Update()
    {
        if (spiderShot)
        {
            SpiderShot();
        }
    }

    private void MoveProjectile()
    {
        int direction;
        bool isFliped = instigator.GetComponent<SpriteRenderer>().flipX;

        if (isFliped)
        {
            if (isVerticalShoot)
            {
                direction = -1;
            }
            else
            {
                direction = 1;
            }

            spriteRenderer.flipX = true;
        }
        else
        {
            if (isVerticalShoot)
            {
                direction = 1;
            }
            else
            {
                direction = -1;
            }
        }

        if (isVerticalShoot)
        {
            rb.velocity = new Vector2(0, direction * speedTravel);
        }
        else
        {
            rb.velocity = new Vector2(direction * speedTravel, 0);
        }
    }

    private void SpiderShot()
    {
        transform.position = Vector2.MoveTowards(transform.position, playerPosition, speedTravel * Time.deltaTime);

        if (transform.position == playerPosition)
        {
            Instantiate(spiderWeb, transform.position, Quaternion.identity, references.GetRoomPool().GetCurrentRoom().transform);

            Destroy(gameObject);
        }
    }

    public void SetupProjectile(GameObject instigator, float hitDamage, bool isCritical, bool isVerticalShoot)
    {
        this.instigator = instigator;
        this.hitDamage = hitDamage;
        this.isCritical = isCritical;
        this.isVerticalShoot = isVerticalShoot;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Hazards")) return;
        if (collision.CompareTag("BleedingHazards")) return;
        if (collision.CompareTag("Enemy")) return;
        if (collision.CompareTag("NonePlayer")) return;
        if (collision.CompareTag("Pickups")) return;

        if (collision.CompareTag("Player"))
        {
            bool hitAnyway = false;

            if (collision.GetComponentInParent<PlayerController>().isRolling) return;

            if (spiderShot)
            {
                collision.GetComponentInParent<PlayerController>().SlowDown(true);

                hitAnyway = true;
            }

            collision.GetComponentInParent<PlayerHealth>().TakeDamage(hitDamage, isCritical, hitAnyway, false, true, true);

            Destroy(gameObject);
        }
        else
        {
            if (canonBallVFX)
            {
                ParticleSystem vfx = Instantiate(canonBallVFX, transform.position, Quaternion.identity, FindObjectOfType<Room>().transform);
                vfx.Play();
            }

            Destroy(gameObject);
        }
    }
}

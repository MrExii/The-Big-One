using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Shoot : MonoBehaviour
{
    [SerializeField] Projectile projectilePrefab;
    [SerializeField] Transform instantiatePosition;
    [SerializeField] float projectileDamage;
    [SerializeField] float timeBetweenShoot;
    [SerializeField] bool isEnemy;
    [SerializeField] bool verticalShoot;
    [SerializeField] bool shootWhenSpawn;
    [SerializeField] GameObject fireArrowVFX;
    [SerializeField] float startDelay;
    [SerializeField] AudioSource audioSource;

    Animator animator;
    ChargeBar chargerBar;
    GenerateDamage generateDamage;

    float instantiatePositionX;
    float timeSinceLastShoot;
    float timeSinceStart;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        chargerBar = GetComponentInChildren<ChargeBar>();
        generateDamage = GetComponent<GenerateDamage>();
    }

    private void Start()
    {
        if (shootWhenSpawn)
        {
            timeSinceLastShoot = Mathf.Infinity;
        }

        if (GetComponent<SpriteRenderer>().flipX && chargerBar)
        {
            chargerBar.FlipPosition();
        }

        instantiatePositionX = instantiatePosition.localPosition.x;

        if (chargerBar && startDelay > 0)
        {
            chargerBar.ShootChargeBar(timeSinceLastShoot, timeBetweenShoot);
        }
    }

    private void Update()
    {
        timeSinceStart += Time.deltaTime;

        if (timeSinceStart < startDelay) return;

        if (timeSinceLastShoot > timeBetweenShoot && !isEnemy)
        {
            animator.SetTrigger("fire");

            timeSinceLastShoot = 0f;
        }

        timeSinceLastShoot += Time.deltaTime;

        if (chargerBar)
        {
            chargerBar.ShootChargeBar(timeSinceLastShoot, timeBetweenShoot);
        }
    }

    //Call by fire animation
    public void RaycastShoot()
    {
        Vector2 direction;

        audioSource.Play();

        if (verticalShoot)
        {
            if (GetComponent<SpriteRenderer>().flipX)
            {
                direction = Vector2.down;
            }
            else
            {
                direction = Vector2.up;
            }
        }
        else
        {
            if (GetComponent<SpriteRenderer>().flipX)
            {
                instantiatePosition.localPosition = new Vector2(instantiatePositionX * -1, instantiatePosition.localPosition.y);

                direction = Vector2.right;
            }
            else
            {
                direction = Vector2.left;
            }
        }

        RaycastHit2D[] hits = Physics2D.RaycastAll(instantiatePosition.position, direction, Mathf.Infinity);

        if (hits.Length > 0)
        {
            foreach (RaycastHit2D ray in hits)
            {
                if (ray.collider.CompareTag("Platform")) break;

                if (ray.collider.CompareTag("Player"))
                {
                    float damage = generateDamage.GetRandomDamage(projectileDamage, false, false);
                    bool isCritical = generateDamage.GetIsCriticalDamage();

                    ray.collider.GetComponentInParent<PlayerHealth>().TakeDamage(damage, isCritical, false, true, true, true);
                }
            }
        }
    }

    //Call by fire animation
    public void LaunchProjectile()
    {
        if (GetComponent<SpriteRenderer>().flipX)
        {
            instantiatePosition.localPosition = new Vector2(instantiatePositionX * -1, instantiatePosition.localPosition.y);

            InstantiateFireArrowVFX(true);
        }
        else
        {
            InstantiateFireArrowVFX(false);
        }

        float damage = generateDamage.GetRandomDamage(projectileDamage, false, false);
        bool isCritical = generateDamage.GetIsCriticalDamage();

        Projectile projectile = Instantiate(projectilePrefab, instantiatePosition.position, Quaternion.identity);
        projectile.SetupProjectile(gameObject, damage, isCritical, verticalShoot);
    }

    private void InstantiateFireArrowVFX(bool flipX)
    {
        if (fireArrowVFX)
        {
            if (flipX)
            {
                Vector2 instancePosition = new(transform.position.x + 1.4f, transform.position.y + 0.2f);
                GameObject fireArrowInstance = Instantiate(fireArrowVFX, instancePosition, Quaternion.identity, transform);

                Vector3 localScale = fireArrowInstance.transform.localScale;
                fireArrowInstance.transform.localScale = new Vector2(localScale.x * -1, localScale.y);
            }
            else
            {
                Vector2 instancePosition = new(transform.position.x - 1.4f, transform.position.y + 0.2f);
                Instantiate(fireArrowVFX, instancePosition, Quaternion.identity, transform);
            }
        }
    }
}

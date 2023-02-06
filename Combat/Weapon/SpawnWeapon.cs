using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnWeapon : MonoBehaviour
{
    [SerializeField] GameObject[] weapons;
    [SerializeField] GameObject[] epicWeapons;

    private void Start()
    {
        int weapon = Random.Range(0, weapons.Length);
        int epicWeapon = Random.Range(0, 101);

        if (epicWeapon <= 5 || FindObjectOfType<GameManager>().GetNoobMode())
        {
            Instantiate(epicWeapons[weapon], transform.position, Quaternion.identity, transform).GetComponent<Weapon>().SetWeaponEpic();
        }
        else
        {
            Instantiate(weapons[weapon], transform.position, Quaternion.identity, transform);
        }
    }
}

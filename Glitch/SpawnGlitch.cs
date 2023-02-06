using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnGlitch : MonoBehaviour
{
    [SerializeField] GameObject glitch;
    [SerializeField] int minSpawn;
    [SerializeField] int maxSpawn;
    [SerializeField] int noGlitchPercentage = -1;

    public void Spawn()
    {
        int numberOfLoots = Random.Range(minSpawn, maxSpawn);

        if (noGlitchPercentage != -1)
        {
            int noSpawnPercentage = Random.Range(0, 101);

            if (noSpawnPercentage <= noGlitchPercentage)
            {
                numberOfLoots = 0;
            }
        }

        for (int i = 0; i < numberOfLoots; i++)
        {
            GameObject glitchSpawn = Instantiate(glitch, transform.position, Quaternion.identity);

            float forceRange = Random.Range(-70, -40);
            int direction = Random.Range(0, 2);
            
            if (direction == 0)
            {
                direction = -1;
            }
            else
            {
                direction = 1;
            }

            glitchSpawn.GetComponent<Rigidbody2D>().AddForce(new Vector2(forceRange * direction, 25));
        }
    }
}

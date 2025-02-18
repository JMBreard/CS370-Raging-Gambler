using UnityEngine;
using System.Collections;

public class ObsctacleSpawner : MonoBehaviour
{
     // Random position will be the position we want to place the object
    Vector2 randomPosition;

    // xRange the range in the x axis that the object can be placed
    public float xRange = 3f;

    // yRange the range in the y axis that the object can be placed
    public float yRange = 3f;

    public GameObject obstaclePrefab;

    public float spawnInterval = 1f;

    void Start()
    {
        StartCoroutine(SpawnObstacles());
    }

     IEnumerator SpawnObstacles()
    {
        for (int i = 0; i < 10; i++) {
            // xPosition and yPosition are set to random values with the ranges
            float xPosition = Random.Range(0 - xRange, 0 + xRange);
            float yPosition = Random.Range(0 - yRange, 0 + yRange);

            // randomPosition is then given values xPosition and yPosition, making it a random vector
            randomPosition = new Vector2(xPosition, yPosition);

             Instantiate(obstaclePrefab, randomPosition, Quaternion.identity);

            // Wait for the interval before spawning the next object
            yield return new WaitForSeconds(spawnInterval);
        }
    }

}

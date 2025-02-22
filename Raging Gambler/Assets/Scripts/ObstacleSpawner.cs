using UnityEngine;
using System.Collections;
using System.Threading;

public class ObsctacleSpawner : MonoBehaviour
{

    // xRange the range in the x axis that the object can be placed
    public float xRange = 12f;

    // yRange the range in the y axis that the object can be placed
    public float yRange = 12f;

    public GameObject obstaclePrefab;

    public float minDist = 6f;

    void Start()
    {
        SpawnObstacles();
    }

    void SpawnObstacles()
    {
        for (int i = 0; i < 3; i++) {
            
            // Creates vector location for object
            Vector2 randomPosition;

            float xPosition = Random.Range(-xRange, xRange);
            float yPosition = Random.Range(-yRange, yRange);
            randomPosition = new Vector2(xPosition, yPosition);

            // Creates obstacle in frame     
            Instantiate(obstaclePrefab, randomPosition, Quaternion.identity);

        }
    }

}

using UnityEngine;
using System.Collections;
using System.Threading;
using System.Globalization;

public class ObsctacleSpawner : MonoBehaviour
{

    // xRange the range in the x axis that the object can be placed
    public float xRange = 100f;

    // yRange the range in the y axis that the object can be placed
    public float yRange = 25f;

    private int num; 

    public GameObject obstaclePrefab;

    void Start()
    {
        SpawnObstacles();
    }

    void SpawnObstacles()
    {
        num = Random.Range(1, 8);

        for (int i = 0; i < num; i++) {
            
            // Creates vector location for object
            Vector2 randomPosition;

            float xPosition = Random.Range(-xRange, xRange);
            float yPosition = Random.Range(-yRange, yRange);
            randomPosition = new Vector2(xPosition, yPosition);

            // Creates obstacle in frame     
            Instantiate(obstaclePrefab, randomPosition, Quaternion.identity);

        }
    }

    public void NewRoomObstacles(Vector3 newPos)
    {
        num = Random.Range(1, 8);

        for (int i = 0; i < num; i++) {
            
            // Creates vector location for object
            Vector2 randomPosition = new Vector2(newPos.x, newPos.y);

            float xPosition = Random.Range( -xRange, xRange );
            float yPosition = Random.Range( -yRange, yRange );
            randomPosition.x += xPosition;
            randomPosition.y += yPosition;

            // Creates obstacle in frame     
            Instantiate(obstaclePrefab, randomPosition, Quaternion.identity);

        }
    }

}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Globalization;

public class ObsctacleSpawner : MonoBehaviour
{

    // xRange the range in the x axis that the object can be placed
    public float xRange;

    // yRange the range in the y axis that the object can be placed
    public float yRange;

    public int min;

    public int max;

    private int num;

    public GameObject obstaclePrefab;
    private List<GameObject> obstaclePool = new List<GameObject>();

    void Start()
    {
        // Create pool of obstacles
        for (int i = 0; i < max; i++)
        {
            GameObject obstacle = Instantiate(obstaclePrefab, new Vector3(0, 0, 0), Quaternion.identity);
            obstacle.SetActive(false);
            obstaclePool.Add(obstacle);
        }
        SpawnObstacles();
    }

    void SpawnObstacles()
    {
        // Deactivate all obstacles 
        foreach (GameObject obstacle in obstaclePool)
        {
            obstacle.SetActive(false);
        }

        num = Random.Range(min, max);

        for (int i = 0; i < num; i++)
        {

            if (i < obstaclePool.Count)
            {
                // Creates vector location for object
                Vector2 randomPosition;

                float xPosition = Random.Range(-xRange, xRange);
                float yPosition = Random.Range(-yRange, yRange);
                randomPosition = new Vector2(xPosition, yPosition - 2);

                obstaclePool[i].transform.position = randomPosition;
                obstaclePool[i].SetActive(true);
            }
        }
    }

    public void ResetObstaclesHealth()
    {
        foreach (GameObject obstacle in obstaclePool)
        {
            if (obstacle != null && obstacle.activeSelf)
            {
                HealthController healthController = obstacle.GetComponent<HealthController>();
                if (healthController != null)
                {
                    healthController.currentHealth = healthController.maxHealth;
                    healthController.isDead = false;
                }
            }
        }
    }

    public void NewRoomObstacles(Vector3 newPos)
    {
        // Deactivate all obstacles first
        foreach (GameObject obstacle in obstaclePool)
        {
            obstacle.SetActive(false);
        }

        for (int i = 0; i < num; i++)
        {
            if (i < obstaclePool.Count)
            {
                // Creates vector location for object
                Vector2 randomPosition = new Vector2(newPos.x, newPos.y);

                float xPosition = Random.Range(-xRange, xRange);
                float yPosition = Random.Range(-yRange, yRange);
                randomPosition.x += xPosition;
                randomPosition.y += yPosition - 2;

                obstaclePool[i].transform.position = randomPosition;
                obstaclePool[i].SetActive(true);
            }
        }
        ResetObstaclesHealth();
    }
}

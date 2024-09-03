using UnityEngine;

public class AsteroidSpawner : MonoBehaviour
{
    public GameObject asteroidPrefab;
    public float spawnRate = 5f;
    public float spawnDistance = 10f;
    public int maxAsteroids = 30;

    public int asteroidSize = 2;
    public Vector2 centerPoint = Vector2.zero; // Center of the screen or play area

    private float nextSpawnTime;

    private void Update()
    {
        if (Time.time >= nextSpawnTime && GameObject.FindGameObjectsWithTag("Asteroid").Length < maxAsteroids)
        {
            SpawnAsteroid();

            nextSpawnTime = Time.time + spawnRate;
        }
    }

    private void SpawnAsteroid()
    {
        // Generate a random point on the circle
        Vector2 spawnPosition = Random.insideUnitCircle.normalized * spawnDistance;

        // Calculate direction towards the center
        Vector2 directionToCenter = (centerPoint - spawnPosition).normalized;

        // Add some randomness to the direction
        float randomAngle = Random.Range(-30f, 30f); // Adjust this range for more or less variation
        Vector2 randomizedDirection = Quaternion.Euler(0, 0, randomAngle) * directionToCenter;

        // Instantiate the asteroid
        GameObject asteroidObject = Instantiate(asteroidPrefab, spawnPosition, Quaternion.identity);
        Asteroid asteroid = asteroidObject.GetComponent<Asteroid>();

        if (asteroid != null)
        {
            float randomSize = new float[] { 2f, 1f, 0.5f }[Random.Range(0, 3)];

            asteroid.Initialize(randomizedDirection, randomSize);
        }
        else
        {
            Debug.LogError("Asteroid script not found on the instantiated prefab!");
        }
    }

    public void newLevel(){
        GameObject[] asteroids = GameObject.FindGameObjectsWithTag("Asteroid");

        foreach (GameObject asteroid in asteroids)
        {
            Destroy(asteroid);
        }
    }

}
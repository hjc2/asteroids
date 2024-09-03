using UnityEngine;

public class Asteroid : MonoBehaviour
{

    public GameObject asteroidPrefab;

    public float minSpeed = 1f;
    public float maxSpeed = 3f;
    public float minLifetime = 10f;
    public float maxLifetime = 20f;

    public float size;

    private Vector2 movement;
    private bool hasBeenOnScreen = false;
    private Camera mainCamera;
    private float lifetime;

    private void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("Main camera not found!");
        }
        
        // Set a random lifetime for the asteroid
        lifetime = Random.Range(minLifetime, maxLifetime);
    }

    public void Initialize(Vector2 direction, float sizeA)
    {
        // Set random size
        size = sizeA;
        transform.localScale = Vector3.one * size;

        // Set movement based on the provided direction
        float speed = Random.Range(minSpeed, maxSpeed);
        movement = direction.normalized * speed;
    }

    private void Update()
    {
        // Decrease lifetime
        lifetime -= Time.deltaTime;


        // Move the asteroid
        transform.Translate(movement * Time.deltaTime);

        // Check if asteroid is on screen, accounting for its radius
        Vector3 viewportPosition = mainCamera.WorldToViewportPoint(transform.position);
        float radius = transform.localScale.x / 2; // Assuming the scale is uniform
        
        // Convert radius to viewport space
        Vector3 radiusInViewport = mainCamera.WorldToViewportPoint((Vector2)transform.position + Vector2.one * radius) - mainCamera.WorldToViewportPoint(transform.position);
        float viewportRadius = Mathf.Max(radiusInViewport.x, radiusInViewport.y);

        bool isOnScreen = viewportPosition.x + viewportRadius > 0 && viewportPosition.x - viewportRadius < 1 && 
                          viewportPosition.y + viewportRadius > 0 && viewportPosition.y - viewportRadius < 1 && 
                          viewportPosition.z > 0;

        if (isOnScreen)
        {
            hasBeenOnScreen = true;
        } else if (lifetime <= 0 )
        {
            Destroy(gameObject);
            return;

        }
        else if (hasBeenOnScreen)
        {
            // If it has been on screen before and now it's off, destroy it
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Rocket"))
        {

            if(size >= 1f){
                newAsteroid(size);
                newAsteroid(size);
            }

            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
    }

    public void newAsteroid(float size){
        Vector2 spawnPosition = transform.position;
        Vector2 directionToCenter = (Vector2.zero - spawnPosition).normalized;

        // Add some randomness to the direction
        float randomAngle = Random.Range(-60f, 60); // Adjust this range for more or less variation
        Vector2 randomizedDirection = Quaternion.Euler(0, 0, randomAngle) * directionToCenter;

        // Instantiate the asteroid
        GameObject asteroidObject = Instantiate(asteroidPrefab, spawnPosition, Quaternion.identity);

        Asteroid asteroid = asteroidObject.GetComponent<Asteroid>();

        if (asteroid != null)
        {
            asteroid.Initialize(randomizedDirection, size / 2);
        }
        else
        {
            Debug.LogError("Asteroid script not found on the instantiated prefab!");
        }
    }

}

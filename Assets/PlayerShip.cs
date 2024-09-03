using UnityEngine;
using System.Collections;

public class PlayerShip : MonoBehaviour
{
    public float rotationSpeed = 180f;
    public float thrustForce = 5f;
    public float maxSpeed = 5f;
    public float drag = 0.1f;
    public GameObject rocketPrefab; 
    public float fireRate = 0.25f;
    public float rocketOffset = 0.4f;
    public GameObject thrustFireIndicator;

    [Header("Thrust Fire Flashing")]
    public float flashOnDuration = 0.1f;
    public float flashOffDuration = 0.05f;

    private Vector2 velocity;
    private float nextFireTime = 0f;
    private Camera mainCamera;
    private float screenWidth;
    private float screenHeight;
    private Coroutine flashingCoroutine;

    void Start()
    {
        mainCamera = Camera.main;
        UpdateScreenBounds();
        
        if (thrustFireIndicator != null)
        {
            thrustFireIndicator.SetActive(false);
        }
    }

    void UpdateScreenBounds()
    {
        screenHeight = 2f * mainCamera.orthographicSize;
        screenWidth = screenHeight * mainCamera.aspect;
    }

    void Update()
    {
        HandleRotation();
        HandleThrust();
        HandleMovement();
        HandleShooting();
        WrapAroundScreen();
    }

    void HandleRotation()
    {
        float rotation = 0f;

        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            rotation = rotationSpeed;
        }
        else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            rotation = -rotationSpeed;
        }

        transform.Rotate(Vector3.forward * rotation * Time.deltaTime);
    }

    void HandleThrust()
    {
        bool isThrusting = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);

        if (isThrusting)
        {
            velocity += (Vector2)(transform.up * thrustForce * Time.deltaTime);
            velocity = Vector2.ClampMagnitude(velocity, maxSpeed);

            if (flashingCoroutine == null)
            {
                flashingCoroutine = StartCoroutine(FlashThrustFire());
            }
        }
        else
        {
            if (flashingCoroutine != null)
            {
                StopCoroutine(flashingCoroutine);
                flashingCoroutine = null;
                if (thrustFireIndicator != null)
                {
                    thrustFireIndicator.SetActive(false);
                }
            }
        }
    }

    IEnumerator FlashThrustFire()
    {
        while (true)
        {
            if (thrustFireIndicator != null)
            {
                thrustFireIndicator.SetActive(true);
                yield return new WaitForSeconds(flashOnDuration);
                thrustFireIndicator.SetActive(false);
                yield return new WaitForSeconds(flashOffDuration);
            }
            else
            {
                yield return null;
            }
        }
    }

    void HandleMovement()
    {
        transform.position += (Vector3)velocity * Time.deltaTime;
        velocity *= (1 - drag * Time.deltaTime);
    }

    void HandleShooting()
    {
        if ((Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0)) && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;
            Vector3 offset = new Vector3(0, rocketOffset, 0);
            Vector3 spawnPosition = transform.position + transform.TransformDirection(offset);
            Instantiate(rocketPrefab, spawnPosition, transform.rotation);
        }
    }

    void WrapAroundScreen()
    {
        Vector3 viewportPosition = mainCamera.WorldToViewportPoint(transform.position);
        Vector3 newPosition = transform.position;

        if (viewportPosition.x < 0)
            newPosition.x = mainCamera.ViewportToWorldPoint(new Vector3(1, viewportPosition.y, viewportPosition.z)).x;
        else if (viewportPosition.x > 1)
            newPosition.x = mainCamera.ViewportToWorldPoint(new Vector3(0, viewportPosition.y, viewportPosition.z)).x;

        if (viewportPosition.y < 0)
            newPosition.y = mainCamera.ViewportToWorldPoint(new Vector3(viewportPosition.x, 1, viewportPosition.z)).y;
        else if (viewportPosition.y > 1)
            newPosition.y = mainCamera.ViewportToWorldPoint(new Vector3(viewportPosition.x, 0, viewportPosition.z)).y;

        transform.position = newPosition;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Asteroid"))
        {
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
    }
}
using UnityEngine;

public class Rocket : MonoBehaviour
{
    public float speed = 10f;
    public float lifetime = 1.5f;
    public float size = 0.1f;

    void Start()
    {
        Destroy(gameObject, lifetime);
        transform.localScale = Vector3.one * size;
    }

    void Update()
    {
        transform.Translate(Vector3.up * speed * Time.deltaTime);
    }



}
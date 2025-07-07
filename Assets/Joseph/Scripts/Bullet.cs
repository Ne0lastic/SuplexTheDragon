using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;
    public float lifeTime = 3f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            BallScript ball = collision.gameObject.GetComponent<BallScript>();
            if (ball != null)
            {
                ball.OpenGate();
            }
        }

        Destroy(gameObject); // Destroy bullet either way
    }

}

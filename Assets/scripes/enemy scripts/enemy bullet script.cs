using UnityEngine;

public class enemybulletscript : MonoBehaviour
{
     public float speed = 12f;
    public float bulletLifeTime = 2f;
    public int bulletDamage = 1;
    public float bulletKnockback = 1f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
        bulletLifeTime -= Time.deltaTime;
        if (bulletLifeTime <= 0)
        {
            Destroy(gameObject);
        }
        

    }
    private void OnCollisionEnter(Collision other){
        if (other.gameObject.tag == "player")
        {
        
        }
        if (other.gameObject.tag == "Level")
        {
        Destroy(gameObject);
        }

    }
}

using UnityEngine;

public class EnemyMusicBombProjectile : MonoBehaviour
{
    [SerializeField]
    private float speed;
    [SerializeField]
    private GameObject explosion;
    [SerializeField]
    private LayerMask playerLayer;

    private bool moving = true;    

    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Invoke("StopMoving", 1);
    }

    // Update is called once per frame
    void Update()
    {
        if (moving)
        {
            transform.position += transform.forward * speed * Time.deltaTime;
        }
    }

    private void StopMoving()
    {
        moving = false;
        Invoke("Explosion", 1);
    }

    private void Explosion()
    {
        Instantiate(explosion, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
    private void OnTriggerEnter(Collider other)
    {
        if ((playerLayer.value & (1 << other.transform.gameObject.layer)) > 0)
        {            
            Explosion();
            other.GetComponentInParent<PlayerHealth>().Damage(15f);
        }
    }
}

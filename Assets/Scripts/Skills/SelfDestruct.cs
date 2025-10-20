using UnityEngine;

public class SelfDestruct : MonoBehaviour
{
    [SerializeField]
    private float selfDestructTime = 0.5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Invoke("Die", selfDestructTime);
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}

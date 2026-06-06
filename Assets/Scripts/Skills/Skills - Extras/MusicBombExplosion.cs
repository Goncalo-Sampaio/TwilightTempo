using UnityEngine;

public class MusicBombExplosion : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Invoke("SelfDestruct", 2);
    }

    private void SelfDestruct()
    {
        Destroy(gameObject);
    }
}

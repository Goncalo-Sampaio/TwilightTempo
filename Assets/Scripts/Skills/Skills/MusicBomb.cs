using UnityEngine;

public class MusicBomb : MonoBehaviour, ISkill
{
    [SerializeField]
    private GameObject musicBombProjectile;

    public void Cast()
    {
        Instantiate(musicBombProjectile, transform.position, Quaternion.identity);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

using UnityEngine;

public class MusicBomb : MonoBehaviour, ISkill
{
    [SerializeField]
    private GameObject musicBombProjectile;

    private GameObject playerModel;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerModel = FindAnyObjectByType<PlayerAnimEventsHandler>().gameObject;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Cast()
    {
        Instantiate(musicBombProjectile, transform.parent.transform.position, playerModel.transform.rotation);
    }
}

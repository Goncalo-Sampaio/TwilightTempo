using UnityEngine;

public class MusicBomb : MonoBehaviour, ISkill
{
    [SerializeField]
    private GameObject musicBombProjectile;
    [SerializeField]
    private float delay = 0.33f;

    [SerializeField]
    private Vector3 castPosition = Vector3.zero;

    private GameObject playerModel;

    private float bombCastTimer = 0f;
    private bool castBomb = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerModel = FindAnyObjectByType<PlayerAnimEventsHandler>().gameObject;
    }

    void FixedUpdate()
    {
        bombCastTimer -= Time.fixedDeltaTime;

        if (bombCastTimer <= 0)
        {
            bombCastTimer = 0;
            if (castBomb)
            {
                castBomb = false;

                transform.rotation = playerModel.transform.rotation;
                Instantiate(musicBombProjectile, transform.position + transform.TransformVector(castPosition), playerModel.transform.rotation);
            }
        }
    }

    public void Cast()
    {
        Debug.Log("Music Bomb");
        bombCastTimer = delay;
        castBomb = true;
    }
}

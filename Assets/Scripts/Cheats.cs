using UnityEngine;
using UnityEngine.SceneManagement;

public class Cheats : MonoBehaviour
{
    [SerializeField]
    private GameObject player;
    [SerializeField]
    private Transform cheatTeleport;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(0);
        }

        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            FindAnyObjectByType<PlayerHealth>().Heal();
        }

        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            FindAnyObjectByType<GaugeManager>().IncreaseGauge(100, SkillAttunement.None);
        }

        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            player.transform.position = cheatTeleport.transform.position;
        }
    }
}

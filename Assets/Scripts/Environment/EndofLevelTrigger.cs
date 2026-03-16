using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndofLevelTrigger : MonoBehaviour
{
    private bool m_IsActive = false;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && !m_IsActive)
        {
            StartCoroutine(EndOfLevelSequence());
        }
    }
    private IEnumerator EndOfLevelSequence()
    {
        m_IsActive = true;
        yield return null;
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene(0);
    }
}

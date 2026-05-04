using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InvokeThenPlay : MonoBehaviour
{
    [SerializeField] AudioSource buttonSound;
    
    public void PlayWithDelay()
    {
        if (buttonSound != null)
        {
            buttonSound.Play();
        }
    }
    public void PlayWithDelayAndChangeScene()
    {
        if (buttonSound != null)
        {
            buttonSound.Play();
        }
        StartCoroutine(ChangeToRot());
    }
    private IEnumerator ChangeToRot()
    {
        yield return null;
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
   
}

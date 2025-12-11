using UnityEngine;
using System.Collections;

public class SimpleWaveWarning : MonoBehaviour
{
    public GameObject warningUI;   
    public AudioSource audioSource;
    public AudioClip warningClip;
    public float displayTime = 3f;

    private void Awake()
    {
        if (warningUI != null)
            warningUI.SetActive(false);
    }

    public void ShowWarning()
    {
        StartCoroutine(WarningRoutine());
    }

    private IEnumerator WarningRoutine()
    {
        warningUI.SetActive(true);

        if (audioSource != null && warningClip != null)
            audioSource.PlayOneShot(warningClip);

        yield return new WaitForSeconds(displayTime);

        warningUI.SetActive(false);
    }
}

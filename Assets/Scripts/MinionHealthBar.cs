using UnityEngine;
using UnityEngine.UI;

public class MinionHealthBar : MonoBehaviour
{
    public MinionStats stats;      // The minion this bar tracks
    public Slider slider;          // The UI slider
    public float smoothSpeed = 10f;

    private Camera cam;

    void Start()
    {
        cam = Camera.main;

        if (slider != null)
        {
            slider.maxValue = 1f;
            slider.value = 1f;
        }
    }

    void LateUpdate()
    {
        if (stats == null || slider == null) return;

        // Always face the camera
        transform.LookAt(transform.position + cam.transform.forward);

        // Update health percentage
        float targetValue = Mathf.Clamp01(stats.currentHealth / stats.maxHealth);
        slider.value = Mathf.Lerp(slider.value, targetValue, Time.deltaTime * smoothSpeed);
    }

    // Immediately update the UI (useful on damage events)
    public void ForceUpdateUI()
    {
        if (stats == null || slider == null) return;

        slider.value = Mathf.Clamp01(stats.currentHealth / stats.maxHealth);
    }
}

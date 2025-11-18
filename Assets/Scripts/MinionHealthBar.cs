using UnityEngine;
using UnityEngine.UI;

public class MinionHealthBar : MonoBehaviour
{
    [Header("References")]
    public MinionStats stats;           // Assigned when spawned
    public Slider slider;               // The UI slider with Fill assigned

    [Header("Settings")]
    public float smoothSpeed = 10f;

    private Camera cam;

    void Awake()
    {
        cam = Camera.main;
    }

    void Start()
    {
        if (slider != null)
        {
            slider.maxValue = 1f;
            slider.value = 1f;
        }
    }

    void LateUpdate()
    {
        if (stats == null || slider == null)
            return;

        // Always face camera
        transform.LookAt(transform.position + cam.transform.forward);

        float targetValue = Mathf.Clamp01(stats.currentHealth / stats.maxHealth);
        slider.value = Mathf.Lerp(slider.value, targetValue, Time.deltaTime * smoothSpeed);
    }

    public void ForceUpdateUI()
    {
        if (stats == null || slider == null) return;
        slider.value = Mathf.Clamp01(stats.currentHealth / stats.maxHealth);
    }
}

using UnityEngine;
using UnityEngine.UI;

public class WorldSpaceHealthBar : MonoBehaviour
{
    [Header("References")]
    public MonoBehaviour healthSource;   // Must implement IHealth
    private IHealth health;

    public Slider slider;

    [Header("Settings")]
    public float smoothSpeed = 10f;

    private Camera cam;

    void Awake()
    {
        cam = Camera.main;
        health = healthSource as IHealth;

        if (health == null)
        {
            Debug.LogError($"{name}: Health source does not implement IHealth");
            enabled = false;
        }
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
        if (health == null || slider == null)
            return;

        // Face camera
        transform.LookAt(transform.position + cam.transform.forward);

        float target = Mathf.Clamp01(health.CurrentHealth / health.MaxHealth);
        slider.value = Mathf.Lerp(slider.value, target, Time.deltaTime * smoothSpeed);
    }

    public void ForceUpdateUI()
    {
        if (health == null || slider == null) return;
        slider.value = Mathf.Clamp01(health.CurrentHealth / health.MaxHealth);
    }
}

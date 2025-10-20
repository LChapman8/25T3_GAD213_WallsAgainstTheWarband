using UnityEngine;
using TMPro;
using System.Collections;

public class TowerTooltipUI : MonoBehaviour
{
    public static TowerTooltipUI Instance;

    [Header("UI References")]
    public GameObject tooltipObject;
    public TextMeshProUGUI towerNameText;
    public TextMeshProUGUI towerStatsText;
    public TextMeshProUGUI towerBioText;

    [Header("Tooltip Positioning")]
    public Vector2 offset = new Vector2(0f, 60f);
    public float hideDelay = 0.15f;

    private RectTransform tooltipRect;
    private bool followMouse = false;
    private Coroutine hideRoutine;

    private void Awake()
    {
        Instance = this;
        tooltipRect = tooltipObject.GetComponent<RectTransform>();
        tooltipObject.SetActive(false);
    }

    private void Update()
    {
        if (followMouse && tooltipObject.activeSelf)
        {
            Vector3 mousePos = Input.mousePosition;
            tooltipRect.position = mousePos + (Vector3)offset;
        }
    }

    public void ShowTooltip(string towerName, string stats, string bio)
    {
        if (hideRoutine != null)
        {
            StopCoroutine(hideRoutine);
            hideRoutine = null;
        }

        towerNameText.text = towerName;
        towerStatsText.text = stats;
        towerBioText.text = bio;

        tooltipObject.SetActive(true);
        followMouse = true;
        tooltipRect.position = Input.mousePosition + (Vector3)offset;
    }

    public void HideTooltip()
    {
        if (hideRoutine != null)
            StopCoroutine(hideRoutine);

        hideRoutine = StartCoroutine(HideAfterDelay());
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(hideDelay);
        tooltipObject.SetActive(false);
        followMouse = false;
        hideRoutine = null;
    }
}

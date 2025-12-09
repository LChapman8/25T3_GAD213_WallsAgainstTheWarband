using UnityEngine;
using TMPro;

public class WaveCounterUI : MonoBehaviour
{
    public WaveManager waveManager;
    public EndlessModeManager endlessManager;
    public TextMeshProUGUI waveText;

    private void OnEnable()
    {
        if (waveManager != null)
            waveManager.OnWaveStarted += UpdateWaveUI;
    }

    private void OnDisable()
    {
        if (waveManager != null)
            waveManager.OnWaveStarted -= UpdateWaveUI;
    }

    private void UpdateWaveUI(int waveNumber)
    {
        if (waveText == null) return;

        if (endlessManager != null && endlessManager.endlessActive)
            waveText.text = "Wave: Unlimited";
        else if (waveNumber < 0)
            waveText.text = "Wave: Boss Round";
        else
            waveText.text = $"Wave: {waveNumber}/{waveManager.totalWaves}";
    }

    public void SetUnlimitedMode()
    {
        if (waveText != null)
            waveText.text = "Wave: Unlimited";
    }

}

using UnityEngine;
using TMPro;

public class EnemiesRemainingUI : MonoBehaviour
{
    public EnemySpawner spawner;
    public EndlessModeManager endlessManager;
    public TextMeshProUGUI enemiesText;

    private void Update()
    {
        if (enemiesText == null) return;

        if (endlessManager != null && endlessManager.endlessActive)
            enemiesText.text = "Enemies: Unlimited";
        else if (spawner != null)
            enemiesText.text = $"Enemies: {spawner.enemiesAlive}";
    }

    public void SetUnlimitedMode()
    {
        if (enemiesText != null)
            enemiesText.text = "Enemies: Unlimited";
    }

}

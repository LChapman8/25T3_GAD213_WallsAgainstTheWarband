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

        // Check endless mode first
        if (endlessManager != null && endlessManager.endlessActive)
        {
            enemiesText.text = "Enemies: Unlimited";
            return;
        }

        if (spawner != null)
            enemiesText.text = $"Enemies: {spawner.enemiesAlive}";
    }


    public void SetUnlimitedMode()
    {
        if (enemiesText != null)
            enemiesText.text = "Enemies: Unlimited";
    }

}

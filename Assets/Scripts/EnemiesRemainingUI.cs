using UnityEngine;
using TMPro;

public class EnemiesRemainingUI : MonoBehaviour
{
    public EnemySpawner spawner;         // Drag your EnemySpawner here
    public TextMeshProUGUI enemiesText;  // Drag your TMP text here

    private void Update()
    {
        if (spawner != null && enemiesText != null)
        {
            enemiesText.text = $"Enemies: {spawner.enemiesAlive}";
        }
    }
}

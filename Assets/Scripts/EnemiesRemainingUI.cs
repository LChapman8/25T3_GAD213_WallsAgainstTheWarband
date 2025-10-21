using UnityEngine;
using TMPro;

public class EnemiesRemainingUI : MonoBehaviour
{
    public EnemySpawner spawner;         
    public TextMeshProUGUI enemiesText;  

    private void Update()
    {
        if (spawner != null && enemiesText != null)
        {
            enemiesText.text = $"Enemies: {spawner.enemiesAlive}";
        }
    }
}

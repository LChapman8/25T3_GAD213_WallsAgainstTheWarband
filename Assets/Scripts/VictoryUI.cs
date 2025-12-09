using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class VictoryUI : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject panel;
    public Button mainMenuButton;
    public Button exitButton;
    public Button endlessModeButton;

    [Header("Endless Mode Settings")]
    public EndlessModeManager endlessModeManager;

    private void Awake()
    {
        if (panel != null)
            panel.SetActive(false);

        mainMenuButton.onClick.AddListener(ReturnToMainMenu);
        exitButton.onClick.AddListener(ExitGame);
        endlessModeButton.onClick.AddListener(StartEndlessMode);
    }

    private void OnEnable()
    {
        BossStats.OnBossDefeated += ShowVictory;
    }

    private void OnDisable()
    {
        BossStats.OnBossDefeated -= ShowVictory;
    }

    public void ShowVictory()
    {
        if (panel != null)
            panel.SetActive(true);
    }

    void ReturnToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    void ExitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    void StartEndlessMode()
    {
        panel.SetActive(false);
        if (endlessModeManager != null)
            endlessModeManager.StartEndless();
    }
}

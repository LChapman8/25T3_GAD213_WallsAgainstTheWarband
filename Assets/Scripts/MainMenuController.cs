using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public string gameplaySceneName = "GameScene";

    public void PlayGame()
    {
        SceneManager.LoadScene(gameplaySceneName);
    }

    public void ExitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Stops play mode in editor
        #else
        Application.Quit(); // Closes build
        #endif
    }
}

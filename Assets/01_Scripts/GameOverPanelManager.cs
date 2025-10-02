using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverPanelManager : MonoBehaviour
{
    [Header("Asignaciones")]
    [SerializeField] private GameObject gameOverPanel;     
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private bool shown = false;

    private void Awake()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    // Llama esto cuando detectes que termin� el juego
    public void ShowGameOver()
    {
        if (shown) return;
        shown = true;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
    }

    // --- Botones ---
    // Reinicia la escena actual (empezar de cero)
    public void RestartGame()
    {
        var current = SceneManager.GetActiveScene();
        SceneManager.LoadScene(current.name);
    }

    // Ir al men� principal (otra escena)
    public void GoToMenu()
    {
        if (!string.IsNullOrEmpty(mainMenuSceneName))
            SceneManager.LoadScene(mainMenuSceneName);
        else
            Debug.LogError("[GameOverPanelManager] Asigna el nombre de la escena de men� en 'mainMenuSceneName'.");
    }
}

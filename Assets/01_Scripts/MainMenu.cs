using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private string gameSceneName = "SampleScene";

    public void StartGame()
    {
        // Con transición (ver Sección 3). Si no tienes el fader configurado aún, usa la línea directa:
        //SceneManager.LoadScene(gameSceneName);
        SceneTransition.LoadSceneWithFade(gameSceneName);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // para que funcione en el Editor
#else
        Application.Quit();
#endif
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    public static SceneTransition Instance { get; private set; }

    [Header("Fade")]
    [SerializeField] private CanvasGroup fadeCanvasGroup; // arrastra aquí el FadeImage
    [SerializeField] private float fadeDuration = 0.5f;   // velocidad del fade

    private void Awake()
    {
        // Singleton simple
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Asegura referencia si no está asignada
        if (fadeCanvasGroup == null)
            fadeCanvasGroup = GetComponentInChildren<CanvasGroup>(true);
    }

    private void Start()
    {
        // Al entrar a la primera escena, hacemos fade-in desde negro
        StartCoroutine(Fade(1f, 0f, fadeDuration));
    }

    public static void LoadSceneWithFade(string sceneName)
    {
        if (Instance == null)
        {
            Debug.LogWarning("[SceneTransition] No hay instancia. Cargando directa.");
            SceneManager.LoadScene(sceneName);
            return;
        }
        Instance.StartCoroutine(Instance.FadeAndSwitchScenes(sceneName));
    }

    private IEnumerator FadeAndSwitchScenes(string sceneName)
    {
        // Fade a negro
        yield return Fade(0f, 1f, fadeDuration);

        // Carga asíncrona (no bloquea la UI)
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = false;

        // Puedes mostrar tips o una barra de progreso si quieres.
        while (op.progress < 0.9f)
            yield return null;

        // Activamos la escena
        op.allowSceneActivation = true;
        yield return null; // espera un frame a que cargue

        // Fade desde negro a juego
        yield return Fade(1f, 0f, fadeDuration);
    }

    private IEnumerator Fade(float from, float to, float duration)
    {
        if (fadeCanvasGroup == null) yield break;

        float t = 0f;
        fadeCanvasGroup.alpha = from;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime; // independiente de Time.timeScale
            fadeCanvasGroup.alpha = Mathf.Lerp(from, to, t / duration);
            yield return null;
        }
        fadeCanvasGroup.alpha = to;
    }
}
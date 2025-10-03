using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ScoreManagment : MonoBehaviour
{
    public static ScoreManagment Instance;

    [Header("Puntajes")]
    public int p1Score = 0;
    public int p2Score = 0;
    public int winScore = 5;

    [Header("UI")]
    public Text p1ScoreText;
    public Text p2ScoreText;
    public GameObject gameOverPanel;
    public Text gameOverText;

    [Header("Bola")]
    public Rigidbody2D ballRb;
    public Transform ballTransform;
    public float ballStartSpeed = 8f;
    public float delayRelanzar = 1.0f;

    [Header("Audio Victoria")]
    [Tooltip("Clip genérico para victoria (se usa si no hay clip específico).")]
    public AudioClip winClip;
    [Tooltip("Clip cuando gana el Jugador 1 (opcional).")]
    public AudioClip p1WinClip;
    [Tooltip("Clip cuando gana el Jugador 2 (opcional).")]
    public AudioClip p2WinClip;
    [Range(0f, 1f)] public float winVolume = 1f;

    [SerializeField] private string mainMenuSceneName = "MainMenu";

    // --- interno ---
    AudioSource audioSrc;
    private int _nextLaunchDir = 1;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        // AudioSource interno para SFX (2D)
        audioSrc = gameObject.AddComponent<AudioSource>();
        audioSrc.playOnAwake = false;
        audioSrc.loop = false;
        audioSrc.spatialBlend = 0f;
        audioSrc.dopplerLevel = 0f;
        audioSrc.priority = 0;
    }

    void Start()
    {
        gameOverPanel?.SetActive(false);
        ActualizarUI();

        // Warm-up (evita delay la primera vez)
        if (winClip) audioSrc.PlayOneShot(winClip, 0f);
        if (p1WinClip) audioSrc.PlayOneShot(p1WinClip, 0f);
        if (p2WinClip) audioSrc.PlayOneShot(p2WinClip, 0f);
    }

    public void AddPointToPlayer(int playerIndex)
    {
        if (IsGameOver()) return;

        if (playerIndex == 1) p1Score++;
        else if (playerIndex == 2) p2Score++;

        ActualizarUI();

        if (CheckWin()) return;

        int dir = (playerIndex == 1) ? +1 : -1;
        CancelInvoke(nameof(RelanzarBola));
        ResetBall();
        Invoke(nameof(RelanzarBola), delayRelanzar);
        _nextLaunchDir = dir;
    }

    public bool IsGameOver()
    {
        return gameOverPanel != null && gameOverPanel.activeSelf;
    }

    bool CheckWin()
    {
        if (p1Score >= winScore)
        {
            MostrarGameOver("¡Ganó el Jugador 1!", winner: 1);
            return true;
        }
        if (p2Score >= winScore)
        {
            MostrarGameOver("¡Ganó el Jugador 2!", winner: 2);
            return true;
        }
        return false;
    }

    void MostrarGameOver(string mensaje, int winner)
    {
        if (gameOverText) gameOverText.text = mensaje;
        if (gameOverPanel) gameOverPanel.SetActive(true);

        // Detener bola
        if (ballRb)
        {
            ballRb.velocity = Vector2.zero;
            ballRb.angularVelocity = 0f;
        }

        // --- Sonido de victoria ---
        AudioClip clip = null;
        if (winner == 1 && p1WinClip) clip = p1WinClip;
        else if (winner == 2 && p2WinClip) clip = p2WinClip;
        else clip = winClip;

        if (clip) audioSrc.PlayOneShot(clip, winVolume);
    }

    public void RestartGame()
    {
        var current = SceneManager.GetActiveScene();
        SceneManager.LoadScene(current.name);
    }

    public void GoToMenu()
    {
        if (!string.IsNullOrEmpty(mainMenuSceneName))
            SceneManager.LoadScene(mainMenuSceneName);
        else
            Debug.LogError("[GameOverPanelManager] Asigna el nombre de la escena de menú en 'mainMenuSceneName'.");
    }

    void ActualizarUI()
    {
        if (p1ScoreText) p1ScoreText.text = p1Score.ToString();
        if (p2ScoreText) p2ScoreText.text = p2Score.ToString();
    }

    public void ResetBall()
    {
        if (!ballTransform || !ballRb) return;
        ballRb.velocity = Vector2.zero;
        ballRb.angularVelocity = 0f;
        ballTransform.position = Vector3.zero;
    }

    void RelanzarBola()
    {
        if (!ballRb) return;
        float y = Random.Range(-0.4f, 0.4f);
        Vector2 dir = new Vector2(Mathf.Sign(_nextLaunchDir), y).normalized;
        ballRb.velocity = dir * ballStartSpeed;
    }

    public void RestartMatch()
    {
        p1Score = 0;
        p2Score = 0;
        ActualizarUI();
        if (gameOverPanel) gameOverPanel.SetActive(false);
        ResetBall();
        _nextLaunchDir = Random.value < 0.5f ? -1 : 1;
        Invoke(nameof(RelanzarBola), 0.6f);
    }

    public void LoadMenu(string menuSceneName)
    {
        SceneManager.LoadScene(menuSceneName);
    }
}

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ScoreManagment : MonoBehaviour
{
    public static ScoreManagment Instance;

    [Header("Puntajes")]
    public int p1Score = 0;
    public int p2Score = 0;
    public int winScore = 5; // cambia a 10 si lo prefieres

    [Header("UI")]
    public Text p1ScoreText;   // Texto del panel del score P1
    public Text p2ScoreText;   // Texto del panel del score P2
    public GameObject gameOverPanel; // Panel de Game Over (desactivado al inicio)
    public Text gameOverText;  // Texto dentro del panel para mostrar "Gana P1/P2"

    [Header("Bola")]
    public Rigidbody2D ballRb;       // arrastra aquí el Rigidbody2D de la bola
    public Transform ballTransform;  // arrastra aquí el Transform de la bola
    public float ballStartSpeed = 8f;
    public float delayRelanzar = 1.0f; // tiempo antes de relanzar tras un punto

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        gameOverPanel?.SetActive(false);
        ActualizarUI();
    }

    public void AddPointToPlayer(int playerIndex)
    {
        if (IsGameOver()) return;

        if (playerIndex == 1) p1Score++;
        else if (playerIndex == 2) p2Score++;

        ActualizarUI();

        if (CheckWin()) return;

        // Reiniciar y relanzar la bola hacia quien recibió el gol (al que le METIERON el punto).
        // Si P1 anotó (playerIndex=1), relanzamos hacia P2 (derecha). Si P2 anotó, relanzamos hacia P1 (izquierda).
        int dir = (playerIndex == 1) ? +1 : -1;
        CancelInvoke(nameof(RelanzarBola));
        ResetBall();
        Invoke(nameof(RelanzarBola), delayRelanzar);
        // guardamos la dirección para el próximo relanzamiento
        _nextLaunchDir = dir;
    }

    private int _nextLaunchDir = 1;

    public bool IsGameOver()
    {
        return gameOverPanel != null && gameOverPanel.activeSelf;
    }

    bool CheckWin()
    {
        if (p1Score >= winScore)
        {
            MostrarGameOver("¡Ganó el Jugador 1!");
            return true;
        }
        if (p2Score >= winScore)
        {
            MostrarGameOver("¡Ganó el Jugador 2!");
            return true;
        }
        return false;
    }

    void MostrarGameOver(string mensaje)
    {
        if (gameOverText) gameOverText.text = mensaje;
        if (gameOverPanel) gameOverPanel.SetActive(true);

        // Detenemos la bola
        if (ballRb)
        {
            ballRb.velocity = Vector2.zero;
            ballRb.angularVelocity = 0f;
        }
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
        // Lanza con una pequeña variación vertical para que no sea 100% recto
        float y = Random.Range(-0.4f, 0.4f);
        Vector2 dir = new Vector2(Mathf.Sign(_nextLaunchDir), y).normalized;
        ballRb.velocity = dir * ballStartSpeed;
    }

    // Botón opcional para reiniciar toda la partida
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

    // Botón opcional para volver al menú (si tienes una escena de menú)
    public void LoadMenu(string menuSceneName)
    {
        SceneManager.LoadScene(menuSceneName);
    }
}

 zusing UnityEngine;
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


        int dir = (playerIndex == 1) ? +1 : -1;
        CancelInvoke(nameof(RelanzarBola));
        ResetBall();
        Invoke(nameof(RelanzarBola), delayRelanzar);
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

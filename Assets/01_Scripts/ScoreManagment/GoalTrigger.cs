using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class GoalTrigger : MonoBehaviour
{
    [Tooltip("1 si este wallPoint suma punto al Jugador 1; 2 si suma al Jugador 2")]
    public int pointToPlayer = 1;

    [Tooltip("Tag que debe tener la bola")]
    public string ballTag = "ball";

    [Header("Audio")]
    [Tooltip("Clip de sonido que se reproducirá cuando se marque gol.")]
    public AudioClip goalClip;
    [Range(0f, 1f)] public float volume = 1f;

    AudioSource audioSrc;

    private void Reset()
    {
        var col = GetComponent<Collider2D>();
        if (col) col.isTrigger = true;
    }

    private void Awake()
    {
        // Crear un AudioSource interno y configurarlo para SFX
        audioSrc = gameObject.AddComponent<AudioSource>();
        audioSrc.playOnAwake = false;
        audioSrc.loop = false;
        audioSrc.spatialBlend = 0f; // 2D
        audioSrc.dopplerLevel = 0f;
        audioSrc.priority = 0;      // máxima prioridad
    }

    private void Start()
    {
        // Warm-up para evitar delay en el primer uso
        if (goalClip != null)
            audioSrc.PlayOneShot(goalClip, 0f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(ballTag)) return;

        // Sonido de gol
        if (goalClip != null)
            audioSrc.PlayOneShot(goalClip, volume);

        // Sumar punto
        if (ScoreManagment.Instance != null)
            ScoreManagment.Instance.AddPointToPlayer(pointToPlayer);
    }
}

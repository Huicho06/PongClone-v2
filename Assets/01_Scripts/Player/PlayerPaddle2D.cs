using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class PlayerPaddle2D : MonoBehaviour
{
    public enum ControlType { Player1, Player2 }

    [Header("Control")]
    public ControlType control = ControlType.Player1;
    public float moveSpeed = 8f;

    [Header("Colores")]
    public SpriteRenderer spriteRenderer;

    [Header("Audio")]
    [Tooltip("Clip que se reproducirá al golpear la bola.")]
    public AudioClip hitBallClip;
    [Range(0f, 1f)] public float volume = 1f;
    [Tooltip("Evita spam si hay múltiples contactos muy seguidos.")]
    public float hitCooldown = 0.015f;

    [Header("Límites (opcional si no usas paredes)")]
    public bool clampY = false;
    public float minY = -4f;
    public float maxY = 4f;

    Rigidbody2D rb;
    float inputY;
    bool blockedUp, blockedDown;
    float lastHitTime = -999f;

    AudioSource audioSrc;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // Bloqueos físicos recomendados para paleta
        rb.freezeRotation = true;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionX;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        // AudioSource interno
        audioSrc = gameObject.AddComponent<AudioSource>();
        audioSrc.playOnAwake = false;
        audioSrc.loop = false;
        audioSrc.spatialBlend = 0f; // 2D
        audioSrc.dopplerLevel = 0f;
        audioSrc.priority = 0;      // máxima prioridad
    }

    void Start()
    {
        // Warm-up para evitar delay la primera vez
        if (hitBallClip != null)
            audioSrc.PlayOneShot(hitBallClip, 0f);
    }

    void Update()
    {
        inputY = (control == ControlType.Player1)
            ? (Input.GetKey(KeyCode.W) ? 1f : 0f) + (Input.GetKey(KeyCode.S) ? -1f : 0f)
            : (Input.GetKey(KeyCode.UpArrow) ? 1f : 0f) + (Input.GetKey(KeyCode.DownArrow) ? -1f : 0f);

        if (inputY > 0 && blockedUp) inputY = 0f;
        if (inputY < 0 && blockedDown) inputY = 0f;
    }

    void FixedUpdate()
    {
        rb.velocity = new Vector2(0f, inputY * moveSpeed);

        if (clampY)
        {
            float clampedY = Mathf.Clamp(rb.position.y, minY, maxY);
            if (Mathf.Abs(clampedY - rb.position.y) > Mathf.Epsilon)
                rb.position = new Vector2(rb.position.x, clampedY);
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.collider.CompareTag("ball"))
        {
            HitBall();
        }
        else if (col.collider.CompareTag("wall"))
        {
            var n = col.contacts[0].normal;
            if (n.y < 0) blockedUp = true;
            if (n.y > 0) blockedDown = true;
        }
    }

    void OnCollisionExit2D(Collision2D col)
    {
        if (col.collider.CompareTag("wall"))
        {
            blockedUp = false;
            blockedDown = false;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("ball")) HitBall();
    }

    void HitBall()
    {
        SetColor(Random.ColorHSV());

        if (hitBallClip == null) return;
        if (Time.time - lastHitTime < hitCooldown) return;

        audioSrc.PlayOneShot(hitBallClip, volume);
        lastHitTime = Time.time;
    }

    void SetColor(Color c)
    {
        if (spriteRenderer != null) spriteRenderer.color = c;
    }
}

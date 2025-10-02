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
    public SpriteRenderer spriteRenderer; // asigna el SpriteRenderer del Player

    Rigidbody2D rb;
    float inputY;

    // Bloqueo de movimiento por pared
    bool blockedUp = false;
    bool blockedDown = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    void Update()
    {
        // INPUT LOCAL:
        if (control == ControlType.Player1)
        {
            inputY = (Input.GetKey(KeyCode.W) ? 1f : 0f) +
                     (Input.GetKey(KeyCode.S) ? -1f : 0f);
        }
        else // Player2
        {
            inputY = (Input.GetKey(KeyCode.UpArrow) ? 1f : 0f) +
                     (Input.GetKey(KeyCode.DownArrow) ? -1f : 0f);
        }

        // bloquear si choca con pared
        if (inputY > 0 && blockedUp) inputY = 0f;
        if (inputY < 0 && blockedDown) inputY = 0f;
    }

    void FixedUpdate()
    {
        rb.velocity = new Vector2(0f, inputY * moveSpeed);
    }

    // --- Colisiones ---
    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.collider.CompareTag("Ball"))
        {
            // Cambiar a un color aleatorio cada vez que colisiona con la pelota
            SetColor(Random.ColorHSV());
        }

        if (col.collider.CompareTag("wall"))
        {
            Vector2 normal = col.contacts[0].normal;
            if (normal.y < 0) blockedUp = true;
            if (normal.y > 0) blockedDown = true;
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

    void SetColor(Color c)
    {
        if (spriteRenderer != null) spriteRenderer.color = c;
    }
}

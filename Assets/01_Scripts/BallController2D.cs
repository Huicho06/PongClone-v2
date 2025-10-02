using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class BallController2D : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float speed = 8f;
    [SerializeField] private float serveDelay = 0.75f;
    [SerializeField, Range(0f, 0.9f)] private float minYRatio = 0.25f;

    [Header("Rebote con paddle (player)")]
    [SerializeField, Range(0f, 1f)] private float controlFactor = 0.75f;

    [Tooltip("Al chocar con el paddle, aumenta levemente la velocidad")]
    [SerializeField] private float speedGainOnPaddle = 0.25f;

    private Rigidbody2D rb;
    private Collider2D col;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();

        // Recomendado para Pong: sin gravedad y colisiones discretas
        rb.gravityScale = 0f;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    private void OnEnable()
    {
        ResetAndServe();
    }

    public void ResetAndServe()
    {
        StopAllCoroutines();
        transform.position = Vector3.zero;
        rb.velocity = Vector2.zero;
        StartCoroutine(ServeAfterDelay());
    }

    private System.Collections.IEnumerator ServeAfterDelay()
    {
        yield return new WaitForSeconds(serveDelay);
        LaunchRandom();
    }

    private void LaunchRandom()
    {
        int dirX = Random.value < 0.5f ? -1 : 1; // -1 = izquierda, 1 = derecha

        // Componente Y aleatoria con magnitud mínima (según minYRatio)
        float y = Random.Range(-1f, 1f);
        if (Mathf.Abs(y) < minYRatio)
        {
            y = Mathf.Sign(y == 0 ? 1 : y) * minYRatio;
        }

        Vector2 dir = new Vector2(dirX, y).normalized;
        rb.velocity = dir * speed;
    }

    private void FixedUpdate()
    {
        // Mantener velocidad constante (evita acumulación/deriva numérica)
        if (rb.velocity.sqrMagnitude > 0.01f)
        {
            rb.velocity = rb.velocity.normalized * speed;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        string tag = collision.collider.tag;

        if (tag == "wall")
        {
            // Rebote físico usando la normal del primer contacto
            Vector2 inVel = rb.velocity;
            if (collision.contactCount > 0)
            {
                Vector2 normal = collision.GetContact(0).normal;
                Vector2 reflected = Vector2.Reflect(inVel, normal).normalized * speed;
                rb.velocity = reflected;
            }
            else
            {
                // Respaldo: invierte Y si por algún motivo no hay contactos
                rb.velocity = new Vector2(inVel.x, -inVel.y).normalized * speed;
            }
        }
        else if (tag == "player")
        {
            // Rebote tipo Pong: la dirección depende del punto de impacto en el paddle
            Transform paddle = collision.collider.transform;

            // Diferencia vertical respecto al centro del paddle, normalizada por su altura
            float paddleHeight = collision.collider.bounds.size.y;
            float yDelta = (transform.position.y - paddle.position.y) / (paddleHeight * 0.5f);
            yDelta = Mathf.Clamp(yDelta, -1f, 1f);

            // Hacia dónde debe ir en X (si golpea paddle izquierdo ? +X, derecho ? -X)
            float xDir = transform.position.x < paddle.position.x ? -1f : 1f;
            xDir *= -1f;

            Vector2 controlledDir = new Vector2(xDir, yDelta).normalized;

            Vector2 reflectDir = rb.velocity;
            if (collision.contactCount > 0)
            {
                Vector2 normal = collision.GetContact(0).normal;
                reflectDir = Vector2.Reflect(rb.velocity, normal).normalized;
            }

            Vector2 finalDir = Vector2.Lerp(reflectDir, controlledDir, controlFactor).normalized;

            speed += speedGainOnPaddle;
            rb.velocity = finalDir * speed;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("wallPoint0"))
        {
            Debug.Log("¡Gol del Jugador 1!");
            ResetAndServe();
        }
        else if (other.CompareTag("wallPoint1"))
        {
            Debug.Log("¡Gol del Jugador 0!");
            ResetAndServe();
        }
    }

}

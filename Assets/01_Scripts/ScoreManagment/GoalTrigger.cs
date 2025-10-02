using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class GoalTrigger : MonoBehaviour
{
    [Tooltip("1 si este wallPoint suma punto al Jugador 1; 2 si suma al Jugador 2")]
    public int pointToPlayer = 1;

    [Tooltip("Tag que debe tener la bola")]
    public string ballTag = "ball";

    private void Reset()
    {
        var col = GetComponent<Collider2D>();
        if (col) col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(ballTag)) return;

        if (ScoreManagment.Instance != null)
        {
            ScoreManagment.Instance.AddPointToPlayer(pointToPlayer);
        }
    }
}

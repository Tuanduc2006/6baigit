using UnityEngine;

public class DinoDie : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Cactus"))
        {
            FindObjectOfType<GameOverManager>().GameOver();
        }
    }
}

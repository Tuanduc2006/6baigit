using UnityEngine;

public class BirdController : MonoBehaviour
{
    public float jumpForce = 5f;
    Rigidbody2D rb;
    bool canControl = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.simulated = false;
    }

    void Update()
    {
        if (!canControl) return;

        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            AudioManager.instance.PlayPress();
        }

        if (Input.GetMouseButtonUp(0) || Input.GetKeyUp(KeyCode.Space))
        {
            Flap();
            AudioManager.instance.PlayRelease();
        }
    }


    void Flap()
    {
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    public void EnableControl()
    {
        canControl = true;
        rb.simulated = true;
    }

    public void DisableControl()
    {
        canControl = false;
        rb.simulated = false;
    }

    public void ResetBird()
    {
        canControl = false;
        rb.simulated = false;
        rb.linearVelocity = Vector2.zero;
        transform.position = Vector3.zero; // hoặc vị trí spawn
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (GameManager.instance.state != GameState.Playing) return;

        if (collision.gameObject.CompareTag("Pipe"))
        {
            AudioManager.instance.PlayDiePipe();
            GameManager.instance.GameOver();
        }
        else if (collision.gameObject.CompareTag("Ground"))
        {
            AudioManager.instance.PlayDieFall();
            GameManager.instance.GameOver();
        }
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if (GameManager.instance.state != GameState.Playing) return;

        if (other.CompareTag("ScoreZone"))
        {
            ScoreManager.instance.AddScore();
        }
    }

}

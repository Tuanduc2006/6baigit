using UnityEngine;

public class DinoController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;

    [Header("Game Manager")]
    public GameManager gameManager;

    [Header("Cai dat Nhay")]
    public float jumpForce = 10f;
    public float jumpTime = 0.35f;
    private float jumpTimeCounter;
    private bool isJumping;

    [Header("Kiem tra Cham dat")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask whatIsGround;
    private bool isGrounded;

    [Header("Âm thanh")]
    public AudioSource audioSource; // Loa số 1: Dành cho tiếng nhảy
    public AudioClip jumpSound;

    public AudioSource runAudioSource; // Loa số 2 (MỚI): Dành cho tiếng chạy

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        if (gameManager == null)
        {
            gameManager = FindFirstObjectByType<GameManager>();
        }
    }

    void Update()
    {
        // 1. NẾU GAME CHƯA BẮT ĐẦU HOẶC ĐÃ CHẾT: Tắt tiếng chạy và không làm gì cả
        if (gameManager != null && (!gameManager.isGameStarted || anim.GetBool("isDead")))
        {
            if (runAudioSource != null && runAudioSource.isPlaying)
            {
                runAudioSource.Stop();
            }
            return;
        }

        // Kiểm tra xem khủng long có chạm đất không
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);

        // 2. XỬ LÝ TIẾNG BƯỚC CHÂN CHẠY
        if (runAudioSource != null)
        {
            if (isGrounded)
            {
                // Nếu đang chạm đất mà nhạc chạy chưa bật thì bật lên
                if (!runAudioSource.isPlaying) runAudioSource.Play();
            }
            else
            {
                // Nếu đang bay trên không thì tắt tiếng chạy
                if (runAudioSource.isPlaying) runAudioSource.Stop();
            }
        }

        // Tắt hiệu ứng nhảy khi chạm đất
        if (isGrounded && rb.linearVelocity.y <= 0)
        {
            anim.SetBool("isJumping", false);
        }

        // 3. KHI BẤM NHẢY
        if (Input.GetMouseButtonDown(0) && isGrounded)
        {
            isJumping = true;
            jumpTimeCounter = jumpTime;
            rb.linearVelocity = Vector2.up * jumpForce;

            anim.SetBool("isJumping", true);

            // Phát tiếng nhảy
            if (audioSource != null && jumpSound != null)
            {
                audioSource.PlayOneShot(jumpSound);
            }
        }

        // Giữ phím để nhảy cao hơn
        if (Input.GetMouseButton(0) && isJumping)
        {
            if (jumpTimeCounter > 0)
            {
                rb.linearVelocity = Vector2.up * jumpForce;
                jumpTimeCounter -= Time.deltaTime;
            }
            else
            {
                isJumping = false;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            isJumping = false;
        }
    }

    // KHI ĐÂM CHƯỚNG NGẠI VẬT
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Obstacle"))
        {
            anim.SetBool("isDead", true);

            // Tắt ngay tiếng chạy khi chết
            if (runAudioSource != null)
            {
                runAudioSource.Stop();
            }

            if (gameManager != null)
            {
                gameManager.GameOver();
            }
        }
    }
}

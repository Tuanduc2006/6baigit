using UnityEngine;

public class BirdController : MonoBehaviour
{
    [Header("Cài đặt bay")]
    public float jumpForce = 7f;
    public float smoothRotation = 5f;

    private Rigidbody2D rb;
    private Animator anim;
    public bool isDead = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        // Khóa vật lý lúc đầu để chim lơ lửng, không bị rơi
        rb.bodyType = RigidbodyType2D.Kinematic;
    }

    void Update()
    {
        if (isDead) return;

        // Nhận diện click hoặc chạm màn hình
        if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
        {
            // Nếu game chưa bắt đầu thì chặn lại, không cho làm gì cả
            if (!GameManager.instance.gameStarted) return;

            Fly();
        }

        // Nếu game đã bắt đầu (bấm nút) mà chim vẫn đang bị khóa vật lý -> Mở khóa
        if (GameManager.instance.gameStarted && rb.bodyType == RigidbodyType2D.Kinematic)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            Fly(); // Tự động nảy lên nhịp đầu tiên cho mượt
        }
    }

    void FixedUpdate()
    {
        // Chỉ xoay đầu khi game đang chạy và chim đã được mở khóa vật lý
        if (!isDead && GameManager.instance.gameStarted && rb.bodyType == RigidbodyType2D.Dynamic)
        {
            float angle = Mathf.Clamp((rb.linearVelocity.y * 10f), -90f, 45f);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, angle), Time.deltaTime * smoothRotation);
        }
    }

    void Fly()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

        anim.SetTrigger("Flap");
        GameManager.instance.PlayFlapSound();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead) return;

        isDead = true;
        GameManager.instance.GameOver();
        Debug.Log("Chim đã va chạm và Game Over!");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isDead && collision.gameObject.CompareTag("Score"))
        {
            GameManager.instance.AddScore();
            Debug.Log("Đã qua ống thành công, +1 điểm!");
        }
    }
}
using UnityEngine;

public class ScrollBackground : MonoBehaviour
{
    public float baseSpeed = 1.5f; // Đổi tốc độ nhỏ lại cho phù hợp với nền
    private float width;
    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;

        // Lấy đúng chiều rộng chuẩn của tấm Mẹ
        width = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void Update()
    {
        // 1. Nếu game chưa bắt đầu -> Không cuộn nền
        if (GameManager.instance != null && !GameManager.instance.gameStarted) return;

        float currentSpeed = baseSpeed;

        // 2. Lấy tốc độ Global (Lưu ý: Bài trước biến tên là globalSpeed, nếu bạn đổi thành gameSpeed thì sửa lại chỗ này nhé)
        if (GameManager.instance != null)
        {
            currentSpeed = baseSpeed * GameManager.instance.globalSpeed;
        }

        // 3. Di chuyển tấm Mẹ sang trái (tấm Con gắn trên lưng sẽ tự động bị kéo theo)
        transform.Translate(Vector3.left * currentSpeed * Time.deltaTime);

        // 4. Khi Mẹ chạy lùi qua đúng 1 chiều rộng, bế cả 2 mẹ con nhảy vọt về lại vạch xuất phát
        if (transform.position.x <= startPos.x - width)
        {
            // Tính toán phần dư do vượt quá để reset không bị khựng/giật khung hình
            float offset = transform.position.x - (startPos.x - width);
            transform.position = new Vector3(startPos.x + offset, startPos.y, startPos.z);
        }
    }
}
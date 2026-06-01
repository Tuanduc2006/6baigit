using UnityEngine;

public class ScrollBackground : MonoBehaviour
{
    public float baseSpeed = 5f;
    private float width;
    private GameManager gameManager;
    private Vector3 startPos;

    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        startPos = transform.position;

        // Lấy đúng chiều rộng chuẩn, không dùng mẹo trừ bớt nữa
        width = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void Update()
    {
        float currentSpeed = baseSpeed;
        if (gameManager != null) currentSpeed = baseSpeed * gameManager.gameSpeed;

        // Di chuyển tấm Mẹ sang trái (tấm Con gắn trên lưng sẽ tự động bị kéo theo)
        transform.Translate(Vector3.left * currentSpeed * Time.deltaTime);

        // Khi Mẹ chạy lùi qua đúng 1 chiều rộng, bế cả 2 mẹ con nhảy vọt về lại vạch xuất phát
        if (transform.position.x <= startPos.x - width)
        {
            // Tính toán phần dư do vượt quá để reset không bị khựng/giật khung hình
            float offset = transform.position.x - (startPos.x - width);
            transform.position = new Vector3(startPos.x + offset, startPos.y, startPos.z);
        }
    }
}

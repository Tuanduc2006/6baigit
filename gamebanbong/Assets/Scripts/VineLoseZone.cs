using UnityEngine;

// Script này dùng cho vùng thua game
// Ví dụ: dây leo ở phía dưới màn hình
// Khi bóng chạm vào dây leo thì game over
public class VineLoseZone : MonoBehaviour
{
    [Header("Game Manager")]
    // Tham chiếu tới GameManager để gọi hàm GameOver()
    public GameManager gameManager;

    [Header("Cài đặt")]
    // Nếu bật true:
    // Chỉ bóng đang được bắn mới làm thua game
    //
    // Nếu tắt false:
    // Bất kỳ quả bóng nào chạm vào dây leo cũng làm thua game
    public bool onlyShotBubbleCanLose = true;

    private void Awake()
    {
        // Nếu chưa kéo GameManager vào Inspector
        // thì tự tìm GameManager trong Scene
        if (gameManager == null)
        {
            gameManager = FindFirstObjectByType<GameManager>();
        }
    }

    // Hàm này chạy khi có object đi vào vùng Trigger
    // Dùng khi Collider của dây leo bật Is Trigger
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Kiểm tra object vừa chạm có làm thua game không
        CheckLose(other.gameObject);
    }

    // Hàm này chạy khi có va chạm vật lý bình thường
    // Dùng khi Collider của dây leo không bật Is Trigger
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Kiểm tra object vừa va chạm có làm thua game không
        CheckLose(collision.gameObject);
    }

    // Kiểm tra điều kiện thua game
    private void CheckLose(GameObject obj)
    {
        // Nếu chưa có GameManager thì không xử lý
        if (gameManager == null) return;

        // Nếu game đã kết thúc rồi thì không xử lý nữa
        if (gameManager.IsGameEnded) return;

        // Kiểm tra object chạm vào có phải là bóng không
        Bubble bubble = obj.GetComponent<Bubble>();

        // Nếu không phải bóng thì bỏ qua
        if (bubble == null)
        {
            return;
        }

        // Nếu chỉ cho bóng đang bắn làm thua game
        if (onlyShotBubbleCanLose)
        {
            // Kiểm tra quả bóng đó có script BubbleProjectile không
            // BubbleProjectile thường chỉ có trên bóng đang bay sau khi bắn
            BubbleProjectile projectile = obj.GetComponent<BubbleProjectile>();

            // Nếu không có BubbleProjectile
            // nghĩa là không phải bóng đang bắn, nên không thua
            if (projectile == null)
            {
                return;
            }
        }

        // Nếu qua hết điều kiện thì gọi Game Over
        gameManager.GameOver();
    }
}
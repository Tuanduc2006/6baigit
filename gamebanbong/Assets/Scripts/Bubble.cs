using UnityEngine;

// Yêu cầu GameObject gắn script này bắt buộc phải có SpriteRenderer
// SpriteRenderer dùng để hiển thị hình ảnh quả bóng
[RequireComponent(typeof(SpriteRenderer))]

// Yêu cầu GameObject phải có CircleCollider2D
// Collider hình tròn dùng để va chạm với bóng khác, tường, dây leo...
[RequireComponent(typeof(CircleCollider2D))]

// Yêu cầu GameObject phải có Rigidbody2D
// Rigidbody2D dùng để điều khiển vật lý như bay, rơi, va chạm
[RequireComponent(typeof(Rigidbody2D))]
public class Bubble : MonoBehaviour
{
    // Mã màu của quả bóng
    // Ví dụ: 0 = đỏ, 1 = xanh, 2 = vàng, 3 = tím
    public int colorId;

    // Lưu SpriteRenderer để thay đổi hình ảnh quả bóng
    [HideInInspector] public SpriteRenderer spriteRenderer;

    // Lưu CircleCollider2D để bật/tắt va chạm
    [HideInInspector] public CircleCollider2D circleCollider;

    // Lưu Rigidbody2D để điều khiển vật lý của bóng
    [HideInInspector] public Rigidbody2D rb;

    private void Awake()
    {
        // Lấy component SpriteRenderer trên GameObject
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Lấy component CircleCollider2D trên GameObject
        circleCollider = GetComponent<CircleCollider2D>();

        // Lấy component Rigidbody2D trên GameObject
        rb = GetComponent<Rigidbody2D>();
    }

    // Hàm thiết lập màu và hình ảnh cho quả bóng
    public void Setup(int newColorId, Sprite sprite)
    {
        // Gán mã màu mới cho bóng
        colorId = newColorId;

        // Gán hình ảnh sprite tương ứng với màu bóng
        spriteRenderer.sprite = sprite;

        // Đổi tên GameObject để dễ nhìn trong Hierarchy
        name = "Bubble_Color_" + colorId;
    }

    // Trạng thái bóng đang nằm trên súng, chuẩn bị được bắn
    public void SetAsLoadedBubble()
    {
        // Kinematic nghĩa là bóng không bị vật lý tác động
        // Bóng sẽ đứng yên theo vị trí mình đặt
        rb.bodyType = RigidbodyType2D.Kinematic;

        // Dừng mọi chuyển động của bóng
        rb.linearVelocity = Vector2.zero;

        // Dừng xoay bóng
        rb.angularVelocity = 0;

        // Không cho bóng rơi xuống
        rb.gravityScale = 0;

        // Tắt collider để bóng trên súng không va chạm lung tung
        circleCollider.enabled = false;

        // Không dùng trigger
        circleCollider.isTrigger = false;
    }

    // Trạng thái bóng đã nằm cố định trên bảng/lưới bóng
    public void SetAsGridBubble()
    {
        // Bóng đứng yên, không chịu tác động vật lý
        rb.bodyType = RigidbodyType2D.Kinematic;

        // Không cho bóng di chuyển
        rb.linearVelocity = Vector2.zero;

        // Không cho bóng xoay
        rb.angularVelocity = 0;

        // Không cho bóng rơi
        rb.gravityScale = 0;

        // Bật collider để bóng khác có thể nhận biết va chạm
        circleCollider.enabled = true;

        // Đặt là Trigger để bóng bắn vào có thể phát hiện chạm
        // nhưng không bị lực vật lý đẩy nhau
        circleCollider.isTrigger = true;
    }

    // Trạng thái bóng đang được bắn ra khỏi súng
    public void SetAsShotBubble(PhysicsMaterial2D bounceMaterial)
    {
        // Dynamic nghĩa là bóng chịu tác động vật lý
        // Bóng có thể di chuyển và va chạm
        rb.bodyType = RigidbodyType2D.Dynamic;

        // Không cho bóng bị trọng lực kéo xuống khi đang bay
        rb.gravityScale = 0;

        // Giúp phát hiện va chạm chính xác khi bóng bay nhanh
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        // Không cho bóng xoay khi bay
        rb.freezeRotation = true;

        // Bật collider để bóng có thể va chạm với tường hoặc bóng khác
        circleCollider.enabled = true;

        // Không dùng trigger vì bóng đang bắn cần va chạm vật lý với tường
        circleCollider.isTrigger = false;

        // Gán vật liệu nảy cho bóng
        // Dùng để bóng bật lại khi chạm tường
        circleCollider.sharedMaterial = bounceMaterial;
    }

    // Trạng thái bóng bị rơi xuống sau khi không còn dính với cụm bóng chính
    public void SetAsFallingBubble()
    {
        // Cho bóng chịu tác động vật lý
        rb.bodyType = RigidbodyType2D.Dynamic;

        // Tăng trọng lực để bóng rơi xuống
        rb.gravityScale = 1.3f;

        // Bật collider
        circleCollider.enabled = true;

        // Dùng trigger để bóng rơi không va chạm làm đẩy các bóng khác
        circleCollider.isTrigger = true;
    }
}
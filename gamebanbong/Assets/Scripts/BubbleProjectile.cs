using UnityEngine;

// Yêu cầu GameObject gắn script này bắt buộc phải có script Bubble
// Vì BubbleProjectile cần lấy thông tin của quả bóng đang bay
[RequireComponent(typeof(Bubble))]
public class BubbleProjectile : MonoBehaviour
{
    // Tham chiếu đến script Shooter
    // Dùng để gọi hàm AttachShotBubble khi bóng cần gắn vào lưới
    private Shooter shooter;

    // Tham chiếu đến script Bubble trên chính quả bóng này
    private Bubble bubble;

    // Biến kiểm tra bóng đã được gắn vào lưới hay chưa
    // Tránh trường hợp bóng va chạm nhiều lần và bị gắn nhiều lần
    private bool attached;

    // Hàm khởi tạo cho bóng vừa được bắn
    // Hàm này thường được Shooter gọi sau khi tạo bóng bắn
    public void Init(Shooter newShooter)
    {
        // Lưu lại Shooter đang điều khiển quả bóng này
        shooter = newShooter;

        // Lấy component Bubble trên GameObject hiện tại
        bubble = GetComponent<Bubble>();

        // Đánh dấu bóng chưa gắn vào lưới
        attached = false;

        // Bật script này lên để bắt đầu kiểm tra va chạm
        enabled = true;
    }

    // Hàm này chạy khi bóng đi vào một Collider dạng Trigger
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Nếu bóng đã gắn rồi thì không xử lý nữa
        if (attached) return;

        // Kiểm tra vật vừa chạm có phải là Bubble không
        Bubble otherBubble = other.GetComponent<Bubble>();

        // Nếu chạm vào một quả bóng khác
        // và quả đó không phải chính nó
        if (otherBubble != null && otherBubble != bubble)
        {
            // Gắn bóng bắn vào lưới
            AttachToGrid();
        }
    }

    // Hàm này chạy khi bóng va chạm vật lý với Collider không phải Trigger
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Nếu bóng đã gắn rồi thì không xử lý nữa
        if (attached) return;

        // Nếu bóng chạm vào tường trên
        if (collision.collider.CompareTag("TopWall"))
        {
            // Gắn bóng vào lưới
            AttachToGrid();
            return;
        }

        // Kiểm tra vật vừa va chạm có phải là Bubble không
        Bubble otherBubble = collision.collider.GetComponent<Bubble>();

        // Nếu bóng bắn chạm vào một quả bóng khác
        if (otherBubble != null && otherBubble != bubble)
        {
            // Gắn bóng vào lưới
            AttachToGrid();
        }
    }

    // Hàm xử lý khi bóng cần được gắn vào lưới
    private void AttachToGrid()
    {
        // Nếu bóng đã gắn rồi thì không xử lý lại
        if (attached) return;

        // Đánh dấu bóng đã gắn
        attached = true;

        // Nếu chưa có tham chiếu Bubble thì lấy lại
        if (bubble == null)
        {
            bubble = GetComponent<Bubble>();
        }

        // Nếu có đủ Shooter và Bubble thì gọi Shooter xử lý gắn bóng
        if (shooter != null && bubble != null)
        {
            shooter.AttachShotBubble(bubble);
        }
        else
        {
            // Báo lỗi nếu thiếu Shooter hoặc Bubble
            Debug.LogError("BubbleProjectile thiếu Shooter hoặc Bubble.");
        }
    }
}
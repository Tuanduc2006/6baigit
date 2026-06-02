using System.Collections.Generic;
using UnityEngine;

// Yêu cầu GameObject gắn script này phải có LineRenderer
// LineRenderer dùng để vẽ đường ngắm trong game
[RequireComponent(typeof(LineRenderer))]
public class TrajectoryLine : MonoBehaviour
{
    // LayerMask xác định những vật mà đường ngắm có thể va chạm
    // Ví dụ: tường trái, tường phải, tường trên, bóng trên lưới
    public LayerMask hitMask;

    // Số lần đường ngắm được phép nảy khi chạm tường
    public int maxBounceCount = 5;

    // Tổng khoảng cách tối đa của đường ngắm
    public float maxDistance = 15f;

    // Component LineRenderer dùng để vẽ đường
    private LineRenderer lineRenderer;

    private void Awake()
    {
        // Lấy component LineRenderer trên GameObject hiện tại
        lineRenderer = GetComponent<LineRenderer>();

        // Ban đầu ẩn đường ngắm
        Hide();
    }

    // Hiển thị đường ngắm
    // startPosition: vị trí bắt đầu, thường là đầu nòng súng
    // direction: hướng bắn hiện tại của súng
    public void Show(Vector2 startPosition, Vector2 direction)
    {
        // Danh sách các điểm tạo thành đường ngắm
        List<Vector3> points = new List<Vector3>();

        // Điểm đầu tiên là vị trí đầu nòng súng
        points.Add(startPosition);

        // Vị trí hiện tại của tia ray
        Vector2 currentPosition = startPosition;

        // Hướng hiện tại của tia ray
        // normalized để hướng có độ dài chuẩn bằng 1
        Vector2 currentDirection = direction.normalized;

        // Khoảng cách còn lại mà đường ngắm có thể đi
        float remainingDistance = maxDistance;

        // Lặp để kiểm tra tia ray có chạm vật gì không
        // i <= maxBounceCount nghĩa là cho phép tối đa số lần nảy đã khai báo
        for (int i = 0; i <= maxBounceCount; i++)
        {
            // Bắn một tia Raycast2D từ vị trí hiện tại theo hướng hiện tại
            // Nếu tia chạm object thuộc hitMask thì trả về thông tin va chạm
            RaycastHit2D hit = Physics2D.Raycast(
                currentPosition,
                currentDirection,
                remainingDistance,
                hitMask
            );

            // Nếu tia không chạm vật gì
            if (hit.collider == null)
            {
                // Thêm điểm cuối của đường ngắm theo khoảng cách còn lại
                points.Add(currentPosition + currentDirection * remainingDistance);

                // Dừng vòng lặp
                break;
            }

            // Nếu có va chạm, thêm điểm va chạm vào danh sách điểm
            points.Add(hit.point);

            // Nếu chạm vào bóng hoặc tường trên thì dừng đường ngắm
            // Vì đây là nơi bóng sẽ dính lại
            if (hit.collider.GetComponent<Bubble>() != null || hit.collider.CompareTag("TopWall"))
            {
                break;
            }

            // Nếu chạm tường trái/phải thì tính hướng phản xạ để đường ngắm nảy lại
            currentDirection = Vector2.Reflect(currentDirection, hit.normal).normalized;

            // Dịch vị trí bắt đầu mới ra khỏi điểm va chạm một chút
            // Tránh raycast bị kẹt ngay tại bề mặt tường
            currentPosition = hit.point + currentDirection * 0.03f;

            // Trừ đi khoảng cách đã đi
            remainingDistance -= hit.distance;
        }

        // Cập nhật số điểm cho LineRenderer
        lineRenderer.positionCount = points.Count;

        // Gán các điểm cho LineRenderer để vẽ đường
        lineRenderer.SetPositions(points.ToArray());

        // Bật hiển thị đường ngắm
        lineRenderer.enabled = true;
    }

    // Ẩn đường ngắm
    public void Hide()
    {
        lineRenderer.enabled = false;
    }
}
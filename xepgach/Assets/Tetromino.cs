using UnityEngine;

public class Tetromino : MonoBehaviour
{
    // Tốc độ trượt tự động (đơn vị/giây)
    public float fallSpeed = 2f;

    void Update()
    {
        // 1. Phím di chuyển ngang và xoay
        if (Input.GetKeyDown(KeyCode.A)) { MoveLeft(); }
        else if (Input.GetKeyDown(KeyCode.S)) { MoveRight(); }
        else if (Input.GetKeyDown(KeyCode.Space)) { Rotate(); }

        // 2. RƠI MƯỢT: Khối gạch sẽ trượt xuống liên tục
        float currentSpeed = Input.GetKey(KeyCode.X) ? fallSpeed * 10f : fallSpeed;
        transform.position += new Vector3(0, -currentSpeed * Time.deltaTime, 0);

        // 3. XỬ LÝ VA CHẠM: Chạm là ĐỨNG IM NGAY LẬP TỨC
        if (!IsValidPosition())
        {
            // Ép tọa độ để nó nằm ngay ngắn trên mặt lưới (không bị lún)
            transform.position = new Vector3(
                Mathf.Round(transform.position.x),
                Mathf.Ceil(transform.position.y),
                0
            );

            // Bảo hiểm chống lún tuyệt đối
            while (!IsValidPosition())
            {
                transform.position += new Vector3(0, 1, 0);
            }

            // CHỐT GẠCH VÀ SINH GẠCH MỚI LUÔN!
            LockAndSpawn();
        }
    }

    public void MoveLeft()
    {
        transform.position += new Vector3(-1, 0, 0);
        if (!IsValidPosition()) { transform.position += new Vector3(1, 0, 0); }
    }

    public void MoveRight()
    {
        transform.position += new Vector3(1, 0, 0);
        if (!IsValidPosition()) { transform.position += new Vector3(-1, 0, 0); }
    }

    public void Rotate()
    {
        transform.Rotate(0, 0, -90);
        if (!IsValidPosition()) { transform.Rotate(0, 0, 90); }
    }

    // Hàm kiểm tra va chạm cực nhạy
    public bool IsValidPosition()
    {
        foreach (Transform child in transform)
        {
            int roundX = Mathf.RoundToInt(child.position.x);
            int floorY = Mathf.FloorToInt(child.position.y);

            // Kiểm tra tường và đáy
            if (roundX < 0 || roundX >= GridMap.width || floorY < 0) return false;

            // Kiểm tra đè lên gạch cũ
            if (floorY < GridMap.height && GridMap.grid[roundX, floorY] != null) return false;
        }
        return true;
    }

    void LockAndSpawn()
    {
        // 1. Lưu vị trí vào mảng
        foreach (Transform child in transform)
        {
            int roundX = Mathf.RoundToInt(child.position.x);
            int roundY = Mathf.RoundToInt(child.position.y);

            if (roundX >= 0 && roundX < GridMap.width && roundY >= 0 && roundY < GridMap.height)
            {
                GridMap.grid[roundX, roundY] = child;
            }
        }

        // 2. Xóa hàng nếu đầy
        GridMap.ClearFullRows();

        // 3. Tắt script này để khối gạch thành vật chết
        this.enabled = false;

        // 4. Gọi GameManager sinh ngay khối mới!
        FindFirstObjectByType<GameManager>().SpawnNext();
    }
}
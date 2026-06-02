using System.Collections.Generic;
using UnityEngine;

public class BubbleGrid : MonoBehaviour
{
    [Header("Prefab và Sprite")]
    // Prefab quả bóng dùng để sinh bóng mới trên lưới
    public Bubble bubblePrefab;

    // Mảng sprite chứa các hình ảnh màu bóng
    // Ví dụ: đỏ, xanh, vàng, tím
    public Sprite[] bubbleSprites;

    [Header("Kích thước lưới")]
    // Số hàng tối đa của lưới bóng
    public int rows = 12;

    // Số cột tối đa của lưới bóng
    public int columns = 14;

    // Số hàng bóng được tạo sẵn lúc bắt đầu màn chơi
    public int startFilledRows = 5;

    [Header("Tự động dàn bóng theo màn hình / tường")]
    // Nếu bật, lưới bóng sẽ tự căn theo vị trí tường trái, tường phải và tường trên
    public bool autoFitToWalls = true;

    // Tên GameObject của tường trái trong Hierarchy
    public string leftWallName = "LeftWall";

    // Tên GameObject của tường phải trong Hierarchy
    public string rightWallName = "RightWall";

    // Tên GameObject của tường trên trong Hierarchy
    public string topWallName = "TopWall";

    [Tooltip("Cách mép trái/phải một chút. Để 0 nếu muốn sát tường.")]
    // Khoảng cách lùi vào từ mép trái và phải
    public float horizontalPadding = 0f;

    [Tooltip("Cách tường trên một chút.")]
    // Khoảng cách lùi xuống từ tường trên
    public float topPadding = 0.1f;

    [Header("Căn chỉnh lưới thủ công nếu tắt Auto Fit")]
    // Khoảng cách giữa các quả bóng theo chiều ngang
    public float cellSize = 0.72f;

    // Hệ số chiều cao giữa các hàng
    // Vì bóng xếp kiểu tổ ong nên chiều cao hàng thường nhỏ hơn cellSize
    public float rowHeightMultiplier = 0.86f;

    // Vị trí quả bóng đầu tiên ở góc trên bên trái
    public Vector2 topLeft = new Vector2(-3.2f, 4f);

    [Header("Hiệu ứng")]
    // Hiệu ứng nổ/xóa bóng khi bóng biến mất
    public ParticleSystem popEffectPrefab;

    // Mảng 2 chiều lưu toàn bộ bóng trên lưới
    // grid[row, col] nghĩa là bóng ở hàng row, cột col
    private Bubble[,] grid;

    // Giới hạn bên trái của vùng chơi
    private float leftLimit;

    // Giới hạn bên phải của vùng chơi
    private float rightLimit;

    // Giới hạn phía trên của vùng chơi
    private float topLimit;

    private void Awake()
    {
        // Khởi tạo mảng lưới bóng theo số hàng và số cột
        grid = new Bubble[rows, columns];
    }

    // Tạo màn chơi mới theo level
    public void CreateLevel(int level)
    {
        // Cập nhật vị trí lưới theo tường hoặc camera
        UpdateGridLayout();

        // Xóa lưới cũ nếu có
        ClearGrid();

        // Tính số hàng bóng ban đầu
        // Level càng cao thì số hàng bóng càng nhiều
        // Mathf.Clamp để giới hạn không quá ít và không quá nhiều
        int filledRows = Mathf.Clamp(startFilledRows + level / 2, 3, rows - 3);

        // Lặp qua từng hàng cần tạo bóng
        for (int r = 0; r < filledRows; r++)
        {
            // Lấy số cột của hàng hiện tại
            // Hàng lẻ có thể ít hơn 1 bóng nếu autoFitToWalls bật
            int colCount = GetColumnCountForRow(r);

            for (int c = 0; c < colCount; c++)
            {
                // Chọn màu ngẫu nhiên cho bóng
                int colorId = Random.Range(0, bubbleSprites.Length);

                // Sinh bóng tại hàng r, cột c
                SpawnBubble(r, c, colorId);
            }
        }
    }

    // Tạo màn chơi từ dữ liệu có sẵn
    // data là mảng 2 chiều chứa mã màu bóng
    // Ví dụ: -1 là ô trống, 0/1/2/3 là các màu bóng
    public void LoadLevelFromData(int[,] data)
    {
        // Cập nhật lại cách căn lưới
        UpdateGridLayout();

        // Xóa lưới cũ
        ClearGrid();

        // Lấy số hàng và số cột từ dữ liệu truyền vào
        int dataRows = data.GetLength(0);
        int dataCols = data.GetLength(1);

        for (int r = 0; r < dataRows && r < rows; r++)
        {
            int colCount = GetColumnCountForRow(r);

            for (int c = 0; c < dataCols && c < colCount; c++)
            {
                int colorId = data[r, c];

                // Nếu colorId hợp lệ thì tạo bóng
                if (colorId >= 0 && colorId < bubbleSprites.Length)
                {
                    SpawnBubble(r, c, colorId);
                }
            }
        }
    }

    // Cập nhật vị trí, kích thước của lưới bóng
    private void UpdateGridLayout()
    {
        // Nếu không bật tự động căn theo tường thì dùng thông số thủ công
        if (!autoFitToWalls)
        {
            return;
        }

        // Tìm tường trái, tường phải, tường trên theo tên
        Transform leftWall = GameObject.Find(leftWallName)?.transform;
        Transform rightWall = GameObject.Find(rightWallName)?.transform;
        Transform topWall = GameObject.Find(topWallName)?.transform;

        bool hasWalls = leftWall != null && rightWall != null;

        // Nếu có đủ tường trái và phải thì lấy giới hạn từ collider của tường
        if (hasWalls)
        {
            leftLimit = GetRightEdge(leftWall);
            rightLimit = GetLeftEdge(rightWall);
        }
        else
        {
            // Nếu không tìm thấy tường thì lấy giới hạn theo Camera
            Camera cam = Camera.main;
            float distance = Mathf.Abs(cam.transform.position.z);

            leftLimit = cam.ViewportToWorldPoint(new Vector3(0, 0, distance)).x;
            rightLimit = cam.ViewportToWorldPoint(new Vector3(1, 0, distance)).x;
        }

        // Nếu có tường trên thì lấy giới hạn dưới của tường trên
        if (topWall != null)
        {
            topLimit = GetBottomEdge(topWall);
        }
        else
        {
            // Nếu không có tường trên thì lấy theo mép trên Camera
            Camera cam = Camera.main;
            float distance = Mathf.Abs(cam.transform.position.z);
            topLimit = cam.ViewportToWorldPoint(new Vector3(0, 1, distance)).y;
        }

        // Tính chiều rộng vùng có thể đặt bóng
        float availableWidth = rightLimit - leftLimit - horizontalPadding * 2f;

        // Đảm bảo số cột tối thiểu là 2
        if (columns < 2)
        {
            columns = 2;
        }

        // Chia đều bóng theo chiều ngang
        // Hàng chẵn có đủ columns bóng
        // Hàng lẻ lệch nửa ô và ít hơn 1 bóng để không tràn tường
        cellSize = availableWidth / columns;

        // Tính vị trí bóng đầu tiên ở góc trên bên trái
        topLeft.x = leftLimit + horizontalPadding + cellSize * 0.5f;
        topLeft.y = topLimit - topPadding - cellSize * 0.5f;
    }

    // Lấy mép trái của một tường
    private float GetLeftEdge(Transform wall)
    {
        Collider2D col = wall.GetComponent<Collider2D>();

        if (col != null)
        {
            return col.bounds.min.x;
        }

        return wall.position.x;
    }

    // Lấy mép phải của một tường
    private float GetRightEdge(Transform wall)
    {
        Collider2D col = wall.GetComponent<Collider2D>();

        if (col != null)
        {
            return col.bounds.max.x;
        }

        return wall.position.x;
    }

    // Lấy mép dưới của tường trên
    private float GetBottomEdge(Transform wall)
    {
        Collider2D col = wall.GetComponent<Collider2D>();

        if (col != null)
        {
            return col.bounds.min.y;
        }

        return wall.position.y;
    }

    // Sinh một quả bóng tại hàng row, cột col
    public void SpawnBubble(int row, int col, int colorId)
    {
        // Nếu ô nằm ngoài lưới thì không làm gì
        if (!IsInside(row, col)) return;

        // Lấy vị trí thật trong thế giới Unity
        Vector2 pos = GetWorldPosition(row, col);

        // Tạo bóng từ prefab
        // transform là cha của bóng, giúp bóng nằm gọn trong BubbleGrid
        Bubble bubble = Instantiate(bubblePrefab, pos, Quaternion.identity, transform);

        // Gán màu và sprite cho bóng
        bubble.Setup(colorId, bubbleSprites[colorId]);

        // Đặt bóng thành bóng nằm cố định trên lưới
        bubble.SetAsGridBubble();

        // Lưu bóng vào mảng grid
        grid[row, col] = bubble;
    }

    // Chuyển tọa độ hàng/cột thành vị trí thật trong Scene
    public Vector2 GetWorldPosition(int row, int col)
    {
        // Hàng lẻ lệch sang phải nửa ô để tạo kiểu xếp tổ ong
        float offsetX = row % 2 == 1 ? cellSize * 0.5f : 0f;

        // Tính vị trí X
        float x = topLeft.x + col * cellSize + offsetX;

        // Tính vị trí Y
        float y = topLeft.y - row * cellSize * rowHeightMultiplier;

        return new Vector2(x, y);
    }

    // Tìm ô trống gần nhất để gắn quả bóng vừa bắn vào
    public Vector2Int GetNearestEmptyCell(Vector2 worldPosition)
    {
        // Ô tốt nhất ban đầu là không có
        Vector2Int bestCell = new Vector2Int(-1, -1);

        // Khoảng cách nhỏ nhất ban đầu rất lớn
        float bestDistance = float.MaxValue;

        // Duyệt toàn bộ lưới
        for (int r = 0; r < rows; r++)
        {
            int colCount = GetColumnCountForRow(r);

            for (int c = 0; c < colCount; c++)
            {
                // Nếu ô đã có bóng thì bỏ qua
                if (grid[r, c] != null) continue;

                // Bóng chỉ được gắn vào hàng đầu tiên
                // hoặc gắn vào ô có hàng xóm bên cạnh
                bool canAttach = r == 0 || HasAnyNeighbor(r, c);
                if (!canAttach) continue;

                // Tính khoảng cách từ vị trí va chạm tới ô này
                float distance = Vector2.Distance(worldPosition, GetWorldPosition(r, c));

                // Nếu ô này gần hơn thì chọn làm ô tốt nhất
                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    bestCell = new Vector2Int(r, c);
                }
            }
        }

        return bestCell;
    }

    // Gắn một quả bóng vào ô row, col trong lưới
    public void SetBubble(int row, int col, Bubble bubble)
    {
        if (!IsInside(row, col)) return;

        // Lưu bóng vào mảng
        grid[row, col] = bubble;

        // Đưa bóng thành con của BubbleGrid
        bubble.transform.SetParent(transform);

        // Đặt bóng đúng vị trí ô trong lưới
        bubble.transform.position = GetWorldPosition(row, col);

        // Chuyển bóng sang trạng thái bóng nằm trên lưới
        bubble.SetAsGridBubble();
    }

    // Tìm toàn bộ bóng cùng màu nối liền với bóng ở vị trí startRow, startCol
    public List<Vector2Int> FindMatches(int startRow, int startCol)
    {
        List<Vector2Int> result = new List<Vector2Int>();

        // Nếu ô không hợp lệ hoặc không có bóng thì trả về danh sách rỗng
        if (!IsInside(startRow, startCol)) return result;
        if (grid[startRow, startCol] == null) return result;

        // Lấy màu của bóng bắt đầu
        int targetColor = grid[startRow, startCol].colorId;

        // Mảng đánh dấu ô đã kiểm tra
        bool[,] visited = new bool[rows, columns];

        // Queue dùng để duyệt các bóng nối liền nhau
        Queue<Vector2Int> queue = new Queue<Vector2Int>();

        // Thêm ô bắt đầu vào hàng đợi
        queue.Enqueue(new Vector2Int(startRow, startCol));
        visited[startRow, startCol] = true;

        // Duyệt theo kiểu BFS
        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();

            // Thêm ô hiện tại vào kết quả
            result.Add(current);

            // Kiểm tra các ô hàng xóm
            foreach (Vector2Int neighbor in GetNeighbors(current.x, current.y))
            {
                int r = neighbor.x;
                int c = neighbor.y;

                if (!IsInside(r, c)) continue;
                if (visited[r, c]) continue;
                if (grid[r, c] == null) continue;

                // Chỉ lấy bóng cùng màu
                if (grid[r, c].colorId != targetColor) continue;

                visited[r, c] = true;
                queue.Enqueue(neighbor);
            }
        }

        return result;
    }

    // Tìm các bóng bị rời khỏi trần
    // Những bóng này không còn nối với hàng trên cùng nên sẽ rơi xuống
    public List<Vector2Int> FindDisconnectedBubbles()
    {
        bool[,] connected = new bool[rows, columns];
        Queue<Vector2Int> queue = new Queue<Vector2Int>();

        // Lấy số cột của hàng đầu tiên
        int topColCount = GetColumnCountForRow(0);

        // Những bóng ở hàng đầu tiên được xem là đang dính trần
        for (int c = 0; c < topColCount; c++)
        {
            if (grid[0, c] != null)
            {
                queue.Enqueue(new Vector2Int(0, c));
                connected[0, c] = true;
            }
        }

        // Từ hàng trên cùng, duyệt tất cả bóng đang nối với trần
        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();

            foreach (Vector2Int neighbor in GetNeighbors(current.x, current.y))
            {
                int r = neighbor.x;
                int c = neighbor.y;

                if (!IsInside(r, c)) continue;
                if (connected[r, c]) continue;
                if (grid[r, c] == null) continue;

                connected[r, c] = true;
                queue.Enqueue(neighbor);
            }
        }

        // Danh sách bóng không còn nối với trần
        List<Vector2Int> disconnected = new List<Vector2Int>();

        for (int r = 0; r < rows; r++)
        {
            int colCount = GetColumnCountForRow(r);

            for (int c = 0; c < colCount; c++)
            {
                // Có bóng nhưng không được đánh dấu connected
                // nghĩa là bóng bị lơ lửng, cần rơi xuống
                if (grid[r, c] != null && !connected[r, c])
                {
                    disconnected.Add(new Vector2Int(r, c));
                }
            }
        }

        return disconnected;
    }

    // Xóa danh sách bóng khỏi lưới
    public void RemoveBubbles(List<Vector2Int> bubbles)
    {
        foreach (Vector2Int cell in bubbles)
        {
            int r = cell.x;
            int c = cell.y;

            if (!IsInside(r, c)) continue;
            if (grid[r, c] == null) continue;

            Bubble bubble = grid[r, c];

            // Tạo hiệu ứng nổ bóng nếu có prefab hiệu ứng
            if (popEffectPrefab != null)
            {
                Instantiate(popEffectPrefab, bubble.transform.position, Quaternion.identity);
            }

            // Xóa GameObject bóng
            Destroy(bubble.gameObject);

            // Xóa dữ liệu bóng khỏi mảng grid
            grid[r, c] = null;
        }
    }

    // Làm cho các bóng bị mất liên kết rơi xuống
    public void DropBubbles(List<Vector2Int> bubbles)
    {
        foreach (Vector2Int cell in bubbles)
        {
            int r = cell.x;
            int c = cell.y;

            if (!IsInside(r, c)) continue;
            if (grid[r, c] == null) continue;

            Bubble bubble = grid[r, c];

            // Xóa bóng khỏi lưới trước
            grid[r, c] = null;

            // Chuyển bóng sang trạng thái rơi
            bubble.SetAsFallingBubble();

            // Sau 2 giây thì xóa bóng khỏi Scene
            Destroy(bubble.gameObject, 2f);
        }
    }

    // Thêm một hàng bóng mới ở trên cùng
    // Đồng thời đẩy toàn bộ bóng cũ xuống một hàng
    public bool AddNewTopRow(int level)
    {
        // Kiểm tra hàng cuối cùng
        // Nếu hàng cuối đã có bóng thì không thể đẩy xuống nữa
        // Trường hợp này thường dùng để xử lý thua game
        int bottomColCount = GetColumnCountForRow(rows - 1);

        for (int c = 0; c < bottomColCount; c++)
        {
            if (grid[rows - 1, c] != null)
            {
                return false;
            }
        }

        // Tạo lưới mới
        Bubble[,] newGrid = new Bubble[rows, columns];

        // Duyệt từ gần cuối lên trên
        // Đẩy mỗi bóng xuống một hàng
        for (int r = rows - 2; r >= 0; r--)
        {
            int colCount = GetColumnCountForRow(r);

            for (int c = 0; c < colCount; c++)
            {
                Bubble bubble = grid[r, c];

                if (bubble == null) continue;

                int newRow = r + 1;

                if (IsInside(newRow, c))
                {
                    // Đưa bóng xuống hàng mới
                    newGrid[newRow, c] = bubble;

                    // Cập nhật vị trí thật trong Scene
                    bubble.transform.position = GetWorldPosition(newRow, c);
                }
                else
                {
                    // Nếu vượt khỏi lưới thì xóa
                    Destroy(bubble.gameObject);
                }
            }
        }

        // Thay lưới cũ bằng lưới mới
        grid = newGrid;

        // Sinh hàng bóng mới ở trên cùng
        int topColCount = GetColumnCountForRow(0);

        for (int c = 0; c < topColCount; c++)
        {
            int colorId = Random.Range(0, bubbleSprites.Length);
            SpawnBubble(0, c, colorId);
        }

        return true;
    }

    // Kiểm tra xem người chơi đã phá hết bóng chưa
    public bool IsCleared()
    {
        for (int r = 0; r < rows; r++)
        {
            int colCount = GetColumnCountForRow(r);

            for (int c = 0; c < colCount; c++)
            {
                // Chỉ cần còn một bóng là chưa qua màn
                if (grid[r, c] != null)
                {
                    return false;
                }
            }
        }

        // Không còn bóng nào trên lưới
        return true;
    }

    // Kiểm tra một ô có bóng hàng xóm xung quanh không
    private bool HasAnyNeighbor(int row, int col)
    {
        foreach (Vector2Int neighbor in GetNeighbors(row, col))
        {
            if (IsInside(neighbor.x, neighbor.y) && grid[neighbor.x, neighbor.y] != null)
            {
                return true;
            }
        }

        return false;
    }

    // Lấy danh sách 6 ô hàng xóm xung quanh một ô
    // Vì Bubble Shooter dùng lưới kiểu tổ ong nên mỗi ô có tối đa 6 hàng xóm
    private List<Vector2Int> GetNeighbors(int row, int col)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();

        // Hàng chẵn
        if (row % 2 == 0)
        {
            neighbors.Add(new Vector2Int(row, col - 1));      // trái
            neighbors.Add(new Vector2Int(row, col + 1));      // phải

            neighbors.Add(new Vector2Int(row - 1, col - 1));  // trên trái
            neighbors.Add(new Vector2Int(row - 1, col));      // trên phải

            neighbors.Add(new Vector2Int(row + 1, col - 1));  // dưới trái
            neighbors.Add(new Vector2Int(row + 1, col));      // dưới phải
        }
        // Hàng lẻ
        else
        {
            neighbors.Add(new Vector2Int(row, col - 1));      // trái
            neighbors.Add(new Vector2Int(row, col + 1));      // phải

            neighbors.Add(new Vector2Int(row - 1, col));      // trên trái
            neighbors.Add(new Vector2Int(row - 1, col + 1));  // trên phải

            neighbors.Add(new Vector2Int(row + 1, col));      // dưới trái
            neighbors.Add(new Vector2Int(row + 1, col + 1));  // dưới phải
        }

        return neighbors;
    }

    // Lấy số cột thật sự của một hàng
    private int GetColumnCountForRow(int row)
    {
        // Nếu không tự căn theo tường thì hàng nào cũng đủ columns
        if (!autoFitToWalls)
        {
            return columns;
        }

        // Khi tự căn theo tường:
        // Hàng lẻ bị lệch nửa ô sang phải
        // nên giảm 1 bóng để không bị tràn ra ngoài tường
        if (row % 2 == 1)
        {
            return Mathf.Max(1, columns - 1);
        }

        return columns;
    }

    // Kiểm tra ô row, col có nằm trong lưới không
    private bool IsInside(int row, int col)
    {
        if (row < 0 || row >= rows) return false;
        if (col < 0 || col >= GetColumnCountForRow(row)) return false;

        return true;
    }

    // Xóa toàn bộ bóng đang có trong lưới
    private void ClearGrid()
    {
        // Nếu grid chưa có hoặc kích thước grid không đúng thì tạo lại
        if (grid == null || grid.GetLength(0) != rows || grid.GetLength(1) != columns)
        {
            grid = new Bubble[rows, columns];
        }

        // Duyệt toàn bộ lưới và xóa bóng
        for (int r = 0; r < grid.GetLength(0); r++)
        {
            for (int c = 0; c < grid.GetLength(1); c++)
            {
                if (grid[r, c] != null)
                {
                    Destroy(grid[r, c].gameObject);
                    grid[r, c] = null;
                }
            }
        }
    }
}
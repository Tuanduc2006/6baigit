using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

// Script điều khiển súng bắn bóng
// Bao gồm: ngắm, bắn, tạo bóng, xử lý bóng dính vào lưới
public class Shooter : MonoBehaviour
{
    [Header("Vị trí súng")]
    // Điểm xoay của súng
    // Khi người chơi kéo chuột/tay, súng sẽ xoay quanh điểm này
    public Transform cannonPivot;

    // Vị trí đầu nòng súng
    // Bóng sẽ nằm ở đây trước khi bắn
    public Transform muzzlePoint;

    [Header("Bubble")]
    // Prefab quả bóng dùng để tạo bóng bắn
    public Bubble bubblePrefab;

    // Mảng sprite của các quả bóng
    // FormerlySerializedAs giúp giữ dữ liệu cũ nếu trước đây biến tên là bubbleSprites
    [FormerlySerializedAs("bubbleSprites")]
    public Sprite[] ballSprites;

    // Vật liệu nảy dùng cho bóng khi chạm tường
    public PhysicsMaterial2D bounceMaterial;

    [Header("Liên kết hệ thống")]
    // Script quản lý lưới bóng
    public BubbleGrid grid;

    // Script vẽ đường ngắm
    public TrajectoryLine trajectoryLine;

    // Script quản lý game: điểm, thắng, thua, lượt bắn
    public GameManager gameManager;

    [Header("UI")]
    // SpriteRenderer hiển thị bóng tiếp theo ngoài scene
    public SpriteRenderer nextBubblePreview;

    // UI Controller dùng để gửi sprite bóng tiếp theo lên UI Image
    public MonoBehaviour uiController;

    [Header("Thông số bắn")]
    // Tốc độ bay của bóng sau khi bắn
    public float shootSpeed = 12f;

    // Góc ngắm nhỏ nhất
    public float minAimAngle = 0f;

    // Góc ngắm lớn nhất
    public float maxAimAngle = 155f;

    [Header("Âm thanh")]
    // AudioSource phát âm thanh bắn
    public AudioSource audioSource;

    // Âm thanh khi bắn bóng
    public AudioClip shootClip;

    // Camera chính dùng để đổi vị trí chuột từ màn hình sang thế giới game
    private Camera mainCamera;

    // Quả bóng hiện tại đang nằm trên súng
    private Bubble currentBubble;

    // Mã màu của bóng hiện tại
    private int currentColorId;

    // Mã màu của bóng tiếp theo
    private int nextColorId;

    // Kiểm tra có được bắn hay không
    private bool canShoot = true;

    // Kiểm tra phát bắn hiện tại đã xử lý xong chưa
    // Dùng để tránh bắn tiếp khi bóng cũ chưa dính/xóa xong
    private bool isResolvingShot = false;

    private void Awake()
    {
        // Lấy camera chính
        mainCamera = Camera.main;

        // Tự tìm các object còn thiếu nếu chưa kéo vào Inspector
        FindMissingReferences();

        // Nếu Shooter chưa có mảng sprite thì lấy từ BubbleGrid
        SetupBallSprites();
    }

    private void Start()
    {
        // Reset súng và tạo bóng đầu tiên
        ResetShooter();
    }

    private void Update()
    {
        // Nếu game đã kết thúc thì ẩn đường ngắm và không cho điều khiển nữa
        if (gameManager != null && gameManager.IsGameEnded)
        {
            if (trajectoryLine != null)
            {
                trajectoryLine.Hide();
            }

            return;
        }

        // Nếu đang có bóng trên súng thì luôn giữ bóng ở đầu nòng súng
        if (currentBubble != null && muzzlePoint != null)
        {
            currentBubble.transform.position = muzzlePoint.position;
        }

        // Lấy vị trí chuột hoặc tay chạm
        Vector2 pointerPosition;

        // Kiểm tra người chơi có đang giữ chuột/tay không
        bool isHolding = IsPointerHolding(out pointerPosition);

        if (isHolding)
        {
            // Xoay súng theo vị trí người chơi đang giữ
            Aim(pointerPosition);

            // Hiện đường ngắm
            if (trajectoryLine != null && muzzlePoint != null)
            {
                trajectoryLine.Show(muzzlePoint.position, GetAimDirection());
            }
        }
        else
        {
            // Nếu không giữ chuột/tay thì ẩn đường ngắm
            if (trajectoryLine != null)
            {
                trajectoryLine.Hide();
            }
        }

        // Khi thả chuột/tay thì bắn bóng
        if (canShoot && !isResolvingShot && currentBubble != null && IsPointerReleased())
        {
            Shoot();
        }
    }

    // Reset lại súng khi bắt đầu game hoặc bắt đầu level mới
    public void ResetShooter()
    {
        // Tự tìm lại các object nếu bị thiếu
        FindMissingReferences();

        // Lấy sprite bóng từ BubbleGrid nếu Shooter chưa có
        SetupBallSprites();

        // Nếu chưa có sprite bóng thì báo lỗi
        if (!HasEnoughSprites())
        {
            Debug.LogError("Chưa kéo đủ sprite bóng vào Shooter hoặc BubbleGrid.");
            return;
        }

        // Nếu đang có bóng cũ trên súng thì xóa
        if (currentBubble != null)
        {
            Destroy(currentBubble.gameObject);
            currentBubble = null;
        }

        // Cho phép bắn lại
        canShoot = true;

        // Đánh dấu không còn phát bắn nào đang xử lý
        isResolvingShot = false;

        // Chọn màu bóng tiếp theo ngẫu nhiên
        nextColorId = RandomColorId();

        // Tạo bóng đang nằm trên súng
        SpawnLoadedBubble();

        // Cập nhật UI bóng tiếp theo
        UpdateNextBallUI();
    }

    // Tạo quả bóng đang nằm ở đầu nòng súng
    private void SpawnLoadedBubble()
    {
        // Nếu chưa gắn prefab bóng thì báo lỗi
        if (bubblePrefab == null)
        {
            Debug.LogError("Shooter chưa được gắn Bubble Prefab.");
            return;
        }

        // Nếu chưa gắn điểm đầu nòng súng thì báo lỗi
        if (muzzlePoint == null)
        {
            Debug.LogError("Shooter chưa được gắn Muzzle Point.");
            return;
        }

        // Bóng hiện tại lấy màu của bóng tiếp theo
        currentColorId = nextColorId;

        // Random màu mới cho bóng tiếp theo
        nextColorId = RandomColorId();

        // Tạo bóng tại vị trí đầu nòng súng
        currentBubble = Instantiate(bubblePrefab, muzzlePoint.position, Quaternion.identity);

        // Gán màu và sprite cho bóng hiện tại
        currentBubble.Setup(currentColorId, ballSprites[currentColorId]);

        // Đặt bóng thành trạng thái đang nằm trên súng
        currentBubble.SetAsLoadedBubble();

        // Cho phép bắn
        canShoot = true;

        // Cập nhật hình ảnh bóng tiếp theo
        UpdateNextBallUI();
    }

    // Bắn bóng ra khỏi súng
    private void Shoot()
    {
        // Nếu không có bóng thì không làm gì
        if (currentBubble == null) return;

        // Không cho bắn tiếp trong lúc bóng đang bay
        canShoot = false;

        // Đánh dấu đang xử lý phát bắn
        isResolvingShot = true;

        // Chuyển bóng sang trạng thái đang bay
        currentBubble.SetAsShotBubble(bounceMaterial);

        // Lấy script BubbleProjectile để phát hiện va chạm
        BubbleProjectile projectile = currentBubble.GetComponent<BubbleProjectile>();

        // Nếu bóng chưa có BubbleProjectile thì tự thêm vào
        if (projectile == null)
        {
            projectile = currentBubble.gameObject.AddComponent<BubbleProjectile>();
        }

        // Khởi tạo BubbleProjectile và truyền Shooter hiện tại vào
        projectile.Init(this);

        // Gán vận tốc cho bóng bay theo hướng súng đang ngắm
        currentBubble.rb.linearVelocity = GetAimDirection() * shootSpeed;

        // Phát âm thanh bắn nếu có
        if (audioSource != null && shootClip != null)
        {
            audioSource.PlayOneShot(shootClip);
        }

        // Báo GameManager trừ một lượt bắn
        if (gameManager != null)
        {
            gameManager.UseShot();
        }

        // Xóa tham chiếu bóng hiện tại
        // Vì bóng đã bay ra khỏi súng rồi
        currentBubble = null;

        // Ẩn đường ngắm
        if (trajectoryLine != null)
        {
            trajectoryLine.Hide();
        }
    }

    // Hàm được BubbleProjectile gọi khi bóng bắn chạm bóng khác hoặc tường trên
    public void AttachShotBubble(Bubble bubble)
    {
        if (bubble == null) return;

        // Dùng Coroutine để xử lý gắn bóng, xóa bóng, rơi bóng theo từng bước
        StartCoroutine(AttachRoutine(bubble));
    }

    // Xử lý bóng sau khi chạm và cần gắn vào lưới
    private IEnumerator AttachRoutine(Bubble bubble)
    {
        // Nếu chưa gắn BubbleGrid thì xóa bóng và dừng
        if (grid == null)
        {
            Debug.LogError("Shooter chưa được gắn BubbleGrid.");
            Destroy(bubble.gameObject);
            yield break;
        }

        // Dừng chuyển động của bóng
        bubble.rb.linearVelocity = Vector2.zero;
        bubble.rb.angularVelocity = 0;

        // Tìm ô trống gần nhất để gắn bóng vào lưới
        Vector2Int cell = grid.GetNearestEmptyCell(bubble.transform.position);

        // Nếu không tìm được ô phù hợp thì xóa bóng và thua game
        if (cell.x < 0 || cell.y < 0)
        {
            Destroy(bubble.gameObject);

            if (gameManager != null)
            {
                gameManager.GameOver();
            }

            yield break;
        }

        // Gắn bóng vào ô tìm được trong lưới
        grid.SetBubble(cell.x, cell.y, bubble);

        // Sau khi bóng đã dính vào lưới thì tắt BubbleProjectile
        BubbleProjectile projectile = bubble.GetComponent<BubbleProjectile>();

        if (projectile != null)
        {
            projectile.enabled = false;
        }

        // Chờ rất ngắn để bóng ổn định vị trí
        yield return new WaitForSeconds(0.05f);

        // Biến kiểm tra phát bắn này có xóa được bóng không
        bool removedSomething = false;

        // Số bóng cùng màu bị xóa
        int removedCount = 0;

        // Số bóng bị rơi xuống
        int droppedCount = 0;

        // Tìm các bóng cùng màu nối với bóng vừa bắn
        var matches = grid.FindMatches(cell.x, cell.y);

        // Nếu có từ 3 bóng cùng màu trở lên thì xóa
        if (matches.Count >= 3)
        {
            removedSomething = true;
            removedCount = matches.Count;

            // Xóa cụm bóng cùng màu
            grid.RemoveBubbles(matches);

            // Chờ một chút để hiệu ứng xóa bóng hiển thị
            yield return new WaitForSeconds(0.15f);

            // Tìm các bóng không còn nối với trần
            var disconnected = grid.FindDisconnectedBubbles();

            // Đếm số bóng bị rơi
            droppedCount = disconnected.Count;

            // Cho các bóng bị rời khỏi trần rơi xuống
            grid.DropBubbles(disconnected);
        }

        // Báo kết quả phát bắn cho GameManager
        if (gameManager != null)
        {
            if (removedSomething)
            {
                // Cộng điểm theo số bóng bị xóa và bị rơi
                gameManager.AddScore(removedCount, droppedCount);
            }
            else
            {
                // Nếu không xóa được gì thì reset combo
                gameManager.ResetCombo();
            }

            // Kiểm tra thắng/thua/hết lượt sau phát bắn
            gameManager.OnShotResolved(removedSomething);
        }

        // Chờ ngắn trước khi tạo bóng mới
        yield return new WaitForSeconds(0.1f);

        // Đánh dấu đã xử lý xong phát bắn
        isResolvingShot = false;

        // Nếu game chưa kết thúc thì tạo bóng tiếp theo
        if (gameManager == null || !gameManager.IsGameEnded)
        {
            SpawnLoadedBubble();
        }
    }

    // Xoay súng theo vị trí chuột hoặc tay chạm
    private void Aim(Vector2 screenPosition)
    {
        // Nếu chưa có camera thì lấy lại camera chính
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        // Nếu thiếu camera hoặc thiếu điểm xoay súng thì không xử lý
        if (mainCamera == null || cannonPivot == null) return;

        // Đổi vị trí từ màn hình sang tọa độ thế giới
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(screenPosition);

        // Đặt z = 0 vì game 2D dùng mặt phẳng XY
        worldPosition.z = 0;

        // Tính hướng từ súng đến vị trí người chơi trỏ vào
        Vector2 direction = worldPosition - cannonPivot.position;

        // Nếu khoảng cách quá nhỏ thì bỏ qua
        if (direction.sqrMagnitude < 0.01f) return;

        // Tính góc xoay từ vector direction
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Giới hạn góc xoay để súng không bắn xuống dưới hoặc xoay quá mức
        angle = Mathf.Clamp(angle, minAimAngle, maxAimAngle);

        // Xoay súng
        // Trừ 90 vì sprite súng thường hướng lên trên thay vì hướng sang phải
        cannonPivot.localRotation = Quaternion.Euler(0, 0, angle - 90f);
    }

    // Lấy hướng bắn hiện tại của súng
    private Vector2 GetAimDirection()
    {
        // Nếu thiếu muzzlePoint hoặc cannonPivot thì mặc định bắn lên trên
        if (muzzlePoint == null || cannonPivot == null)
        {
            return Vector2.up;
        }

        // Hướng bắn là hướng từ điểm xoay súng đến đầu nòng súng
        return (muzzlePoint.position - cannonPivot.position).normalized;
    }

    // Cập nhật hình ảnh bóng tiếp theo
    private void UpdateNextBallUI()
    {
        // Nếu chưa có sprite bóng thì không làm gì
        if (ballSprites == null || ballSprites.Length == 0)
        {
            return;
        }

        // Nếu nextColorId bị sai thì đưa về 0 để tránh lỗi vượt mảng
        if (nextColorId < 0 || nextColorId >= ballSprites.Length)
        {
            nextColorId = 0;
        }

        // Lấy sprite của bóng tiếp theo
        Sprite nextSprite = ballSprites[nextColorId];

        // Cập nhật preview bóng tiếp theo bằng SpriteRenderer nếu có
        if (nextBubblePreview != null)
        {
            nextBubblePreview.sprite = nextSprite;
            nextBubblePreview.enabled = nextSprite != null;
        }

        // Gửi sprite sang UI Controller
        // UI Controller có thể có hàm SetNextBallSprite(Sprite)
        if (uiController != null)
        {
            uiController.SendMessage(
                "SetNextBallSprite",
                nextSprite,
                SendMessageOptions.DontRequireReceiver
            );
        }
    }

    // Random mã màu bóng
    private int RandomColorId()
    {
        // Nếu không có sprite thì trả về 0
        if (ballSprites == null || ballSprites.Length == 0)
        {
            return 0;
        }

        // Random từ 0 đến số lượng sprite - 1
        return Random.Range(0, ballSprites.Length);
    }

    // Kiểm tra có đủ sprite bóng hay chưa
    private bool HasEnoughSprites()
    {
        return ballSprites != null && ballSprites.Length > 0;
    }

    // Nếu Shooter chưa có sprite bóng thì lấy từ BubbleGrid
    private void SetupBallSprites()
    {
        if ((ballSprites == null || ballSprites.Length == 0) && grid != null)
        {
            ballSprites = grid.bubbleSprites;
        }
    }

    // Tự động tìm các object còn thiếu
    private void FindMissingReferences()
    {
        // Tìm camera chính
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        // Tìm BubbleGrid trong Scene
        if (grid == null)
        {
            grid = FindObjectOfType<BubbleGrid>();
        }

        // Tìm GameManager trong Scene
        if (gameManager == null)
        {
            gameManager = FindObjectOfType<GameManager>();
        }

        // Tìm TrajectoryLine trong Scene
        if (trajectoryLine == null)
        {
            trajectoryLine = FindObjectOfType<TrajectoryLine>();
        }

        // Lấy AudioSource trên chính object Shooter
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        // Nếu chưa gắn Bubble Prefab thì lấy prefab từ BubbleGrid
        if (bubblePrefab == null && grid != null)
        {
            bubblePrefab = grid.bubblePrefab;
        }
    }

    // Kiểm tra người chơi có đang giữ chuột hoặc giữ tay trên màn hình không
    private bool IsPointerHolding(out Vector2 position)
    {
        // Giá trị mặc định
        position = Vector2.zero;

        // Nếu đang chạm màn hình trên điện thoại
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            // Lấy vị trí chạm
            position = touch.position;

            // Trả về true nếu tay chưa nhấc lên và chưa bị hủy thao tác
            return touch.phase != TouchPhase.Ended &&
                   touch.phase != TouchPhase.Canceled;
        }

        // Nếu đang giữ chuột trái trên máy tính
        if (Input.GetMouseButton(0))
        {
            position = Input.mousePosition;
            return true;
        }

        // Không giữ chuột/tay
        return false;
    }

    // Kiểm tra người chơi vừa thả chuột hoặc nhấc tay khỏi màn hình chưa
    private bool IsPointerReleased()
    {
        // Kiểm tra trên điện thoại
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            // Nếu ngón tay vừa nhấc lên thì bắn
            return touch.phase == TouchPhase.Ended;
        }

        // Kiểm tra trên máy tính
        // Khi thả chuột trái thì bắn
        return Input.GetMouseButtonUp(0);
    }
}
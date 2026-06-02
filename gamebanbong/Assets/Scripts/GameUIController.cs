using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Script này dùng để điều khiển giao diện game
// Bao gồm: điểm, level, số bóng còn lại và hình ảnh bóng tiếp theo
public class GameUIController : MonoBehaviour
{
    [Header("Text điểm / level / số bóng")]

    // Text hiển thị điểm
    [SerializeField] private TMP_Text txtScore;

    // Text hiển thị level hiện tại
    [SerializeField] private TMP_Text txtLevel;

    // Text hiển thị số bóng hoặc số lượt bắn còn lại
    [SerializeField] private TMP_Text txtBallCount;

    [Header("Ảnh bóng tiếp theo trong bảng MÀU")]

    // Image hiển thị quả bóng tiếp theo sẽ được bắn
    [SerializeField] private Image imgNextBall;

    // Điểm hiện tại của người chơi
    private int score = 0;

    // Level hiện tại
    private int level = 1;

    // Số bóng ban đầu
    private int ballCount = 30;

    private void Start()
    {
        // Nếu trong Scene không có GameManager
        // thì script này sẽ tự cập nhật UI bằng giá trị mặc định
        //
        // Trường hợp này dùng để test riêng giao diện UI
        // Ví dụ: chỉ mở Scene UI mà chưa có hệ thống game chính
        if (FindFirstObjectByType<GameManager>() == null)
        {
            UpdateAllUI();
        }

        // Nếu có GameManager thì GameManager sẽ tự cập nhật UI
        // nên script này không cần cập nhật lúc Start
    }

    // Gán điểm bằng một giá trị cụ thể
    public void SetScore(int value)
    {
        // Lưu điểm mới
        score = value;

        // Nếu đã gắn Text điểm thì cập nhật lên màn hình
        if (txtScore != null)
        {
            txtScore.text = score.ToString();
        }
    }

    // Cộng thêm điểm
    public void AddScore(int value)
    {
        // Cộng điểm hiện tại với giá trị truyền vào
        score += value;

        // Cập nhật Text điểm nếu có
        if (txtScore != null)
        {
            txtScore.text = score.ToString();
        }
    }

    // Gán level bằng một giá trị cụ thể
    public void SetLevel(int value)
    {
        // Lưu level mới
        level = value;

        // Cập nhật Text level nếu có
        if (txtLevel != null)
        {
            txtLevel.text = level.ToString();
        }
    }

    // Tăng level lên 1
    public void NextLevel()
    {
        // Tăng level
        level++;

        // Cập nhật Text level nếu có
        if (txtLevel != null)
        {
            txtLevel.text = level.ToString();
        }
    }

    // Gán số bóng còn lại
    public void SetBallCount(int value)
    {
        // Mathf.Max(0, value) giúp số bóng không bị âm
        // Ví dụ nếu value = -1 thì ballCount vẫn bằng 0
        ballCount = Mathf.Max(0, value);

        // Cập nhật Text số bóng nếu có
        if (txtBallCount != null)
        {
            txtBallCount.text = ballCount.ToString();
        }
    }

    // Trừ đi 1 quả bóng sau mỗi lần bắn
    public void UseOneBall()
    {
        // Giảm số bóng đi 1
        SetBallCount(ballCount - 1);
    }

    // Lấy số bóng hiện tại
    public int GetBallCount()
    {
        return ballCount;
    }

    // Cập nhật hình ảnh quả bóng tiếp theo
    public void SetNextBallSprite(Sprite nextBallSprite)
    {
        // Nếu chưa gắn Image hoặc sprite truyền vào bị null thì không làm gì
        if (imgNextBall == null || nextBallSprite == null)
        {
            return;
        }

        // Gán sprite bóng tiếp theo vào Image UI
        imgNextBall.sprite = nextBallSprite;

        // Đặt màu trắng để sprite hiển thị đúng màu gốc
        // Nếu Image bị màu trong suốt hoặc bị đổi màu thì dòng này sẽ sửa lại
        imgNextBall.color = Color.white;

        // Giữ đúng tỉ lệ ảnh, tránh bị méo hình
        imgNextBall.preserveAspect = true;
    }

    // Cập nhật toàn bộ UI cùng lúc
    private void UpdateAllUI()
    {
        // Cập nhật điểm
        SetScore(score);

        // Cập nhật level
        SetLevel(level);

        // Cập nhật số bóng còn lại
        SetBallCount(ballCount);
    }

    // Reset lại toàn bộ UI về giá trị ban đầu
    public void ResetUI()
    {
        // Điểm về 0
        score = 0;

        // Level về 1
        level = 1;

        // Số bóng về 30
        ballCount = 30;

        // Cập nhật lại toàn bộ giao diện
        UpdateAllUI();
    }
}
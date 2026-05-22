using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DayNightCycle : MonoBehaviour
{
    [Header("Cài đặt Màu sắc")]
    public Color dayColor = Color.white; // Màu ban ngày (Trắng)
    public Color nightColor = new Color(0.2f, 0.2f, 0.5f); // Màu đêm (Xanh thẫm)

    [Header("Cài đặt Thời gian (Giây)")]
    public float dayHoldTime = 30f;    // Thời gian ĐỨNG YÊN ở trạng thái Ban Ngày
    public float nightHoldTime = 30f;  // Thời gian ĐỨNG YÊN ở trạng thái Ban Đêm
    public float transitionTime = 5f;  // Thời gian chuyển giao (Đổi màu từ từ mất bao lâu)

    private Light2D globalLight;
    private float timer;

    // Các trạng thái của hệ thống
    private enum TimeState { Day, Sunset, Night, Sunrise }
    private TimeState currentState;

    void Start()
    {
        globalLight = GetComponent<Light2D>();

        // Mới vào game là Ban ngày
        currentState = TimeState.Day;
        globalLight.color = dayColor;
        timer = dayHoldTime; // Bắt đầu đếm ngược 30 giây của ban ngày
    }

    void Update()
    {
        if (globalLight == null) return;

        // Trừ lùi thời gian
        timer -= Time.deltaTime;

        // Xử lý logic tùy theo trạng thái hiện tại
        switch (currentState)
        {
            case TimeState.Day:
                // Hết 30 giây ban ngày -> Bắt đầu sang Hoàng Hôn
                if (timer <= 0)
                {
                    currentState = TimeState.Sunset;
                    timer = transitionTime; // Đặt lại đồng hồ để đếm thời gian chuyển màu
                }
                break;

            case TimeState.Sunset:
                // Tính phần trăm chuyển màu từ Ngày -> Đêm
                float sunsetPercent = 1f - (timer / transitionTime);
                globalLight.color = Color.Lerp(dayColor, nightColor, sunsetPercent);

                // Hết thời gian hoàng hôn -> Chính thức sang Đêm
                if (timer <= 0)
                {
                    currentState = TimeState.Night;
                    globalLight.color = nightColor; // Chốt lại màu chuẩn
                    timer = nightHoldTime; // Bắt đầu đếm ngược 30 giây ban đêm
                }
                break;

            case TimeState.Night:
                // Hết 30 giây ban đêm -> Bắt đầu sang Bình Minh
                if (timer <= 0)
                {
                    currentState = TimeState.Sunrise;
                    timer = transitionTime;
                }
                break;

            case TimeState.Sunrise:
                // Tính phần trăm chuyển màu từ Đêm -> Ngày
                float sunrisePercent = 1f - (timer / transitionTime);
                globalLight.color = Color.Lerp(nightColor, dayColor, sunrisePercent);

                // Hết thời gian bình minh -> Chính thức quay lại Ban Ngày
                if (timer <= 0)
                {
                    currentState = TimeState.Day;
                    globalLight.color = dayColor;
                    timer = dayHoldTime;
                }
                break;
        }
    }
}

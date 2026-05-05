using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Card : MonoBehaviour
{
    public int cardID;
    public Image cardImage;
    public Sprite backSprite;
    private Sprite frontSprite;

    private Button button;

    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(() => GameManager.Instance.CardClicked(this));
    }

    public void SetupCard(CardData data)
    {
        cardID = data.cardID;
        frontSprite = data.frontSprite;
        cardImage.sprite = backSprite;

        // Đảm bảo thẻ luôn sáng rõ 100% (Alpha = 1) lúc bắt đầu ván mới
        // (Phòng trường hợp Unity nhớ nhầm trạng thái mờ của ván trước)
        Color c = cardImage.color;
        c.a = 1f;
        cardImage.color = c;
    }

    public void FlipUp()
    {
        button.interactable = false;
        transform.DORotate(new Vector3(0, 90, 0), 0.15f).OnComplete(() =>
        {
            cardImage.sprite = frontSprite;
            transform.DORotate(new Vector3(0, 0, 0), 0.15f);
        });
    }

    public void FlipDown()
    {
        transform.DORotate(new Vector3(0, 90, 0), 0.15f).OnComplete(() =>
        {
            cardImage.sprite = backSprite;
            transform.DORotate(new Vector3(0, 0, 0), 0.15f).OnComplete(() =>
            {
                button.interactable = true;
            });
        });
    }

    public void FadeOut()
    {
        // Chỉnh Alpha về 0.5 (Mờ đi 50%) trong vòng 0.5 giây
        // KHÔNG dùng lệnh SetActive(false) nữa để thẻ được giữ nguyên trên bảng
        cardImage.DOFade(0.5f, 0.5f);
    }
}
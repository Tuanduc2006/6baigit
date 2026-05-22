using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject menuPanel;
    public GameObject gamePanel;

    [Header("Game Settings")]
    public LevelConfig[] allLevels;
    public GameObject cardPrefab;
    public Transform cardBoard;
    public List<Sprite> cardSprites;

    private LevelConfig currentLevel;
    private List<CardController> cardsInGame = new List<CardController>();
    private CardController firstCard;
    private CardController secondCard;
    private bool canClick = true;

    // Biến mới để đếm số cặp thẻ đã ghép đúng
    private int matchesFound = 0;

    void Start()
    {
        menuPanel.SetActive(true);
        gamePanel.SetActive(false);
    }

    public void StartLevel(int levelIndex)
    {
        currentLevel = allLevels[levelIndex];
        matchesFound = 0; // Reset lại số điểm/số cặp mỗi khi bắt đầu màn mới

        menuPanel.SetActive(false);
        gamePanel.SetActive(true);

        GenerateBoard();
    }

    void GenerateBoard()
    {
        foreach (Transform child in cardBoard)
        {
            Destroy(child.gameObject);
        }
        cardsInGame.Clear();

        int totalCards = currentLevel.rows * currentLevel.columns;
        List<int> cardIDs = new List<int>();

        for (int i = 0; i < totalCards / 2; i++)
        {
            cardIDs.Add(i);
            cardIDs.Add(i);
        }

        for (int i = 0; i < cardIDs.Count; i++)
        {
            int temp = cardIDs[i];
            int randomIndex = Random.Range(i, cardIDs.Count);
            cardIDs[i] = cardIDs[randomIndex];
            cardIDs[randomIndex] = temp;
        }

        for (int i = 0; i < totalCards; i++)
        {
            GameObject newCardObj = Instantiate(cardPrefab, cardBoard);
            CardController card = newCardObj.GetComponent<CardController>();

            int id = cardIDs[i];
            card.Setup(id, cardSprites[id], OnCardSelected);
            cardsInGame.Add(card);
        }
    }

    public void OnCardSelected(CardController selectedCard)
    {
        if (!canClick) return;

        selectedCard.FlipToFront();

        if (firstCard == null)
        {
            firstCard = selectedCard;
        }
        else
        {
            secondCard = selectedCard;
            canClick = false;
            StartCoroutine(CheckMatch());
        }
    }

    IEnumerator CheckMatch()
    {
        yield return new WaitForSeconds(0.6f);

        if (firstCard.GetCardID() == secondCard.GetCardID())
        {
            firstCard.SetMatched();
            secondCard.SetMatched();

            matchesFound++; // Tăng biến đếm lên 1 khi ghép đúng

            // Kiểm tra xem đã thắng chưa (Số cặp ghép được = Tổng số bài / 2)
            int totalPairs = (currentLevel.rows * currentLevel.columns) / 2;
            if (matchesFound >= totalPairs)
            {
                StartCoroutine(LevelComplete());
            }
        }
        else
        {
            firstCard.FlipToBack();
            secondCard.FlipToBack();
        }

        firstCard = null;
        secondCard = null;
        canClick = true;
    }

    // Hàm mới xử lý khi hoàn thành màn chơi
    IEnumerator LevelComplete()
    {
        // Đợi 1.5 giây để người chơi nhìn thấy cặp thẻ cuối cùng mờ đi
        yield return new WaitForSeconds(1.5f);

        // Ẩn màn chơi và hiện lại Menu
        gamePanel.SetActive(false);
        menuPanel.SetActive(true);
    }
}
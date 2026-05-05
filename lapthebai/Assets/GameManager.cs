using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject cardPrefab;
    public Transform gameBoard;
    public List<CardData> cardDataList;

    private List<CardData> deck = new List<CardData>();

    private Card firstCard;
    private Card secondCard;
    private bool isChecking;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        SetupGame();
    }

    void SetupGame()
    {
        deck.AddRange(cardDataList);
        deck.AddRange(cardDataList);

        for (int i = 0; i < deck.Count; i++)
        {
            CardData temp = deck[i];
            int randomIndex = Random.Range(i, deck.Count);
            deck[i] = deck[randomIndex];
            deck[randomIndex] = temp;
        }

        foreach (CardData data in deck)
        {
            GameObject newCard = Instantiate(cardPrefab, gameBoard);
            newCard.GetComponent<Card>().SetupCard(data);
        }
    }

    public void CardClicked(Card clickedCard)
    {
        if (isChecking || clickedCard == firstCard) return;

        clickedCard.FlipUp();

        if (firstCard == null)
        {
            firstCard = clickedCard;
        }
        else
        {
            secondCard = clickedCard;
            StartCoroutine(CheckMatch());
        }
    }

    IEnumerator CheckMatch()
    {
        isChecking = true;

        // Đã tăng thời gian chờ lên 1 giây để bạn nhìn rõ 2 thẻ
        yield return new WaitForSeconds(1.0f);

        if (firstCard.cardID == secondCard.cardID)
        {
            // GIỐNG NHAU: Làm mờ và biến mất
            firstCard.FadeOut();
            secondCard.FadeOut();
        }
        else
        {
            // KHÁC NHAU: Lật úp lại
            firstCard.FlipDown();
            secondCard.FlipDown();
        }

        firstCard = null;
        secondCard = null;
        isChecking = false;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DeckBuilderManager : MonoBehaviour
{
    [SerializeField] MusicianCard musicianCardPrefab;
    [SerializeField] InstrumentCard instrumentCardPrefab;

    GameObject hand; 

    // Start is called before the first frame update
    void Start()
    {
        hand = GameObject.Find("Hand");

        GameManager.CardLists cards = GameManager.Instance.Cards;
        List<GameManager.OwnedCardEntry> ownedInstrumentCards = GameManager.Instance.OwnedInstrumentCards;

        AddCardsToHandPanel(ownedInstrumentCards, cards.InstrumentCards, instrumentCardPrefab);
        AddCardsToHandPanel(GameManager.Instance.OwnedMusicanCards, cards.MusicianCards, musicianCardPrefab);

        ShowInfoPanel();
    }

    void AddCardsToHandPanel (List<GameManager.OwnedCardEntry> ownedCardList, List<GameManager.CardEntry> cardList, Card prefab)
    {
        foreach (GameManager.OwnedCardEntry ownedInstrumentCard in ownedCardList)
        {
            GameManager.CardEntry cardEntry = cardList.Find(x => x.CardName == ownedInstrumentCard.CardName);
            Card card = (Card)Instantiate<Card>(prefab, new Vector3(100, 100, 100), Quaternion.identity, hand.transform);

            card.Name = cardEntry.CardName;
            card.Description = cardEntry.CardDescription;
            card.SetImage("Guitar");

            card.CategoryValue1 = cardEntry.Category1Value;
            card.CategoryValue2 = cardEntry.Category2Value;
            card.CategoryValue3 = cardEntry.Category3Value;
            card.CategoryValue4 = cardEntry.Category4Value;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GoToMenuScene();
        }
    }

    public void GoToMenuScene()
    {
        SceneManager.LoadScene(0);
    }

    public void GoToGameScene()
    {
        CardPair[] cardpairs = GetComponentsInChildren<CardPair>();

        foreach(CardPair cardPair in cardpairs)
        {
            if (cardPair.GetComponentsInChildren<Card>().Length < 2)
            {
                ShowInfoPanel();
                return;
            }
        }

        SceneManager.LoadScene(2);
    }

    void ShowInfoPanel()
    {
        gameObject.transform.Find("InfoPanel").gameObject.SetActive(true);
    }
}

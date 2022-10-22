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
    CardPair[] cardpairs;

    List<string> instrumentCardsInPlay = new List<string>();
    List<string> musicianCardsInPlay = new List<string>();

    // Start is called before the first frame update
    void Start()
    {
        hand = GameObject.Find("Hand");
        cardpairs = GetComponentsInChildren<CardPair>();

        GameManager.CardLists cards = GameManager.Instance.Cards;
        List<GameManager.OwnedCardEntry> ownedInstrumentCards = GameManager.Instance.OwnedInstrumentCards;
        List<GameManager.OwnedCardEntry> ownedMuicianCards = GameManager.Instance.OwnedMusicanCards;

        AddCardsToHandPanels(ownedInstrumentCards, cards.InstrumentCards, instrumentCardPrefab);
        AddCardsToHandPanels(ownedMuicianCards, cards.MusicianCards, musicianCardPrefab);

        ShowInfoPanel(!CheckCardsInPlay());
    }

    void AddCardsToHandPanels (List<GameManager.OwnedCardEntry> ownedCardList, List<GameManager.CardEntry> cardList, Card prefab)
    {
        foreach (GameManager.OwnedCardEntry ownedCard in ownedCardList)
        {
            GameManager.CardEntry cardEntry = cardList.Find(x => x.CardName == ownedCard.CardName);

            GameObject parent = hand;

            if (ownedCard.IsInPlay)
            {
                parent = cardpairs[ownedCard.InPlayPairIndex - 1].gameObject;
            }

            Card card = (Card)Instantiate<Card>(prefab, new Vector3(100, 100, 100), Quaternion.identity, parent.transform);

            card.Name = cardEntry.CardName;
            card.Description = cardEntry.CardDescription;

            // TODO : Set Image to correct image once images have been created and added to resources
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

    protected bool CheckCardsInPlay()
    {
        instrumentCardsInPlay.Clear();
        musicianCardsInPlay.Clear();

        foreach (CardPair cardPair in cardpairs)
        {
            Card[] cardsInPair = cardPair.GetComponentsInChildren<Card>();
            if (cardsInPair.Length < 2)
            {
                return false;
            }

            foreach (Card card in cardsInPair)
            {
                if (card is MusicianCard)
                    musicianCardsInPlay.Add(card.Name);
                else if (card is InstrumentCard)
                    instrumentCardsInPlay.Add(card.Name);
            }
        }

        return true;
    }

    public void OnPlayButtonPressed()
    {
        if (CheckCardsInPlay())
        {
            GameManager.Instance.SetCardsInPlay(musicianCardsInPlay, instrumentCardsInPlay);
            GoToGameScene();
        }
        else
        {
            ShowInfoPanel(false);
        }
    }
    public void OnMenuButtonPressed()
    {
        if (CheckCardsInPlay())
        {
            GameManager.Instance.SetCardsInPlay(musicianCardsInPlay, instrumentCardsInPlay);
        }

        GoToMenuScene();
    }

    public void GoToGameScene()
    {
        SceneManager.LoadScene(2);
    }

    void ShowInfoPanel(bool show)
    {
        gameObject.transform.Find("InfoPanel").gameObject.SetActive(show);
    }
}

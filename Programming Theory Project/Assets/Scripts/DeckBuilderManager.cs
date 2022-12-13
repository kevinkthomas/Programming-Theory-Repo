using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DeckBuilderManager : MonoBehaviour
{
    GameObject hand;
    CardPair[] cardpairs;

    List<string> instrumentCardsInPlay = new List<string>();
    List<string> musicianCardsInPlay = new List<string>();

    // Start is called before the first frame update
    void Start()
    {
        hand = GameObject.Find("Hand");
        cardpairs = GetComponentsInChildren<CardPair>();

        AddCardsToHandPanels();

        ShowInfoPanel(!CheckCardsInPlay());
    }

    void AddCardsToHandPanels ()
    {
        GameManager.CardLists cards = GameManager.Instance.Cards;
        List<GameManager.OwnedCardEntry> ownedInstrumentCards = GameManager.Instance.OwnedInstrumentCards;
        List<GameManager.OwnedCardEntry> ownedMusicanCards = GameManager.Instance.OwnedMusicanCards;

        AddCardsFromListToPanels(ownedMusicanCards, cards.MusicianCards, true);
        AddCardsFromListToPanels(ownedInstrumentCards, cards.InstrumentCards, false);
    }

    void AddCardsFromListToPanels(List<GameManager.OwnedCardEntry> ownedCardList, List<GameManager.CardEntry> cardEntryList, bool isMusicianCard)
    {
        GameObject parent = hand;

        foreach (GameManager.OwnedCardEntry ownedCard in ownedCardList)
        {
            GameManager.CardEntry cardEntry = cardEntryList.Find(x => x.CardName == ownedCard.CardName);

            if (ownedCard.IsInPlay)
            {
                parent = cardpairs[ownedCard.InPlayPairIndex - 1].gameObject;
            }
            else
            {
                parent = hand;
            }

            Card card;

            if (isMusicianCard)
            {
                card = GameManager.Instance.CreateMusicianCard(ownedCard.CardName, parent.transform);
            }
            else
            {
                card = GameManager.Instance.CreateInstrumentCard(ownedCard.CardName, parent.transform);
            }

            card.IsDraggable = true;
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

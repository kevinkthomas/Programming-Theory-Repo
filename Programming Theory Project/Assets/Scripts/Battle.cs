using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Battle : MonoBehaviour
{
    enum Result
    {
        LOSE = -1,
        DRAW = 0,
        WIN = 1
    }

    [SerializeField] float battleOffset = 20f;

    List<GameManager.OwnedCardEntry> supportActMusicianCards = new List<GameManager.OwnedCardEntry>();
    List<GameManager.OwnedCardEntry> supportActInstrumentCards = new List<GameManager.OwnedCardEntry>();
    List<GameObject> playerCardPanels = new List<GameObject>();
    List<GameObject> supportCardPanels = new List<GameObject>();
    List<TMP_Text> battleTexts = new List<TMP_Text>();
    List<Result> results = new List<Result>();

    float originalSupportYPos;
    float originalPlayerYPos;

    Card playerMusicianCardInPlay;
    Card playerInstrumentCardInPlay;
    Card supportMusicianCardInPlay;
    Card supportInstrumentCardInPlay;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.GetSupportActCards(supportActMusicianCards, supportActInstrumentCards);

        playerCardPanels.Add(GameObject.Find("PlayerPairPanel1"));
        playerCardPanels.Add(GameObject.Find("PlayerPairPanel2"));
        playerCardPanels.Add(GameObject.Find("PlayerPairPanel3"));

        supportCardPanels.Add(GameObject.Find("SupportPairPanel1"));
        supportCardPanels.Add(GameObject.Find("SupportPairPanel2"));
        supportCardPanels.Add(GameObject.Find("SupportPairPanel3"));

        battleTexts.Add(transform.Find("VersesText1").GetComponentInChildren<TMP_Text>());
        battleTexts.Add(transform.Find("VersesText2").GetComponentInChildren<TMP_Text>());
        battleTexts.Add(transform.Find("VersesText3").GetComponentInChildren<TMP_Text>());

        // ABSTRACTION
        AddCardsToPlayerPanels();
        AddCardsToSupportPanels();

        originalSupportYPos = supportCardPanels[0].transform.position.y;
        originalPlayerYPos = playerCardPanels[0].transform.position.y;

        InitBattle(0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnSelectCategory(int battleIndex, int categoryIndex, GameManager.CARD_TYPE cardType)
    {
        int playerValue, supportValue;

        if (cardType == GameManager.CARD_TYPE.MUSICIAN_CARD)
        {
            playerValue = playerMusicianCardInPlay.GetCategoryValue(categoryIndex);
            supportValue = supportMusicianCardInPlay.GetCategoryValue(categoryIndex);
            playerMusicianCardInPlay.HighlightCategory(categoryIndex);
            supportMusicianCardInPlay.HighlightCategory(categoryIndex);
        }
        else
        {
            playerValue = playerInstrumentCardInPlay.GetCategoryValue(categoryIndex);
            supportValue = supportInstrumentCardInPlay.GetCategoryValue(categoryIndex);
            playerInstrumentCardInPlay.HighlightCategory(categoryIndex);
            supportInstrumentCardInPlay.HighlightCategory(categoryIndex);
        }

        if (playerValue > supportValue)
        {
            battleTexts[battleIndex].text = "Player Wins!";
            results.Add(Result.WIN);
        } 
        else if (playerValue < supportValue)
        {
            battleTexts[battleIndex].text = "Support Wins!";
            results.Add(Result.LOSE);
        }
        else
        {
            battleTexts[battleIndex].text = "It's a draw";
            results.Add(Result.DRAW);
        }

        // ABSTRACTION
        EndBattle(battleIndex);

        battleIndex++;

        if (battleIndex < playerCardPanels.Count)
        {
            // ABSTRACTION
            InitBattle(battleIndex);
        }
        else
        {
            // ABSTRACTION
            BattlesComplete();
        }
    }

    void AddCardsToPanel(GameObject cardPanel, GameManager.OwnedCardEntry musicianCardEntry, GameManager.OwnedCardEntry instumentCardEntry, bool hideValues)
    {
        Card musicianCard = GameManager.Instance.CreateMusicianCard(musicianCardEntry.CardName, cardPanel.transform);
        Card instrumentCard = GameManager.Instance.CreateInstrumentCard(instumentCardEntry.CardName, cardPanel.transform);

        if (hideValues)
        {
            musicianCard.HideAllValueText();
            instrumentCard.HideAllValueText();
        }
    }

    void AddCardsToPlayerPanels()
    {
        Dictionary<int, GameManager.OwnedCardEntry> ownedInstrumentCards = GameManager.Instance.InstrumentCardsInPlay;
        Dictionary<int, GameManager.OwnedCardEntry> ownedMuicianCards = GameManager.Instance.MusicanCardsInPlay;

        for (int pairIndex = 0; pairIndex < playerCardPanels.Count; pairIndex++)
        {
            GameObject cardPanel = playerCardPanels[pairIndex];

            GameManager.OwnedCardEntry musicianCardEntry = ownedMuicianCards[pairIndex];
            GameManager.OwnedCardEntry instumentCardEntry = ownedInstrumentCards[pairIndex];

            // ABSTRACTION
            AddCardsToPanel(cardPanel, musicianCardEntry, instumentCardEntry, false);
        }
    }

    void AddCardsToSupportPanels()
    {
        for (int pairIndex = 0; pairIndex < supportCardPanels.Count; pairIndex++)
        {
            GameObject cardPanel = supportCardPanels[pairIndex];

            GameManager.OwnedCardEntry musicianCardEntry = supportActMusicianCards[pairIndex];
            GameManager.OwnedCardEntry instumentCardEntry = supportActInstrumentCards[pairIndex];

            // ABSTRACTION
            AddCardsToPanel(cardPanel, musicianCardEntry, instumentCardEntry, true);
        }
    }

    void AddClickListenersToCard(Card card, int battleIndex)
    {

        for (int categoryIndex = 1; categoryIndex <= 4; categoryIndex++)
        {
            Button buttonCtrl = card.gameObject.transform.Find("CategoryButton" + categoryIndex).GetComponent<Button>();

            GameManager.CARD_TYPE cardType;

            if (card is MusicianCard)
            {
                cardType = GameManager.CARD_TYPE.MUSICIAN_CARD;
            } 
            else
            {
                cardType = GameManager.CARD_TYPE.INSTRUMENT_CARD;
            }

            int catagoryIndexCopy = categoryIndex;
            buttonCtrl.onClick.AddListener(() => OnSelectCategory(battleIndex, catagoryIndexCopy, cardType));
        }

    }

    void RemoveClickListenersFromCard(Card card)
    {
        for (int categoryIndex = 0; categoryIndex < 4; categoryIndex++)
        {
            Button buttonCtrl = card.gameObject.transform.Find("CategoryButton" + (categoryIndex + 1).ToString()).GetComponent<Button>();
            buttonCtrl.onClick.RemoveAllListeners();
        }
    }

    void InitBattle(int battleIndex)
    {
        GameObject supportPanel = supportCardPanels[battleIndex];
        GameObject playerPanel = playerCardPanels[battleIndex];

        Vector3 pos = supportPanel.transform.position;

        pos.y -= battleOffset;
        supportPanel.transform.position = pos;


        pos = playerPanel.transform.position;
        pos.y += battleOffset;

        playerPanel.transform.position = pos;

        playerMusicianCardInPlay = playerPanel.GetComponentInChildren<MusicianCard>();
        playerInstrumentCardInPlay = playerPanel.GetComponentInChildren<InstrumentCard>();

        supportMusicianCardInPlay = supportPanel.GetComponentInChildren<MusicianCard>();
        supportInstrumentCardInPlay = supportPanel.GetComponentInChildren<InstrumentCard>();

        battleTexts[battleIndex].text = supportMusicianCardInPlay.Name + " v " + playerMusicianCardInPlay.Name;

        // ABSTRACTION
        AddClickListenersToCard(playerMusicianCardInPlay, battleIndex);
        AddClickListenersToCard(playerInstrumentCardInPlay, battleIndex);
    }

    void EndBattle(int battleIndex)
    {

        supportMusicianCardInPlay.ShowAllValueText();
        supportInstrumentCardInPlay.ShowAllValueText();

        GameObject supportPanel = supportCardPanels[battleIndex];
        GameObject playerPanel = playerCardPanels[battleIndex];

        Vector3 pos = supportPanel.transform.position;

        pos.y = originalSupportYPos;
        supportPanel.transform.position = pos;


        pos = playerPanel.transform.position;
        pos.y = originalPlayerYPos;

        playerPanel.transform.position = pos;

        // ABSTRACTION
        RemoveClickListenersFromCard(playerMusicianCardInPlay);
        RemoveClickListenersFromCard(playerInstrumentCardInPlay);
    }

    void BattlesComplete()
    {
        int endResult = 0;
        string endText = "";

        foreach(Result result in results)
        {
            endResult += ((int)result);
        }

        if (endResult > 0)
        {
            endText = "You Win!";
        }
        else if (endResult < 0)
        {
            endText = "You Lose";
        }
        else
        {
            endText = "It's a draw";
        }

        GameObject endPanel = gameObject.transform.Find("EndPanel").gameObject;
        endPanel.transform.Find("WinText").GetComponentInChildren<TMP_Text>().text = endText;
        endPanel.gameObject.SetActive(true);
    }
}

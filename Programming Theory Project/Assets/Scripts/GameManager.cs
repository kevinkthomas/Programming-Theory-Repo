using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public enum CARD_TYPE
    {
        MUSICIAN_CARD,
        INSTRUMENT_CARD
    }

    [SerializeField] MusicianCard musicianCardPrefab;
    [SerializeField] InstrumentCard instrumentCardPrefab;

    private static readonly int INITIAL_CARD_COUNT = 6;

    public static GameManager Instance { get; private set; }
    public CardLists Cards { get; private set; }
    public string BandName { get; set; }
    public List<OwnedCardEntry> OwnedMusicanCards { get; private set; }
    public List<OwnedCardEntry> OwnedInstrumentCards { get; private set; }
    public bool HasSaveGame { get; private set; }
    public Dictionary<int,OwnedCardEntry> MusicanCardsInPlay { get; private set; }
    public Dictionary<int, OwnedCardEntry> InstrumentCardsInPlay { get; private set; }

    // Start is called before the first frame update
    private void Awake()
    {
        OwnedMusicanCards = new List<OwnedCardEntry>();
        OwnedInstrumentCards = new List<OwnedCardEntry>();
        MusicanCardsInPlay = new Dictionary<int, OwnedCardEntry>();
        InstrumentCardsInPlay = new Dictionary<int, OwnedCardEntry>();

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            LoadCards();
            HasSaveGame = LoadGame();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ReturnToPreviousScene();
        }
    }

    public void ReturnToPreviousScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        if (currentSceneIndex > 0)
        {
            SceneManager.LoadScene(currentSceneIndex-1);
        }
    }

    public void LoadCards()
    {
        TextAsset cardFiles = Resources.Load<TextAsset>("cards");

        Cards = JsonUtility.FromJson<CardLists>(cardFiles.text);
    }

    private void InitialiseListWitheRandomCards(List<OwnedCardEntry> targetList, List<CardEntry> sourceList, int cardCount)
    {
        targetList.Clear();

        while (targetList.Count < cardCount)
        {
            int randomValue = Random.Range(0, sourceList.Count);

            string cardName = sourceList[randomValue].CardName;

            if (!targetList.Exists(x => x.CardName == cardName))
            {
                OwnedCardEntry cardEntry = new OwnedCardEntry();
                cardEntry.CardName = cardName;
                targetList.Add(cardEntry);
            }
        }
    }

    private List<CardEntry> GetUnownedCards(List<CardEntry> sourceList, List<OwnedCardEntry> ownedList)
    {
        List<CardEntry> unownedCards = new List<CardEntry>();

        foreach(CardEntry cardEntry in sourceList)
        {
            if (!ownedList.Exists(x => x.CardName == cardEntry.CardName))
            {
                unownedCards.Add(cardEntry);
            }
        }

        return unownedCards;
    }

    public void GetSupportActCards(List<OwnedCardEntry> musicianList, List<OwnedCardEntry> instrumentList)
    {
        List<CardEntry> availableMusicianCards = GetUnownedCards(Cards.MusicianCards, OwnedMusicanCards);
        List<CardEntry> availableInstrumentCards = GetUnownedCards(Cards.InstrumentCards, OwnedInstrumentCards);

        InitialiseListWitheRandomCards(musicianList, availableMusicianCards, 3);
        InitialiseListWitheRandomCards(instrumentList, availableInstrumentCards, 3);
    }

    public void ResetGame()
    {
        InitialiseListWitheRandomCards(OwnedMusicanCards, Cards.MusicianCards, INITIAL_CARD_COUNT);
        InitialiseListWitheRandomCards(OwnedInstrumentCards, Cards.InstrumentCards, INITIAL_CARD_COUNT);
    }

    public void SetCardsInPlay(List<string> musicianCardsInPlay, List<string> instrumentCardsInPlay)
    {
        InstrumentCardsInPlay.Clear();
        MusicanCardsInPlay.Clear();

        foreach (OwnedCardEntry ownedCardEntry in OwnedInstrumentCards)
        {
            int inPlayIndex = instrumentCardsInPlay.FindIndex(x => x == ownedCardEntry.CardName);
            ownedCardEntry.InPlayPairIndex = inPlayIndex + 1;
            ownedCardEntry.IsInPlay = inPlayIndex >= 0;

            if (inPlayIndex >= 0)
            {
                InstrumentCardsInPlay.Add(inPlayIndex, ownedCardEntry);
            }
        }

        foreach (OwnedCardEntry ownedCardEntry in OwnedMusicanCards)
        {
            int inPlayIndex = musicianCardsInPlay.FindIndex(x => x == ownedCardEntry.CardName);
            ownedCardEntry.InPlayPairIndex = inPlayIndex + 1;
            ownedCardEntry.IsInPlay = inPlayIndex >= 0;

            if (inPlayIndex >= 0)
            {
                MusicanCardsInPlay.Add(inPlayIndex, ownedCardEntry);
            }
        }

        SaveGame();
    }

    public Card CreateMusicianCard(string cardName, Transform parent)
    {
        CardEntry cardEntry = Cards.MusicianCards.Find(x => x.CardName == cardName);
        return CreateCardObject(musicianCardPrefab, cardEntry, parent);
    }

    public Card CreateInstrumentCard(string cardName, Transform parent)
    {
        CardEntry cardEntry = Cards.InstrumentCards.Find(x => x.CardName == cardName);
        return CreateCardObject(instrumentCardPrefab, cardEntry, parent);
    }

    public Card CreateCardObject(Card prefab, CardEntry cardEntry, Transform parent)
    {
        Card card = (Card)Instantiate<Card>(prefab, new Vector3(100, 100, 100), Quaternion.identity, parent.transform);

        card.Name = cardEntry.CardName;
        card.Description = cardEntry.CardDescription;

        card.SetImage(cardEntry.CardName);

        card.CategoryValue1 = cardEntry.Category1Value;
        card.CategoryValue2 = cardEntry.Category2Value;
        card.CategoryValue3 = cardEntry.Category3Value;
        card.CategoryValue4 = cardEntry.Category4Value;

        return card;
    }

    public bool LoadGame()
    {
        bool hasSaveGame = false;

        string path = Application.persistentDataPath + "/savefile.json";

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            BandName = data.BandName;
            OwnedInstrumentCards = data.OwnedInstrumentCards;
            OwnedMusicanCards = data.OwnedMusicanCards;

            hasSaveGame = true;
        }

        return hasSaveGame;
    }

    public void SaveGame()
    {
        SaveData saveData = new SaveData();
        saveData.BandName = BandName;
        saveData.OwnedInstrumentCards = OwnedInstrumentCards;
        saveData.OwnedMusicanCards = OwnedMusicanCards;

        string json = JsonUtility.ToJson(saveData);
        File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);
    }

    [System.Serializable]
    public class CardEntry
    {
        public string CardName;
        public string CardDescription;
        public int Category1Value;
        public int Category2Value;
        public int Category3Value;
        public int Category4Value;
    }

    [System.Serializable]
    public class CardLists
    {
        public List<CardEntry> MusicianCards;
        public List<CardEntry> InstrumentCards;
    }

    [System.Serializable]
    public class OwnedCardEntry
    {
        public string CardName;
        public bool IsInPlay;
        public int InPlayPairIndex = 0;
        public int Experience;
    }

    [System.Serializable]
    public class SaveData
    {
        public string BandName;
        public List<OwnedCardEntry> OwnedMusicanCards;
        public List<OwnedCardEntry> OwnedInstrumentCards;
    }
}

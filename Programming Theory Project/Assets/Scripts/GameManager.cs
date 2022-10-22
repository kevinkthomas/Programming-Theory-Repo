using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static readonly int INITIAL_CARD_COUNT = 6;

    public static GameManager Instance { get; private set; }
    public CardLists Cards { get; private set; }
    public string BandName { get; set; }
    public List<OwnedCardEntry> OwnedMusicanCards { get; private set; }
    public List<OwnedCardEntry> OwnedInstrumentCards { get; private set; }
    public bool HasSaveGame { get; private set; }

    // Start is called before the first frame update
    private void Awake()
    {
        OwnedMusicanCards = new List<OwnedCardEntry>();
        OwnedInstrumentCards = new List<OwnedCardEntry>();

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

    private void InitialiseListWitheRandomCards(List<OwnedCardEntry> ownedList, List<CardEntry> sourceList)
    {
        ownedList.Clear();

        while (ownedList.Count < INITIAL_CARD_COUNT)
        {
            int randomValue = Random.Range(0, sourceList.Count);

            string cardName = sourceList[randomValue].CardName;

            if (!ownedList.Exists(x => x.CardName == cardName))
            {
                OwnedCardEntry cardEntry = new OwnedCardEntry();
                cardEntry.CardName = cardName;
                ownedList.Add(cardEntry);
            }
        }
    }

    public void ResetGame()
    {
        InitialiseListWitheRandomCards(OwnedMusicanCards, Cards.MusicianCards);
        InitialiseListWitheRandomCards(OwnedInstrumentCards, Cards.InstrumentCards);
    }

    public void SetCardsInPlay(List<string> musicianCardsInPlay, List<string> instrumentCardsInPlay)
    {
        foreach(OwnedCardEntry ownedCardEntry in OwnedInstrumentCards)
        {
            int inPlayIndex = instrumentCardsInPlay.FindIndex(x => x == ownedCardEntry.CardName);
            ownedCardEntry.InPlayPairIndex = inPlayIndex + 1;
            ownedCardEntry.IsInPlay = inPlayIndex >= 0;
        }

        foreach (OwnedCardEntry ownedCardEntry in OwnedMusicanCards)
        {
            int inPlayIndex = musicianCardsInPlay.FindIndex(x => x == ownedCardEntry.CardName);
            ownedCardEntry.InPlayPairIndex = inPlayIndex + 1;
            ownedCardEntry.IsInPlay = inPlayIndex >= 0;
        }

        SaveGame();
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

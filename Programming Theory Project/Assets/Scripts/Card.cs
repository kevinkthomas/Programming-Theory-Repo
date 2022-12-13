using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class Card : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public string Name { get; set; }
    public string Description { get; set; }
    protected int m_CategoryValue1;
    public int CategoryValue1
    {
        get { return m_CategoryValue1; }
        set { m_CategoryValue1 = Mathf.Max(0, value); }
    }
    protected int m_CategoryValue2;
    public int CategoryValue2
    {
        get { return m_CategoryValue2; }
        set { m_CategoryValue2 = Mathf.Max(0, value); }
    }
    protected int m_CategoryValue3;
    public int CategoryValue3
    {
        get { return m_CategoryValue3; }
        set { m_CategoryValue3 = Mathf.Max(0, value); }
    }
    protected int m_CategoryValue4;
    public int CategoryValue4
    {
        get { return m_CategoryValue4; }
        set { m_CategoryValue4 = Mathf.Max(0, value); }
    }
    public Transform DropParent { get; set; }
    public bool IsDraggable { get; set; }

    private Sprite cardImageSprite = null;
    private Vector2 dragoffset = new Vector2();
    private TMP_Text nameText;
    private TMP_Text descriptionText;
    private Image cardImage;
    private Dictionary<int,TMP_Text> valueTexts = new Dictionary<int, TMP_Text>();
    private Dictionary<int, TMP_Text> categoryTexts = new Dictionary<int, TMP_Text>();

    private void Awake()
    {
        nameText = transform.Find("CardNameText").GetComponentInChildren<TMP_Text>();
        descriptionText = transform.Find("CardDescriptionText").GetComponentInChildren<TMP_Text>();
        cardImage = transform.Find("CardImage").GetComponentInChildren<Image>();
        categoryTexts.Add(1, transform.Find("CategoryButton1").GetComponentInChildren<TMP_Text>());
        categoryTexts.Add(2, transform.Find("CategoryButton2").GetComponentInChildren<TMP_Text>());
        categoryTexts.Add(3, transform.Find("CategoryButton3").GetComponentInChildren<TMP_Text>());
        categoryTexts.Add(4, transform.Find("CategoryButton4").GetComponentInChildren<TMP_Text>());

        valueTexts.Add(1,transform.Find("Value1").GetComponentInChildren<TMP_Text>());
        valueTexts.Add(2,transform.Find("Value2").GetComponentInChildren<TMP_Text>());
        valueTexts.Add(3,transform.Find("Value3").GetComponentInChildren<TMP_Text>());
        valueTexts.Add(4,transform.Find("Value4").GetComponentInChildren<TMP_Text>());
    }

    private void Start()
    {
        InitialiseCard();
    }

    protected void InitialiseCard()
    {
        nameText.text = Name;
        descriptionText.text = Description;

        string[] text = GetCatagoryText();

        categoryTexts[1].text = text[0];
        categoryTexts[2].text = text[1];
        categoryTexts[3].text = text[2];
        categoryTexts[4].text = text[3];

        valueTexts[1].text = CategoryValue1.ToString();
        valueTexts[2].text = CategoryValue2.ToString();
        valueTexts[3].text = CategoryValue3.ToString();
        valueTexts[4].text = CategoryValue4.ToString();
    }

    public virtual void SetImage(string imageName)
    {
        cardImageSprite = Resources.Load<Sprite>("CardImages/" + imageName);
        cardImage.sprite = cardImageSprite;
    }

    public virtual string[] GetCatagoryText()
    {
        return new string[0];
    }

    public void HighlightCategory(int categoryIndex)
    {
        Color color = new Color(255, 0, 0);

        categoryTexts[categoryIndex].color = color;
        valueTexts[categoryIndex].color = color;
    }

    public void HideAllValueText()
    {
        foreach(KeyValuePair<int,TMP_Text> keyValuePair in valueTexts)
        {
            keyValuePair.Value.gameObject.SetActive(false);
        }
    }

    public void ShowAllValueText()
    {
        foreach (KeyValuePair<int, TMP_Text> keyValuePair in valueTexts)
        {
            keyValuePair.Value.gameObject.SetActive(true);
        }
    }

    public void ShowValueText(int textIndex)
    {
        valueTexts[textIndex].gameObject.SetActive(true);
    }

    public int GetCategoryValue(int categoryIndex)
    {
        int value = 0;

        if (valueTexts.Count >= categoryIndex)
        {
            int.TryParse(valueTexts[categoryIndex].text, out value);
        }

        return value;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (IsDraggable)
        {
            transform.position = eventData.position - dragoffset;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (IsDraggable)
        {
            dragoffset.x = eventData.position.x - transform.position.x;
            dragoffset.y = eventData.position.y - transform.position.y;

            DropParent = transform.parent;
            transform.SetParent(transform.parent.parent);

            GetComponent<CanvasGroup>().blocksRaycasts = false;
            gameObject.transform.SetAsLastSibling();
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (IsDraggable)
        {
            if (DropParent != null)
            {
                transform.SetParent(DropParent);
            }

            GetComponent<CanvasGroup>().blocksRaycasts = true;
        }
    }
}

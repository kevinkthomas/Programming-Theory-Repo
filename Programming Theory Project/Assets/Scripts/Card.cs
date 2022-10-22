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
    public int CategoryValue1 { get; set; }
    public int CategoryValue2 { get; set; }
    public int CategoryValue3 { get; set; }
    public int CategoryValue4 { get; set; }
    public Transform DropParent { get; set; }

    private Sprite cardImageSprite = null;
    private Vector2 dragoffset = new Vector2();
    private TMP_Text nameText;
    private TMP_Text descriptionText;
    private Image cardImage;
    private TMP_Text category1Text;
    private TMP_Text category2Text;
    private TMP_Text category3Text;
    private TMP_Text category4Text;
    private TMP_Text value1Text;
    private TMP_Text value2Text;
    private TMP_Text value3Text;
    private TMP_Text value4Text;

    private void Awake()
    {
        nameText = transform.Find("CardNameText").GetComponentInChildren<TMP_Text>();
        descriptionText = transform.Find("CardDescriptionText").GetComponentInChildren<TMP_Text>();
        cardImage = transform.Find("CardImage").GetComponentInChildren<Image>();
        category1Text = transform.Find("Category1").GetComponentInChildren<TMP_Text>();
        category2Text = transform.Find("Category2").GetComponentInChildren<TMP_Text>();
        category3Text = transform.Find("Category3").GetComponentInChildren<TMP_Text>();
        category4Text = transform.Find("Category4").GetComponentInChildren<TMP_Text>();
        value1Text = transform.Find("Value1").GetComponentInChildren<TMP_Text>();
        value2Text = transform.Find("Value2").GetComponentInChildren<TMP_Text>();
        value3Text = transform.Find("Value3").GetComponentInChildren<TMP_Text>();
        value4Text = transform.Find("Value4").GetComponentInChildren<TMP_Text>();
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

        category1Text.text = text[0];
        category2Text.text = text[1];
        category3Text.text = text[2];
        category4Text.text = text[3];

        value1Text.text = CategoryValue1.ToString();
        value2Text.text = CategoryValue2.ToString();
        value3Text.text = CategoryValue3.ToString();
        value4Text.text = CategoryValue4.ToString();
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

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position - dragoffset;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        dragoffset.x = eventData.position.x - transform.position.x;
        dragoffset.y = eventData.position.y - transform.position.y;

        DropParent = transform.parent;
        transform.SetParent(transform.parent.parent);

        GetComponent<CanvasGroup>().blocksRaycasts = false;
        gameObject.transform.SetAsLastSibling();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (DropParent != null)
        {
            transform.SetParent(DropParent);
        }

        GetComponent<CanvasGroup>().blocksRaycasts = true;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

public class DropHandler : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        Card card = eventData.pointerDrag.GetComponent<Card>();
        card.DropParent = gameObject.transform;

        OnDropAction(card);
    }

    public virtual void OnDropAction(Card card)
    {

    }
}

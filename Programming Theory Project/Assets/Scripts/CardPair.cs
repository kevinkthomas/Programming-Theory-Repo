using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

public class CardPair : DropHandler
{
    [SerializeField] bool allowDrop;

    public override void OnDropAction(Card card)
    {
        if (allowDrop)
        {
            Transform handTransform = GameObject.FindObjectOfType<Deck>().transform;

            if (card is MusicianCard)
            {
                MusicianCard otherMusicianCard = GetComponentInChildren<MusicianCard>();

                if (otherMusicianCard != null)
                {
                    otherMusicianCard.gameObject.transform.SetParent(handTransform);
                }
            }
            else if (card is InstrumentCard)
            {
                InstrumentCard otherInstrumentCard = GetComponentInChildren<InstrumentCard>();

                if (otherInstrumentCard != null)
                {
                    otherInstrumentCard.gameObject.transform.SetParent(handTransform);
                }
            }
        }
    }
}

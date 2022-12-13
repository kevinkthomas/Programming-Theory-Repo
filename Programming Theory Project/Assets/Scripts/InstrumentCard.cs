using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// INHERITANCE
public class InstrumentCard : Card
{
    private static readonly string[] categories = { "Range", "Loudness", "Exotic", "Skill Required" };

    public override void SetImage(string imageName)
    {
        base.SetImage("Instruments\\" + imageName);
    }

    // INHERITANCE
    public override string[] GetCatagoryText()
    {
        return categories;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// INHERITANCE
public class MusicianCard : Card
{
    private static readonly string[] categories = { "Attitude", "Decadence", "Proficiency", "Charisma" };

    public override void SetImage(string imageName)
    {
        base.SetImage("Musicians\\"+ imageName);
    }

    // INHERITANCE
    public override string[] GetCatagoryText()
    {
        return categories;
    }
}

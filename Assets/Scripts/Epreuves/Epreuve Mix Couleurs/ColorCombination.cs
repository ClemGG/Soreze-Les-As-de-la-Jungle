using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public enum ColorID
//{
//    Null = -1,
//    Magenta = 0,
//    Jaune = 1,
//    Cyan = 2,
//    Noir = 3,
//    Blanc = 4,
//    Rouge = 5,
//    Vert = 6,
//    Bleu = 7,
//    Violet = 8,
//    Orange = 10,
//    Saumon = 11,
//    Gris = 12,
//    JauneClair = 13,
//};

public enum ColorID
{
    Null = -1,
    Magenta = 0,
    Jaune = 1,
    Cyan = 2,
    Noir = 3,
    Blanc = 4,
    Vert = 5,
    Violet = 6,
    Rose = 7,
    BleuFoncé = 8,
    Rouge = 9,
    Gris = 10,
    VertFoncé = 11,
    Brun = 12,
    RoseGris = 13,
    Beige = 14,
};

[System.Serializable]
public class ColorCombination
{
    public string tag = null;
    public ColorID colorResultID = ColorID.Null;
    public List<ColorID> colorsToMix;
    public Color colorResult = Color.white;


}

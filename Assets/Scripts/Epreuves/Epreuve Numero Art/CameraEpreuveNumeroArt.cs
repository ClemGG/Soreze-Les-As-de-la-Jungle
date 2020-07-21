using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraEpreuveNumeroArt : GodMode
{
    EpreuveNumeroArt e;

    protected override void Start()
    {
        e = (EpreuveNumeroArt)Epreuve.instance;
        base.Start();
    }
}

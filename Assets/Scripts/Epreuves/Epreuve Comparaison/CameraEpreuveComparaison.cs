using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraEpreuveComparaison : GodMode
{
    EpreuveComparaison e;

    protected override void Start()
    {
        e = (EpreuveComparaison)Epreuve.instance;
        base.Start();
    }


}

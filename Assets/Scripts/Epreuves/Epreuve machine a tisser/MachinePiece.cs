using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachinePiece : MonoBehaviour
{
    [Space(10)]
    [Header("Scripts & Components : ")]
    [Space(10)]

    public Vector3 startPos, startEuler;
    [HideInInspector] public Transform originalParent;
    Transform t;
    EpreuveMachineTisser epreuve;


    [Space(10)]
    [Header("Piece : ")]
    [Space(10)]

    public int slotIndex;
    

    private void Start()
    {
        epreuve = (EpreuveMachineTisser)Epreuve.instance;
        t = transform;
        originalParent = t.parent;
        startPos = t.position;
        startEuler = t.eulerAngles;
    }

}
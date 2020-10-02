using UnityEngine;

public class MachinePiece : MonoBehaviour
{
    [Space(10)]
    [Header("Scripts & Components : ")]
    [Space(10)]

    public Vector3 startPos;
    public Vector3 startEuler;
    [HideInInspector] public Transform originalParent;
    Transform t;
    EpreuveMachineTisser epreuve;
    CameraControllerMachineTisser cam;


    [Space(10)]
    [Header("Piece : ")]
    [Space(10)]

    public int slotIndex;
    

    private void Start()
    {
        epreuve = (EpreuveMachineTisser)Epreuve.instance;
        cam = FindObjectOfType<CameraControllerMachineTisser>();
        t = transform;
        originalParent = t.parent;
        startPos = t.position;
        startEuler = t.eulerAngles;
    }


    private void OnMouseDown()
    {
        if (epreuve.allowedToPlacePiece)
        {
            cam.GetPiece();
        }
    }
}
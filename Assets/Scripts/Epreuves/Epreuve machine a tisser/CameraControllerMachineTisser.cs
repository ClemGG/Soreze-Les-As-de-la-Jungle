using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControllerMachineTisser : MonoBehaviour
{
    EpreuveMachineTisser epreuve;

    [Space(10)]
    [Header("Movement : ")]
    [Space(10)]

    [SerializeField] float vitesseDeplacement = 10f;
    float temp = 0f;


    [Space]

    [SerializeField] float sensitivityX = 5; //reglage de la sensibilité de la souris en x
    [SerializeField] float sensitivityY = 5; //reglage de la sensibilité de la souris en y


    float minimumX = -360; //le min et max de rotation que l'on peux avoir sur x
    float maximumX = 360;
    [SerializeField] float minimumY = -60; //le min et le max de rotation que l'on peux avoir sur y
    [SerializeField] float maximumY = 60;

    float rotationX = 0; //la rotation de depart en x
    float rotationY = 0; //la rotation de depart en y
    Quaternion originalRotation; //Quaternion contenant la rotation de départ


    [Space(10)]
    [Header("Piece : ")]
    [Space(10)]

    public LayerMask pieceAndSlotMask;
    Camera mainCam;
    [HideInInspector] public Transform piecePosOnThisCam;

    [HideInInspector] public MachinePiece currentPiece;
    RaycastHit hit;








    // Start is called before the first frame update
    void Start()
    {
        epreuve = (EpreuveMachineTisser)Epreuve.instance;
        mainCam = GetComponent<Camera>();
        piecePosOnThisCam = mainCam.transform.GetChild(0);

        originalRotation = transform.localRotation; //la rotation de depart est la rotation locale du transform
        temp = vitesseDeplacement;
    }


    void Update()
    {
        if (!epreuve.EpreuveFinished)
        {
#if UNITY_EDITOR
            RotateCamera();
            MoveCamera();
#endif

            if (Input.GetMouseButtonDown(0))
            {
                GetPiece();
            }
        }
    }











    private void RotateCamera()
    {
        //on recupere les getaxis de la souris, on multiplie par la sensibilité et on calcule la rotation
        rotationX += Input.GetAxis("Mouse X") * sensitivityX;
        rotationY += Input.GetAxis("Mouse Y") * sensitivityY;

        rotationX = ClampAngle(rotationX, minimumX, maximumX);
        rotationY = ClampAngle(rotationY, minimumY, maximumY);

        //on prepare la rotation en x autour de l'axe y
        Quaternion xQuaternion = Quaternion.AngleAxis(rotationX, Vector3.up);
        //on prepare la rotation autour de l'axe x
        Quaternion yQuaternion = Quaternion.AngleAxis(rotationY, Vector3.left);
        //on effectue la rotation
        transform.localRotation = originalRotation * xQuaternion * yQuaternion;
    }

    private void MoveCamera()

    {
        transform.Translate(Vector3.forward * Input.GetAxis("Vertical") * vitesseDeplacement * Time.fixedDeltaTime);
        transform.Translate(Vector3.right * Input.GetAxis("Horizontal") * vitesseDeplacement * Time.fixedDeltaTime);
        //transform.Translate(Vector3.up * Input.GetAxis("Pages") * vitesseDeplacement * Time.fixedDeltaTime); //not setup
    }

    static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360.0f)
            angle += 360.0f;
        if (angle > 360.0f)
            angle -= 360.0f;
        return Mathf.Clamp(angle, min, max);
    }




    public void GetPiece()
    {

        if (Raycast())
        {
            //Debug.Log("touché", gameObject);

            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("slot"))
            {

                if (currentPiece)
                {

                    if (IndexesMatch(hit) && currentPiece.slotIndex == epreuve.currentPieceIndex)
                    {
                        epreuve.PlacePieceOnSlot(currentPiece, epreuve.machineSlots[currentPiece.slotIndex], true);
                    }
                    else
                    {
                        epreuve.ShowDialogueBadAnswer();
                        AudioManager.instance.Play(epreuve.errorClip);

                        //currentPiece.transform.parent = currentPiece.originalParent;
                        //currentPiece.transform.position = currentPiece.startPos;
                        //currentPiece.transform.eulerAngles = currentPiece.startEuler;
                    }
                }
            }
            //else if (hit.collider.gameObject.layer == LayerMask.NameToLayer("piece"))
            //{
            //    MachinePiece mp = hit.collider.gameObject.GetComponent<MachinePiece>();

            //    if (currentPiece)
            //    {
            //        currentPiece.transform.parent = currentPiece.originalParent;
            //        currentPiece.transform.position = currentPiece.startPos;
            //        currentPiece.transform.eulerAngles = currentPiece.startEuler;

            //        mp.transform.position = piecePosOnThisCam.position;
            //        mp.transform.eulerAngles = piecePosOnThisCam.eulerAngles;
            //        mp.transform.parent = piecePosOnThisCam;

            //        currentPiece = mp;
            //    }
            //    else
            //    {
            //        mp.transform.position = piecePosOnThisCam.position;
            //        mp.transform.eulerAngles = piecePosOnThisCam.eulerAngles;
            //        mp.transform.parent = piecePosOnThisCam;

            //        currentPiece = mp;
            //    }
            //}



        }
    }


    private bool IndexesMatch(RaycastHit hit)
    {
        MachineSlot mp = hit.collider.gameObject.GetComponent<MachineSlot>();
        return mp.slotIndex == currentPiece.slotIndex;
    }
    private bool Raycast()
    {
        Ray ray = mainCam.ViewportPointToRay(Vector3.one / 2f);
        return Physics.Raycast(ray, out hit, pieceAndSlotMask);
    }
}

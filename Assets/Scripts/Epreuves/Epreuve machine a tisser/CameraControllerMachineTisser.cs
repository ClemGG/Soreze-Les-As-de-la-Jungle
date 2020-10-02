using UnityEngine;

public class CameraControllerMachineTisser : MonoBehaviour
{


    #region Variables

    EpreuveMachineTisser epreuve;


    [Space(10)]
    [Header("Piece : ")]
    [Space(10)]

    public LayerMask pieceAndSlotMask;
    Camera mainCam;
    [HideInInspector] public Transform piecePosOnThisCam;

    [HideInInspector] public MachinePiece currentPiece;
    MachineSlot lastSlot;

    #endregion




    #region Mono

    // Start is called before the first frame update
    void Start()
    {
        epreuve = (EpreuveMachineTisser)Epreuve.instance;
        mainCam = GetComponent<Camera>();
        piecePosOnThisCam = mainCam.transform.GetChild(0);


#if UNITY_EDITOR || UNITY_STANDALONE
        originalRotation = transform.localRotation; //la rotation de depart est la rotation locale du transform
#endif
    }


    void Update()
    {
        if (!epreuve.EpreuveFinished)
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            RotateCamera();
            MoveCamera();


            //Pour s'assurer qu'on ne peut ajouter la pièce qu'en cliquant au centre de l'écran.
            //Ca évitera de faire disparaître le btn de la boîte à outils
            Vector2 clickPoint = mainCam.ScreenToViewportPoint(Input.mousePosition);
            if (Input.GetMouseButtonDown(0) && Vector2.Distance(clickPoint, Vector2.one/2f) < .2f && epreuve.allowedToPlacePiece)
            {
                GetPiece();
            }
#else
            if(Input.touchCount == 1)
            {
                Vector2 clickPoint = mainCam.ScreenToViewportPoint(Input.GetTouch(0).position);
                if (Input.GetTouch(0).phase == TouchPhase.Began &&
                    Vector2.Distance(clickPoint, Vector2.one/2f) < .2f &&
                    epreuve.allowedToPlacePiece)
                {
                    GetPiece();
                }
            }
#endif



        }
    }

    public void FixedUpdate()
    {

        //Si la boîte à outils est ouverte, on empêche le placement des pièces
        if (epreuve.planPanel.activeSelf)
            return;


        if (Raycast(out RaycastHit hit))
        {
            //Debug.Log("touché", gameObject);

            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("slot"))
            {

                MachineSlot slotHit = hit.collider.GetComponent<MachineSlot>();
                lastSlot = slotHit;

                if (currentPiece)
                {

                    if (IndexesMatch(hit))
                    {
                        slotHit.ShowSurbrillance();
                    }
                    else if(!slotHit.done)
                    {
                        slotHit.ShowTransparent();
                        slotHit.isInSurbrillance = false;
                    }
                }
                else if (!slotHit.done)
                {
                    slotHit.ShowTransparent();
                    slotHit.isInSurbrillance = false;
                }
            }



        }
        else if (lastSlot && !lastSlot.done)
        {
            lastSlot.ShowTransparent();
            lastSlot.isInSurbrillance = false;
            lastSlot = null;
        }
    }

    #endregion






    #region Machine

    public void GetPiece()
    {

        //Si la boîte à outils est ouverte, on empêche le placement des pièces
        if (epreuve.planPanel.activeSelf)
            return;

        if (Raycast(out RaycastHit hit))
        {
            //Debug.Log("touché", gameObject);

            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("slot"))
            {

                if (currentPiece)
                {

                    if (IndexesMatch(hit))
                    {
                        epreuve.PlacePieceOnSlot(currentPiece, epreuve.machineSlots[currentPiece.slotIndex], true);
                    }
                    else
                    {
                        epreuve.ShowDialogueBadAnswer();
                        AudioManager.instance.Play(epreuve.errorClip);

                    }
                }
            }



        }
    }


    private bool IndexesMatch(RaycastHit hit)
    {
        MachineSlot mp = hit.collider.gameObject.GetComponent<MachineSlot>();
        return mp.slotIndex == currentPiece.slotIndex && currentPiece.slotIndex == epreuve.currentPieceIndex;
    }
    private bool Raycast(out RaycastHit hit)
    {
        Ray ray = mainCam.ViewportPointToRay(Vector3.one / 2f);
        return Physics.Raycast(ray, out hit, pieceAndSlotMask);
    }

    #endregion





#if UNITY_EDITOR || UNITY_STANDALONE

    #region Variables

    [Space(10)]
    [Header("Movement : ")]
    [Space(10)]

    [SerializeField] float vitesseDeplacement = 10f;


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


    #endregion

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

#endif



}

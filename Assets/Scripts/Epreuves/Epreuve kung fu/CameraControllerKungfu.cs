using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CameraControllerKungfu : MonoBehaviour
{

    #region Variables


    EpreuveKungfu epreuve;

    [Space(10)]
    [Header("Scripts & Components : ")]
    [Space(10)]


    public Image viseurImg;
    public Animator alAnim, bobAnim;
    Camera mainCam;
    Transform t;


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
    [Header("Kungfu : ")]
    [Space(10)]

    public bool showGizmos = true;
    public LayerMask moustiqueMask;
    public Vector3 boxSize = Vector3.one;
    public float dst;

    RaycastHit hit;
    Collider[] cols;



    [Space(10)]
    [Header("Kungfu : ")]
    [Space(10)]

    public Button alButton;
    public Button bobButton;
    public AnimationCurve animLangueCurve;
    public float animLangueSpeed = 1f;
    public LineRenderer lineAl, lineBob;
    bool lineAlIsFree = true, lineBobIsFree = true;



    #endregion


    #region Mono


    // Start is called before the first frame update
    void Start()
    {
        epreuve = (EpreuveKungfu)Epreuve.instance;
        mainCam = GetComponent<Camera>();
        t = transform;

        originalRotation = transform.localRotation; //la rotation de depart est la rotation locale du transform
        temp = vitesseDeplacement;
    }





#if UNITY_EDITOR || UNITY_STANDALONE
    void Update()
    {
        if (!epreuve.EpreuveFinished)
        {
            RotateCamera();
            MoveCamera();

        }
    }

#endif

    void FixedUpdate()
    {
        if (!epreuve.EpreuveFinished && epreuve.isFirstLevel)
        {

            //retour visuel
            cols = Physics.OverlapBox(t.position + t.forward * boxSize.z, boxSize, t.rotation, moustiqueMask);
            viseurImg.color = cols.Length > 0f ? Color.red : Color.white;


        }
    }



    #endregion



    #region Enemy

    //Appelée quand le joueur clique sur les grenouilles
    public void GetEnemyInBoxcast (int index)
    {
        if (epreuve.EpreuveFinished)
            return;




        if (epreuve.isFirstLevel)
        {
            cols = Physics.OverlapBox(t.position + t.forward * boxSize.z, boxSize, t.rotation, moustiqueMask);

            if (cols.Length > 0)
            {
                Enemy e = cols[0].GetComponent<Enemy>();
                if(e.EligibleForDestruction())
                {
                    StartCoroutine(CatchEnemy(e, index));
                }
            }
        }
    }



    private IEnumerator CatchEnemy(Enemy enemy, int index)
    {
        if (enemy.isTargeted)
            yield break;




        //On récupère le LineRenderer correspondant à l'ID du bouton
        LineRenderer line;
        if (index == 0)
        {
            //Ne pas activer la langue si elle est déjà occupée
            if (!lineAlIsFree)
                yield break;

            lineAlIsFree = alButton.interactable = false;
            line = lineAl;
            alAnim.Play("a_al_tir");
        }
        else
        {
            //Ne pas activer la langue si elle est déjà occupée
            if (!lineBobIsFree)
                yield break;

            lineBobIsFree = bobButton.interactable = false;
            line = lineBob;
            bobAnim.Play("a_bob_tir");
        }



        float timer = 0f;
        Vector3 targetPos = Vector3.zero;
        enemy.isTargeted = true;







        AudioManager.instance.Play(epreuve.langueStartClip1);
        bool hasPlayed = false;




        while (timer < 1f)
        {


            //Attacher l'ennemi au bout de la langue
            if (timer < .5f)
            {
                targetPos = line.transform.InverseTransformPoint(enemy.t.position);
            }



            timer += Time.deltaTime * animLangueSpeed;
            Vector3 languePos = Vector3.LerpUnclamped(line.GetPosition(0), targetPos, animLangueCurve.Evaluate(timer));
            line.SetPosition(1, languePos);



            //Attacher l'ennemi au bout de la langue
            if (timer > .5f)
            {
                if (!hasPlayed)
                {
                    AudioManager.instance.Play(epreuve.langueEndClip1);
                    //ObjectPooler.instance.SpawnFromPool("hit", enemy.t.position, Quaternion.identity);
                    hasPlayed = true;
                }

                //Au lieu de changer isCaught, on va juste appeler le FX et déplacer le moustique,
                //les crashs viennent du script du moustique

                //enemy.isCaught = true;
                enemy.t.position = line.transform.TransformPoint(languePos);

            }

            yield return null;

        }

        if (index == 0)
        {
            lineAlIsFree = alButton.interactable = true;
        }
        else
        {
            lineBobIsFree = bobButton.interactable = true;
        }

        line.SetPosition(1, line.GetPosition(0));



        enemy.isTargeted = false;
        //enemy.isCaught = false;
        enemy.DestroyThisEnemy();

    }

    #endregion




    #region Editeur

#if UNITY_EDITOR || UNITY_STANDALONE

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
        t.localRotation = originalRotation * xQuaternion * yQuaternion;
    }

    private void MoveCamera()

    {
        t.Translate(Vector3.forward * Input.GetAxis("Vertical") * vitesseDeplacement * Time.fixedDeltaTime);
        t.Translate(Vector3.right * Input.GetAxis("Horizontal") * vitesseDeplacement * Time.fixedDeltaTime);
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




#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        if (!showGizmos)
            return;

        Gizmos.color = Color.red;

        //Check if there has been a hit yet
        if (hit.collider)
        {
            //Draw a Ray forward from GameObject toward the hit
            Gizmos.DrawRay(transform.position, transform.forward * hit.distance);
            //Draw a cube that extends to where the hit exists
            Gizmos.DrawWireCube(transform.position + transform.forward * hit.distance, boxSize);
        }
        //If there hasn't been a hit yet, draw the ray at the maximum distance
        else
        {
            //Draw a Ray forward from GameObject toward the maximum distance
            Gizmos.DrawRay(transform.position, transform.forward * dst);
            //Draw a cube at the maximum distance
            Gizmos.DrawWireCube(transform.position + transform.forward * dst, boxSize);
        }
    }
#endif


    #endregion
}

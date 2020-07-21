using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Clement.Utilities.Maths;

public class CameraControllerKungfu : MonoBehaviour
{
    EpreuveKungfu epreuve;

    [Space(10)]
    [Header("Scripts & Components : ")]
    [Space(10)]


    public Image viseurImg;
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
    LayerMask maskToUse;
    public LayerMask enemyMask;
    public LayerMask fourmiMask;
    public LayerMask moustiqueMask;
    public Vector3 boxSize = Vector3.one;
    public float dst;
    RaycastHit hit;



    [Space(10)]
    [Header("Kungfu : ")]
    [Space(10)]

    public AnimationCurve animLangueCurve;
    public float animLangueSpeed = 1f;
    public LineRenderer lineAl, lineBob;
    bool lineAlIsFree = true, lineBobIsFree = true;


    // Start is called before the first frame update
    void Start()
    {
        epreuve = (EpreuveKungfu)Epreuve.instance;
        mainCam = GetComponent<Camera>();
        t = transform;

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

            viseurImg.color = Physics.BoxCast(transform.position, boxSize, transform.forward, out hit, transform.rotation, dst, enemyMask) ? Color.red : Color.white;

#if UNITY_EDITOR || UNITY_STANDALONE
            //if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
            //GetEnemyInBoxcast(Random.Range(0, 2));
#endif

        }
    }


    public void GetEnemyInBoxcast (int index)
    {
        if (epreuve.EpreuveFinished)
            return;


        switch (index)
        {
            case -1:
                maskToUse = enemyMask;
                break;
            //case -1:
            //    maskToUse = enemyMask;
            //    break;
            case 0:
                maskToUse = moustiqueMask;
                break;
            case 1:
                goto case 0;
            //case 1:
            //    maskToUse = fourmiMask;
            //    break;
        }

        if (Physics.BoxCast(transform.position, boxSize, transform.forward, out hit, transform.rotation, dst, maskToUse))
        {
            Enemy e = hit.transform.GetComponent<Enemy>();

            if (epreuve.isFirstLevel)
                StartCoroutine(CatchEnemy(e, index));
            else
                e.DestroyThisEnemy();
        }
    }

    private IEnumerator CatchEnemy(Enemy enemy, int index)
    {

        if (enemy.isTargeted)
            yield break;


        if (!lineAlIsFree && index == 0)
        {
            if (lineBobIsFree)
            {
                index = 1;
            }
            else
                yield break;

        }
        if (!lineBobIsFree && index == 1)
        {
            if (lineAlIsFree)
            {
                index = 0;
            }
            else
                yield break;

        }

        float timer = 0f;

        Vector3 targetPos = Vector3.zero;
        enemy.isTargeted = true;


        if (index == 0)
        {
            if (!lineAlIsFree)
                yield break;

            lineAlIsFree = false;
            AudioManager.instance.Play(epreuve.langueStartClip1);

            bool hasPlayed = false;

            while (timer < 1f)
            {
                //Attacher l'ennemi au bout de la langue
                if (timer < .5f)
                {
                    targetPos = lineAl.transform.InverseTransformPoint(enemy.transform.position);
                }



                timer += Time.deltaTime * animLangueSpeed;
                Vector3 languePos = Vector3.LerpUnclamped(lineAl.GetPosition(0), targetPos, animLangueCurve.Evaluate(timer));
                lineAl.SetPosition(1, languePos);
                yield return null;



                //Attacher l'ennemi au bout de la langue
                if (timer > .5f)
                {
                    if (!hasPlayed)
                    {
                        AudioManager.instance.Play(epreuve.langueEndClip1);
                        hasPlayed = true;
                    }

                    enemy.isCaught = true; 
                    enemy.transform.position = lineAl.transform.TransformPoint(languePos);

                }


            }


            lineAlIsFree = true;
        }
        else if(index == 1)
        {
            if (!lineBobIsFree)
                yield break;

            lineBobIsFree = false;
            AudioManager.instance.Play(epreuve.langueStartClip2);


            bool hasPlayed = false;

            while (timer < 1f)
            {
                //Attacher l'ennemi au bout de la langue
                if (timer < .5f)
                {
                    targetPos = lineBob.transform.InverseTransformPoint(enemy.transform.position);
                }




                timer += Time.deltaTime * animLangueSpeed;
                Vector3 languePos = Vector3.LerpUnclamped(lineBob.GetPosition(0), targetPos, animLangueCurve.Evaluate(timer));
                lineBob.SetPosition(1, languePos);
                yield return null;

                //Attacher l'ennemi au bout de la langue
                if (timer > .5f)
                {
                    if (!hasPlayed)
                    {
                        AudioManager.instance.Play(epreuve.langueEndClip2);
                        hasPlayed = true;
                    }

                    enemy.isCaught = true;
                    enemy.transform.position = lineBob.transform.TransformPoint(languePos);
                }

            }

            lineBobIsFree = true;
        }

        enemy.isTargeted = false;
        enemy.isCaught = false;
        enemy.DestroyThisEnemy();

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

}

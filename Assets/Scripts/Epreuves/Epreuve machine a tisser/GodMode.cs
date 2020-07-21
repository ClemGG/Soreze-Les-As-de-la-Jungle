using UnityEngine;
using System.Collections;
using System;

public class GodMode : MonoBehaviour
{


    [SerializeField] float vitesseDeplacement = 10f;
                     float temp = 0f;


    [Space]

    [SerializeField] float sensitivityX = 5; //reglage de la sensibilité de la souris en x
    [SerializeField] float sensitivityY = 5; //reglage de la sensibilité de la souris en y


    float minimumX = -360; //le min et max de rotation que l'on peux avoir sur x
    float maximumX = 360;
    [SerializeField] float minimumY = -80; //le min et le max de rotation que l'on peux avoir sur y
    [SerializeField] float maximumY = 80;
    
    float rotationX = 0; //la rotation de depart en x
    float rotationY = 0; //la rotation de depart en y
    Quaternion originalRotation; //Quaternion contenant la rotation de départ




    protected virtual void Start()
    {

        originalRotation = transform.localRotation; //la rotation de depart est la rotation locale du transform
        temp = vitesseDeplacement;
    }

    protected virtual void Update()
    {
        RotateCamera();
        MoveCamera();
     
        
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
        //transform.Translate(Vector3.up * Input.GetAxis("Pages") * vitesseDeplacement * Time.fixedDeltaTime);
    }

    static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360.0f)
            angle += 360.0f;
        if (angle > 360.0f)
            angle -= 360.0f;
        return Mathf.Clamp(angle, min, max);
    }
}

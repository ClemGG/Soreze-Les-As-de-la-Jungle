using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lerpcolormat : MonoBehaviour
{
    Material mat;
    public AnimationCurve lerpCurve;
    private float t;
    float sign = 1f;
    // Start is called before the first frame update
    void Start()
    {
        mat = GetComponent<MeshRenderer>().sharedMaterial;
    }

    // Update is called once per frame
    void Update()
    {
        if(t > 1f || t < 0f)
        {
            sign *= -1f;
        }

        t += Time.deltaTime * sign;


        mat.SetColor("_Color", Color.Lerp(Color.cyan, Color.magenta, lerpCurve.Evaluate(t)));
    }
}

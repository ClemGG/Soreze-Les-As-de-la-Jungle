using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorCheckerV2 : MonoBehaviour
{
    [Space(10)]
    [Header("Scripts & Components : ")]
    [Space(10)]

    private EpreuveMixCouleursV2 epreuve;


    [Space(10)]
    [Header("Colors : ")]
    [Space(10)]

    public bool full = false, isAnimating = false;

    [Space(10)]

    public float fillSpeed = 1f;
    public AnimationCurve fillCurve;
    Material liquideMat;



    private void Start()
    {
        epreuve = (EpreuveMixCouleursV2)Epreuve.instance;

        liquideMat = transform.GetChild(0).GetComponent<MeshRenderer>().material;
        liquideMat.SetColor("_Tint", Color.white);
        liquideMat.SetColor("_TopColor", Color.white);
        liquideMat.SetFloat("_FillAmount", 1.6f);

    }






    public IEnumerator AddColorCo(Color col)
    {
        if (!full)
        {
            StartCoroutine(ChangeFioleFillAmount(true, col));
        }
        else
        {
            yield return StartCoroutine(ChangeFioleFillAmount(false, Color.white));
            isAnimating = true;
            yield return new WaitForSeconds(.5f);
            StartCoroutine(ChangeFioleFillAmount(true, col));
        }
    }

    //Appelée par le bouton de suppression du mélange
    public void ResetFioleColor()
    {
        if (full)
            StartCoroutine(ChangeFioleFillAmount(false, Color.white));
    }

    private IEnumerator ChangeFioleFillAmount(bool increase, Color newColor)
    {

        isAnimating = true;

        Color startColor = liquideMat.GetColor("_Tint");

        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * fillSpeed;
            liquideMat.SetFloat("_FillAmount", Mathf.Lerp(1.6f, 0.7f, fillCurve.Evaluate(increase ? t : 1 - t)));
            liquideMat.SetColor("_Tint", Color.Lerp(startColor, newColor, fillCurve.Evaluate(t)));
            liquideMat.SetColor("_TopColor", Color.Lerp(startColor, newColor, fillCurve.Evaluate(t)));

            yield return null;
        }

        isAnimating = false;
        full = increase;

        yield return null;
    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;





public class ColorChecker : MonoBehaviour
{
    [Space(10)]
    [Header("Scripts & Components : ")]
    [Space(10)]

    private EpreuveMixCouleurs epreuve;


    [HideInInspector] public int nbClicks;
    public bool shouldCountClicks = false;
    public float delayBetweenClicks = .3f;
    float timerClicks;

    [Space(10)]
    [Header("Colors : ")]
    [Space(10)]

    public ColorID currentColorID = ColorID.Null;
    public bool full = false, isAnimating = false;

    [Space(10)]

    public float fillSpeed = 1f;
    public AnimationCurve fillCurve;
    Material liquideMat;



    private void Start()
    {
        epreuve = (EpreuveMixCouleurs)EpreuveMixCouleurs.instance;

        currentColorID = ColorID.Null;
        liquideMat = transform.GetChild(0).GetComponent<MeshRenderer>().material;
        liquideMat.SetColor("_Tint", Color.white);
        liquideMat.SetColor("_TopColor", Color.white);

    }



    private void OnMouseDown()
    {
        if (epreuve.EpreuveFinished)
            return;


        if (!isAnimating)
        {
            if (nbClicks == 1)
            {
                nbClicks = 0;
                shouldCountClicks = false;

                if(currentColorID != ColorID.Null && !isAnimating && epreuve.currentColorID == ColorID.Null)
                    ResetFioleColor();
            }
            else
            {
                shouldCountClicks = true;
                nbClicks++;

                if (epreuve.currentColorID != ColorID.Null && epreuve.currentColorID != currentColorID && !isAnimating)
                {
                    AddColor(epreuve.currentColorID);
                    //StartCoroutine(AddColorCo(epreuve.currentColorID));
                    epreuve.currentColorID = ColorID.Null;
                }

            }
        }


    }


    
    public void Update()
    {
        if (shouldCountClicks)
        {
            if (timerClicks < delayBetweenClicks)
            {
                timerClicks += Time.deltaTime;
            }
            else
            {
                timerClicks = 0f;
                shouldCountClicks = false;
                nbClicks = 0;
            }
        }

    }



    public void AddColor(ColorID col)
    {
        StartCoroutine(AddColorCo(col));
    }

    public IEnumerator AddColorCo(ColorID col)
    {
        if (!full)
        {
            StartCoroutine(ChangeFioleFillAmount(true, col));
        }
        else
        {
            yield return StartCoroutine(ChangeFioleFillAmount(false, ColorID.Null));
            isAnimating = true;
            yield return new WaitForSeconds(.5f);
            StartCoroutine(ChangeFioleFillAmount(true, col));
        }
    }

    public void ResetFioleColor()
    {
        if(full)
            StartCoroutine(ChangeFioleFillAmount(false, ColorID.Null));
    }

    private IEnumerator ChangeFioleFillAmount(bool increase, ColorID newID)
    {

        isAnimating = true;

        Color startColor = liquideMat.GetColor("_Tint");
        Color newColor = epreuve.GetColorFromID(newID);

        float t = 0f;

        while(t < 1f)
        {
            t += Time.deltaTime * fillSpeed;
            liquideMat.SetFloat("_FillAmount", Mathf.Lerp(1.3f, 0.6f, fillCurve.Evaluate(increase ? t : 1-t)));
            liquideMat.SetColor("_Tint", Color.Lerp(startColor, newColor, fillCurve.Evaluate(t)));
            liquideMat.SetColor("_TopColor", Color.Lerp(startColor, newColor, fillCurve.Evaluate(t)));

            yield return null;
        }

        currentColorID = increase ? newID : ColorID.Null;
        isAnimating = false;
        full = increase;

        yield return null;
    }



}

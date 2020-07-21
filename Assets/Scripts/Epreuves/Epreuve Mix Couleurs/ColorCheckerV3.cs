using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorCheckerV3 : MonoBehaviour
{
    [Space(10)]
    [Header("Scripts & Components : ")]
    [Space(10)]

    private EpreuveMixCouleursV2 epreuve;
    public Animator bolAnim;

    [Space(10)]
    [Header("Colors : ")]
    [Space(10)]

    public float fillSpeed = 1f;
    public AnimationCurve fillCurve;
    public bool full = false, isAnimating = false;

    [Space(10)]

    public Image liquideImg;



    private void Start()
    {
        epreuve = (EpreuveMixCouleursV2)Epreuve.instance;

        liquideImg.color = Color.white;

    }


    private void Update()
    {
        isAnimating = bolAnim.GetCurrentAnimatorClipInfo(0)[0].clip.name != "idle";

        full = isAnimating && bolAnim.GetCurrentAnimatorClipInfo(0)[0].clip.name != "errorV2";
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

        Color startColor = liquideImg.color;

        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * fillSpeed; 
            liquideImg.color = Color.Lerp(startColor, newColor, fillCurve.Evaluate(t));

            yield return null;
        }

        isAnimating = false;
        full = increase;

        yield return null;
    }


}

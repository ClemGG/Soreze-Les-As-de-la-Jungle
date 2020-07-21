using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineSlot : MonoBehaviour
{
    [Space(10)]
    [Header("Scripts & Components : ")]
    [Space(10)]

    MeshRenderer[] meshesInChildren;
    public bool done = false;
    public int slotIndex;

    [Space(10)]
    [Header("Materials : ")]
    [Space(10)]

    public Material baseMat;
    public Material transparentMat;
    public Material surbrillanceMat;

    [Space(10)]

    public AnimationCurve fadeCurve;
    public float fadeSpeed = 3f;
    public int nbFadeiterations = 3;

    private void Start()
    {
        ShowBase(false);
    }


    public void ShowBase(bool visible)
    {
        StopAllCoroutines();

        if (meshesInChildren == null)
        {
            meshesInChildren = GetComponentsInChildren<MeshRenderer>();
        }

        for (int i = 0; i < meshesInChildren.Length; i++)
        {
            meshesInChildren[i].sharedMaterial = baseMat;
            Color c = meshesInChildren[i].material.GetColor("_Color");
            meshesInChildren[i].material.SetColor("_Color", new Color(c.r, c.g, c.b, visible ? 1f : 0f));
        }
    }

    public void ShowTransparent()
    {
        StopAllCoroutines();

        if (meshesInChildren == null)
        {
            meshesInChildren = GetComponentsInChildren<MeshRenderer>();
        }

        for (int i = 0; i < meshesInChildren.Length; i++)
        {
            meshesInChildren[i].material = transparentMat;
        }
    }


    public void ShowSurbrillance()
    {
        for (int i = 0; i < meshesInChildren.Length; i++)
        {
            meshesInChildren[i].material = surbrillanceMat;
        }

        StartCoroutine(ShowSurbrillanceCo());
    }





    private IEnumerator ShowSurbrillanceCo()
    {
        foreach (MeshRenderer mr in meshesInChildren)
        {
            Material mat = mr.material;

            float t = 0f;

            for (int i = 0; i < nbFadeiterations; i++)
            {
                while (t < 1f)
                {
                    t += Time.deltaTime * fadeSpeed;
                    mat.SetFloat("_RimWidth", fadeCurve.Evaluate(t));
                    yield return null;
                }

                while (t > 0f)
                {
                    t -= Time.deltaTime * fadeSpeed;
                    mat.SetFloat("_RimWidth", fadeCurve.Evaluate(t));
                    yield return null;
                }
            }

            for (int i = 0; i < meshesInChildren.Length; i++)
            {
                meshesInChildren[i].material = transparentMat;
            }

            yield return null;

        }
    }
}
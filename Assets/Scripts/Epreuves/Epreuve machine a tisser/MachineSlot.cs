using System.Collections;
using UnityEngine;

public class MachineSlot : MonoBehaviour
{
    [Space(10)]
    [Header("Scripts & Components : ")]
    [Space(10)]

    MeshRenderer[] meshesInChildren;
    public bool done = false;
    public int slotIndex;
    [Space(10)] public bool isInSurbrillance;


    [Space(10)]
    [Header("Materials : ")]
    [Space(10)]

    public Material transparentMat;
    public Material surbrillanceMat;

    Material[] baseMats;

    [Space(10)]

    public AnimationCurve fadeCurve;
    public float fadeSpeed = 3f;
    public int nbFadeiterations = 3;

    public void Start()
    {

        if (meshesInChildren == null)
        {
            meshesInChildren = GetComponentsInChildren<MeshRenderer>();
            baseMats = new Material[meshesInChildren.Length];
        }

        for (int i = 0; i < meshesInChildren.Length; i++)
        {
            baseMats[i] = meshesInChildren[i].material;
        }


        ShowBase(false);
    }






    public void ShowBase(bool visible)
    {
        StopAllCoroutines();

        //On a trouvé la pièce, on désactive son collider pour ne pas gêner les autres
        GetComponent<BoxCollider>().enabled = false;

        for (int i = 0; i < meshesInChildren.Length; i++)
        {
            meshesInChildren[i].sharedMaterial = baseMats[i];
            Color c = meshesInChildren[i].material.GetColor("_Color");
            meshesInChildren[i].material.SetColor("_Color", new Color(c.r, c.g, c.b, visible ? 1f : 0f));   //On laisse le changement de couleur si on a mis le shader transparent avant
            meshesInChildren[i].enabled = visible;

        }
    }

    public void ShowTransparent()
    {
        StopAllCoroutines();


        for (int i = 0; i < meshesInChildren.Length; i++)
        {
            meshesInChildren[i].enabled = true;
            meshesInChildren[i].material = transparentMat;
        }
    }


    public void ShowSurbrillance()
    {
        if (isInSurbrillance)
            return;

        isInSurbrillance = true;

        StopAllCoroutines();

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

        isInSurbrillance = false;

    }


}
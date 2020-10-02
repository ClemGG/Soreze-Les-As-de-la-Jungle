using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(Rigidbody))]
public abstract class Enemy : MonoBehaviour
{
    #region Variables

    [Space(10)]
    [Header("Scripts & Components : ")]
    [Space(10)]


    [HideInInspector] public Transform t;
    protected EpreuveKungfu epreuve;





    [Space(10)]
    [Header("IA : ")]
    [Space(10)]


    //Utilisé pour le script des langues pour savoir si l'ennemi est éligible pour la capture,
    //pour éviter les crashs
    protected bool notCaughtYet = true;
    [HideInInspector] public bool isRespawning, isGrowing, isVisible;
    [HideInInspector] public bool isCaught = false, isTargeted = false;




    #endregion



    

    // Use this for initialization
    protected virtual void Start()
    {
        t = transform;
        epreuve = (EpreuveKungfu)Epreuve.instance;
    }


    public virtual void DestroyThisEnemy()
    {
        gameObject.SetActive(false);
    }

    protected virtual IEnumerator ChangeDirection()
    {
        yield break;
    }



    public bool EligibleForDestruction()
    {
        //return isVisible && !isCaught && !isTargeted && !isRespawning & !isGrowing;
        return isVisible && !isCaught && !isTargeted;
    }



}

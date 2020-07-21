using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class WeakPoint : MonoBehaviour
{

    [Space(10)]
    [Header("Scripts & Components : ")]
    [Space(10)]

    [SerializeField] Transform weakPointPrefab;
    Transform t;
    BoxCollider bc;

    EpreuveKungfu epreuve;
    GameObject weakPointGO;

    [Space(10)]
    [Header("Weak point : ")]
    [Space(10)]

    public bool destroyed = false;
    public float startHealth = 1f;
    public float lifeLossPerSecond = .1f;

    float currentHealth;
    [HideInInspector] public int nbEnemiesOnThisWeakPoint = 0;
    bool shouldLooseLife = false;


    // Start is called before the first frame update
    void Start()
    {
        t = transform;
        bc = GetComponent<BoxCollider>();
        epreuve = (EpreuveKungfu)Epreuve.instance;

        bc.isTrigger = true;
        currentHealth = startHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (epreuve.EpreuveFinished)
            return;

        if (shouldLooseLife)
        {
            if(currentHealth > 0f)
            {
                currentHealth -= lifeLossPerSecond * Time.deltaTime;
            }
            else
            {
                if(!destroyed)
                    DestroyThisWeakPoint();
            }
        }
        else
        {
            if (currentHealth < startHealth)
            {
                currentHealth += lifeLossPerSecond * Time.deltaTime;
            }
        }
    }

    private void DestroyThisWeakPoint()
    {
        destroyed = true;
        bc.enabled = false;
        weakPointGO = ObjectPooler.instance.SpawnFromPool(weakPointPrefab.name, t.position, Quaternion.identity, epreuve.enemiesParent);
    }

    public void LooseLife()
    {
        nbEnemiesOnThisWeakPoint++;
        shouldLooseLife = true;
    }
    public void RegainLife()
    {
        nbEnemiesOnThisWeakPoint--;

        if (nbEnemiesOnThisWeakPoint == 0)
            shouldLooseLife = false;
    }


#if UNITY_EDITOR
    //Appelée quand le joueur réussit ou perd un niveau
    public void Reset()
    {
        shouldLooseLife = false;
        destroyed = false;
        bc.enabled = true;
        bc.isTrigger = true;
        currentHealth = startHealth;
        nbEnemiesOnThisWeakPoint = 0;

        if (weakPointGO)
            weakPointGO.SetActive(false);

    }

#endif
}

using System.Collections;
using UnityEngine;

public class Echafaudage : Enemy
{



    [Space(10)]
    [Header("Echafaudage : ")]
    [Space(10)]

    public int nbLives = 3;
    public bool isDead;




    //Marche aussi pour mobile
    private void OnMouseDown()
    {
        DestroyThisEnemy();
    }



    public override void DestroyThisEnemy()
    {
        if (epreuve.EpreuveFinished || isDead)
            return;

        nbLives--;

        if(nbLives == 0)
        {

            ObjectPooler.instance.SpawnFromPool("destruction echafaudage", transform.position, Quaternion.identity);

            AudioManager.instance.Play(epreuve.goodClip);
            AudioManager.instance.Play(epreuve.explosionBoisClip);
            AudioManager.instance.Play(epreuve.bambousClip);

            isDead = true;
            base.DestroyThisEnemy();
            epreuve.CheckVictory();

        }
        else
        {

            ObjectPooler.instance.SpawnFromPool("hit echafaudage", transform.position, Quaternion.identity);

            AudioManager.instance.Play(epreuve.boisClip);
            AudioManager.instance.Play(epreuve.frappeClip);
        }

    }
}

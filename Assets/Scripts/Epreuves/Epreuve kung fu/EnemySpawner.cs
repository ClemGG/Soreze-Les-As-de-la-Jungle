using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class SpawnPointAndTarget
{
    public Transform spawnPoint;
    public WeakPoint target;
}

public class EnemySpawner : MonoBehaviour
{

    [Space(10)]
    [Header("Scripts & Components : ")]
    [Space(10)]

    [SerializeField] SpawnPointAndTarget[] spawnPoints;
    ObjectPooler objectPooler;
    EpreuveKungfu epreuve;


    [Space(10)]
    [Header("Spawn : ")]
    [Space(10)]

    [SerializeField] bool spawnOnAllSpawnersAtOnce = false, spawnAtRandomPoint = false, spawnFourmi = false;
    [SerializeField] string enemyTag;
    [SerializeField] float spawnDelay, delayBeforeStart;
    float timer, startTimer;
    int currentSpawnPoint;

    // Start is called before the first frame update
    void Start()
    {
        objectPooler = ObjectPooler.instance;
        epreuve = (EpreuveKungfu)Epreuve.instance;
    }

    // Update is called once per frame
    void Update()
    {


        if (epreuve.EpreuveFinished || !AllowedToSpawn())
            return;


        if (startTimer < delayBeforeStart)
        {
            startTimer += Time.deltaTime;
        }
        else
        {



            if (timer < spawnDelay)
            {
                timer += Time.deltaTime;
            }
            else
            {
                timer = 0f;

                if (spawnOnAllSpawnersAtOnce)
                {
                    for (int i = 0; i < spawnPoints.Length; i++)
                    {
                        SpawnPointAndTarget st = spawnPoints[i];

                        if (spawnFourmi)
                        {
                            if (!st.target.destroyed)
                                objectPooler.SpawnFromPool(enemyTag, spawnPoints[i].spawnPoint.position, spawnPoints[i].spawnPoint.rotation, epreuve.enemiesParent);
                        }
                        else
                        {
                            objectPooler.SpawnFromPool(enemyTag, spawnPoints[i].spawnPoint.position, spawnPoints[i].spawnPoint.rotation, epreuve.enemiesParent);
                        }

                        if (spawnFourmi)
                            EpreuveKungfuStatic.nbFourmisInScene++;
                        else
                            EpreuveKungfuStatic.nbMoustiquesInScene++;

                    }
                }
                else
                {
                    if (spawnAtRandomPoint)
                    {
                        SpawnPointAndTarget st = null;

                        if (spawnFourmi)
                        {
                            List<SpawnPointAndTarget> sts = new List<SpawnPointAndTarget>();

                            for (int i = 0; i < spawnPoints.Length; i++)
                            {
                                if (!spawnPoints[i].target.destroyed)
                                {
                                    sts.Add(spawnPoints[i]);
                                }
                            }
                            int alea = UnityEngine.Random.Range(0, sts.Count);
                            st = sts[alea];
                        }
                        else
                        {
                            int alea = UnityEngine.Random.Range(0, spawnPoints.Length);
                            st = spawnPoints[alea];

                        }

                        objectPooler.SpawnFromPool(enemyTag, st.spawnPoint.position, st.spawnPoint.rotation, epreuve.enemiesParent);

                        if (spawnFourmi)
                            EpreuveKungfuStatic.nbFourmisInScene++;
                        else
                            EpreuveKungfuStatic.nbMoustiquesInScene++;
                    }
                    else
                    {
                        SpawnPointAndTarget st = null;

                        if (spawnFourmi)
                        {
                            while (st == null)
                            {

                                if (spawnPoints[currentSpawnPoint].target.destroyed)
                                {
                                    currentSpawnPoint++;

                                    if (currentSpawnPoint == spawnPoints.Length)
                                    {
                                        currentSpawnPoint = 0;
                                    }
                                }
                                else
                                {
                                    st = spawnPoints[currentSpawnPoint];

                                }
                            }
                        }
                        objectPooler.SpawnFromPool(enemyTag, spawnPoints[currentSpawnPoint].spawnPoint.position, spawnPoints[currentSpawnPoint].spawnPoint.rotation, epreuve.enemiesParent);


                        if (spawnFourmi)
                            EpreuveKungfuStatic.nbFourmisInScene++;
                        else
                            EpreuveKungfuStatic.nbMoustiquesInScene++;

                        currentSpawnPoint++;

                        if (currentSpawnPoint == spawnPoints.Length)
                        {
                            currentSpawnPoint = 0;
                        }
                    }

                }




            }
        }
    }

    private bool AllowedToSpawn()
    {
        return spawnFourmi ? EpreuveKungfuStatic.nbFourmisInScene < objectPooler.Pools[0].size : EpreuveKungfuStatic.nbMoustiquesInScene < objectPooler.Pools[1].size;
    }



#if UNITY_EDITOR

    private void OnDrawGizmos()
    {

        for (int i = 0; i < spawnPoints.Length; i++)
        {
            Gizmos.color = spawnFourmi ? Color.red : Color.green;
            Gizmos.DrawCube(spawnPoints[i].spawnPoint.position, Vector3.one * .2f);

            if (spawnPoints[i].target)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawCube(spawnPoints[i].target.transform.position, Vector3.one * .2f);
            }

        }
    }

#endif
}

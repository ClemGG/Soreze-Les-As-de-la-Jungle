using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class SpawnerMoustique : MonoBehaviour
{
    [Space(10)]
    [Header("Scripts & Components : ")]
    [Space(10)]

    public LayerMask obstacleMask;
    public Transform moustiquePrefab;

    BoxCollider col;
    Transform t;
    ObjectPooler op;
    EpreuveKungfu e;



    // Start is called before the first frame update
    IEnumerator Start()
    {
        e = (EpreuveKungfu)Epreuve.instance;
        col = GetComponent<BoxCollider>();
        t = transform;
        op = ObjectPooler.instance;

        while (e.EpreuveFinished)
        {
            yield return null;
        }


        StartCoroutine(Spawn());
    }

    // Update is called once per frame
    //void Update()
    //{
    //    print($"Nb ennemis : {FindObjectsOfType<Moustique>().Length}, nb Moustiques : {EpreuveKungfuStatic.nbMoustiquesInScene}");
    //}


    public IEnumerator Spawn()
    {
        bool restore = Physics.queriesHitBackfaces;
        for (int i = 0; i < op.Pools[1].size; i++)
        {
            Vector3 newPos;
            do
            {
                newPos = GetRandomPosInCube();
                yield return null;
            }
            while (Physics.BoxCast(newPos, Vector3.one, Vector3.zero, Quaternion.identity, 0f, obstacleMask));

            op.SpawnFromPool(moustiquePrefab.name, newPos, Quaternion.Euler(0f, Random.Range(0f, 180f), 0f));
            EpreuveKungfuStatic.nbMoustiquesInScene++;
            yield return null;
        }
        Physics.queriesHitBackfaces = restore;

    }
    public IEnumerator Respawn(Transform enemyToRespawn)
    {
        Vector3 newPos;
        do
        {
            newPos = GetRandomPosInCube();
            yield return null;
        }
        while (Physics.BoxCast(newPos, Vector3.one, Vector3.zero, Quaternion.identity, 0f, obstacleMask));

        enemyToRespawn.position = newPos;
        enemyToRespawn.rotation = Quaternion.Euler(0f, Random.Range(0f, 180f), 0f);
        yield return null;
    }

    private Vector3 GetRandomPosInCube()
    {
        return col.center + new Vector3(Random.Range(-col.size.x / 2f, col.size.x / 2f), 
                                        Random.Range(-col.size.y / 2f, col.size.y / 2f), 
                                        Random.Range(-col.size.z / 2f, col.size.z / 2f));
    }




#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        col = GetComponent<BoxCollider>();

        Gizmos.color = new Color(0,1,0,.1f);
        Gizmos.DrawCube(col.center, col.size);

    }
#endif
}

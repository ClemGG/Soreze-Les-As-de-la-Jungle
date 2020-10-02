using System.Collections;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class SpawnerMoustique : MonoBehaviour
{




    #region Variables

    [Space(10)]
    [Header("Scripts & Components : ")]
    [Space(10)]

    public LayerMask obstacleMask;
    public Transform moustiquePrefab;

    BoxCollider col;
    Transform t;
    ObjectPooler op;
    EpreuveKungfu e;


    #endregion





    #region Mono

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



    #endregion



    #region Spawn

    public IEnumerator Spawn()
    {
        bool restore = Physics.queriesHitBackfaces;
        for (int i = 0; i < op.Pools[0].size; i++)
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



    public void Respawn(Transform enemyToRespawn)
    {
        Vector3 newPos = GetRandomPosInCube();

        enemyToRespawn.position = newPos;
        enemyToRespawn.rotation = Quaternion.Euler(0f, Random.Range(0f, 180f), 0f);
    }



    private Vector3 GetRandomPosInCube()
    {
        Vector3 pos = col.center + t.position;
        Vector3 scale = Vector3.Scale(t.lossyScale, col.size) / 2f;
        
        return pos + new Vector3(Random.Range(-scale.x, scale.x), 
                                        Random.Range(-scale.y, scale.y), 
                                        Random.Range(-scale.z, scale.z));
    }


    #endregion



#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!t)
            t = transform;

        col = GetComponent<BoxCollider>();

        Vector3 pos = col.center + t.position;
        Vector3 scale = Vector3.Scale(t.lossyScale, col.size);

        Gizmos.color = new Color(0,1,0,.5f);
        Gizmos.DrawCube(pos, scale);

    }
#endif
}

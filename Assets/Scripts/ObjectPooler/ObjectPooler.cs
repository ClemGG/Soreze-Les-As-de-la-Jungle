using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour {



    [SerializeField] private Dictionary<string, Queue<GameObject>> poolDictionary;
    [SerializeField] private List<Pool> pools;

    public List<Pool> Pools { get => pools; set => pools = value; }


    #region Singleton
    public static ObjectPooler instance;

    private void Awake()
    {
        instance = this;
    }
    #endregion


#if UNITY_EDITOR

    private void OnValidate()
    {
        for (int i = 0; i < pools.Count; i++)
        {
            if(string.IsNullOrEmpty(pools[i].tag) && pools[i].prefab)
                pools[i].tag = pools[i].prefab.name;
        }
    }


#endif


    // Use this for initialization
    void Start () {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach(Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab, transform);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.tag, objectPool);
        }


	}


    public GameObject SpawnFromPool(string tag, Vector3 pos, Quaternion rot)
    {

        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Pool with tag \'" + tag + "\' doesn't exist.");
            return null;
        }
        GameObject obj = poolDictionary[tag].Dequeue();

        obj.transform.position = pos;
        obj.transform.rotation = rot;
        obj.SetActive(false);
        obj.SetActive(true);

        IPooledObject pooledObj = obj.GetComponent<IPooledObject>();

        if (pooledObj != null)
        {
            pooledObj.OnObjectSpawn();
        }

        poolDictionary[tag].Enqueue(obj);

        return obj;

    }


    public GameObject SpawnFromPool(string tag, Vector3 pos, Quaternion rot, Transform parent)
    {

        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Pool with tag \'" + tag + "\' doesn't exist.");
            return null;
        }
        GameObject obj = poolDictionary[tag].Dequeue();

        obj.transform.position = pos;
        obj.transform.rotation = rot;
        obj.transform.parent = parent;
        obj.SetActive(false);
        obj.SetActive(true);

        IPooledObject pooledObj = obj.GetComponent<IPooledObject>();

        if (pooledObj != null)
        {
            pooledObj.OnObjectSpawn();
        }

        poolDictionary[tag].Enqueue(obj);

        return obj;

    }




    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }
}

using UnityEngine;

public class Projectile : MonoBehaviour, IPooledObject
{

    [Space(10)]
    [Header("Scripts & Components : ")]
    [Space(10)]


    MeshRenderer rend;
    MeshFilter filter;
    Transform t;
    EpreuveNumeroArt e;
    Rigidbody rb;

    [Space(10)]
    [Header("Projectile : ")]
    [Space(10)]

    [Tooltip("Les apparences que pourra prendre le projectile.")]
    [SerializeField] ProjectileAppearance[] skins;

    [HideInInspector] public int ID;
    [HideInInspector] public Color projectileColor;
    public float speed = 10f;
    [HideInInspector] public Vector3 target;


    // Update is called once per frame
    void FixedUpdate()
    {
        t.position = Vector3.MoveTowards(t.position, target, Time.deltaTime * speed);
        t.LookAt(target);
    }


    public void SetProjectileColor(Color col, int index)
    {
        projectileColor = col;
        ID = index;

        filter.mesh = skins[index].mesh;
        rend.material.SetTexture("_MainTex", skins[index].mainTex);
    }


    public void OnObjectSpawn()
    {
        if (!t) t = transform;
        if (!rend) rend = t.GetChild(0).GetComponent<MeshRenderer>();
        if (!filter) filter = t.GetChild(0).GetComponent<MeshFilter>();
        if (!e) e = (EpreuveNumeroArt)Epreuve.instance;
        if(!rb) rb = GetComponent<Rigidbody>();

        rb.velocity = Vector3.zero;
        t.LookAt(target);

    }


    [System.Serializable]
    public struct ProjectileAppearance
    {
        public Mesh mesh;
        public Texture mainTex;
    }
}

using UnityEngine;

public class RandomProjectileRot : MonoBehaviour, IPooledObject
{
    [SerializeField] Vector2 minMaxRotSpeed = new Vector2(-5f, 5f);

    Transform t;
    bool rotX, rotY, rotZ;
    float spdX, spdY, spdZ;


    // Update is called once per frame
    void FixedUpdate()
    {
        if (!t) t = transform;


        if (rotX)
            t.Rotate(Vector3.right * spdX * Time.fixedDeltaTime);
        if (rotY)
            t.Rotate(Vector3.right * spdY * Time.fixedDeltaTime);
        if (rotZ)
            t.Rotate(Vector3.right * spdZ * Time.fixedDeltaTime);

    }


    public void OnObjectSpawn()
    {
        if (!t) t = transform;

        t.rotation = Random.rotation;

        rotX = Random.Range(0, 2) == 1 ? true : false;
        rotY = Random.Range(0, 2) == 1 ? true : false;
        rotZ = Random.Range(0, 2) == 1 ? true : false;

        if (!rotX & !rotY & !rotZ)
            rotX = true;

        spdX = Random.Range(minMaxRotSpeed.x, minMaxRotSpeed.y);
        spdY = Random.Range(minMaxRotSpeed.x, minMaxRotSpeed.y);
        spdZ = Random.Range(minMaxRotSpeed.x, minMaxRotSpeed.y);
    }

}

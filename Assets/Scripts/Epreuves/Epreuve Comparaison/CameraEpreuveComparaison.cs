
public class CameraEpreuveComparaison : GodMode
{
#if UNITY_EDITOR
    EpreuveComparaison e;

    protected override void Start()
    {
        e = (EpreuveComparaison)Epreuve.instance;
        base.Start();
    }


    protected override void Update()
    {
        //Si on tire junior, on bloque la caméra sur sa position actuelle
        if (e.isShooting)
        {
            transform.rotation = transform.rotation;
            transform.position = transform.position;
        }
        else
        {
            base.Update();
        }
    }
#endif
}


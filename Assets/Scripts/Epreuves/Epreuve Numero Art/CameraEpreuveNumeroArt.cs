public class CameraEpreuveNumeroArt : GodMode
{

#if UNITY_EDITOR || UNITY_STANDALONE
    EpreuveNumeroArt e;

    protected override void Start()
    {
        e = (EpreuveNumeroArt)Epreuve.instance;
        base.Start();
    }

#endif
}

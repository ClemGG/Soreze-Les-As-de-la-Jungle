using UnityEngine;


[System.Serializable]
public struct SpriteID
{
    public SpriteRenderer sprite;
    public int colorID;
    [HideInInspector] public SpriteNAS nasSprite;
}

public class NumeroArtSprite : MonoBehaviour
{


    [Space(10)]
    [Header("Colours : ")]
    [Space(10)]


    [HideInInspector] public bool done = false;
    public SpriteID[] sprites;
    //public AnimationCurve blinkCurve;

    private EpreuveNumeroArt epreuve;

    //One l'appelle plus depuis la Start. On l'appelle depuis le script de l'épreuve au moment où elle commence
    //por charger la scène plus vite
    private void Start()
    {
        epreuve = (EpreuveNumeroArt)Epreuve.instance;
        //sr = GetComponent<SpriteRenderer>();
        //sr.color = Color.white;
        //done = ID == -1;    //Si jamais il n'y a pas de couleur à ajouter et que le sprite doit rester blanc
        done = false;

        for (int i = 0; i < sprites.Length; i++)
        {
            sprites[i].sprite.color = new Color(0,0,0,0);
            sprites[i].nasSprite = sprites[i].sprite.GetComponent<SpriteNAS>();
            sprites[i].nasSprite.ID = sprites[i].colorID;
        }
    }


    public void AddColor(Color col, int index)
    {

        for (int i = 0; i < sprites.Length; i++)
        {
            if (index == sprites[i].colorID && !sprites[i].nasSprite.done)
            {
                sprites[i].sprite.color = col;
                sprites[i].nasSprite.done = true;
                epreuve.UpdateScoreUI(index);
            }

        }

        done = true;

        for (int i = 0; i < sprites.Length; i++)
        {
            if (!sprites[i].nasSprite.done)
            {
                done = false;
                break;
            }
        }

        
    }

    private void OnCollisionEnter(Collision c)
    {
        if (c.collider.TryGetComponent(out Projectile p))
        {
            NumeroArtSprite nas = c.collider.GetComponent<NumeroArtSprite>();
            Animator splashAnim = ObjectPooler.instance.SpawnFromPool("splash", p.transform.position, Quaternion.identity).GetComponent<Animator>();
            splashAnim.transform.GetChild(0).GetComponent<SpriteRenderer>().color = p.projectileColor;

            bool correct = false;
            for (int i = 0; i < sprites.Length; i++)
            {
                if (sprites[i].colorID == p.ID && !sprites[i].nasSprite.done)
                {
                    correct = true;
                    AudioManager.instance.Play(epreuve.goodClip);
                    epreuve.ResetHelpTimer();
                    break;
                }
            }
            splashAnim.Play(correct ? "correct" : "error");

            splashAnim.transform.GetChild(1).GetComponent<ParticleSystem>().startColor = p.projectileColor;
            splashAnim.transform.GetChild(1).GetChild(0).GetComponent<ParticleSystem>().startColor = p.projectileColor;



            AddColor(p.projectileColor, p.ID);
            epreuve.CheckVictory();

            p.gameObject.SetActive(false);
        }

    }


}

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EpreuveDifferences : Epreuve
{
    #region Variables

    [Space(10)]
    [Header("Scripts & Components : ")]
    [Space(10)]

    [SerializeField] GameObject cross;
    [SerializeField] GameObject correctParticle;
    [SerializeField] GameObject victoryParticle;

    [SerializeField] Button[] differences;

    [Space(10)]
    [Header("Differences : ")]
    [Space(10)]

    int differencesFound = 0;


    [Space(10)]
    [Header("Audio : ")]
    [Space(10)]

    [SerializeField] AudioClip goodClip;
    [SerializeField] AudioClip errorClip;
    [SerializeField] AudioClip victoryClip;
    [SerializeField] AudioClip crayonClip;


    #endregion





    #region Epreuve

    //Appelée par l'aide et les différences
    public void HighlightDifferences(int index)
    {
        if (EpreuveFinished)
            return;


        Vector3 v = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.WorldToScreenPoint(cross.transform.position).z);

        if (index == -1)
        {
            cross.SetActive(true);

            cross.transform.position = Camera.main.ScreenToWorldPoint(v);
            AudioManager.instance.Play(errorClip);
        }
        else
        {
            cross.SetActive(false); 


            differences[index].interactable = false; // Lance l'animation qui entoure les boutons

            differencesFound++;
            UpdateScoreUI();

            CheckVictory(v);

        }

        
    }

    //Vérifie si les conditions de victoire sont réunies
    private void CheckVictory(Vector3 v)
    {

        if (differencesFound == differences.Length)
        {
            OnEpreuveEnded(true);
            victoryParticle.SetActive(false);
            victoryParticle.SetActive(true);
            victoryParticle.transform.position = Camera.main.transform.position + Camera.main.transform.forward;

            AudioManager.instance.Play(victoryClip);
        }
        else
        {
            correctParticle.SetActive(false);
            correctParticle.SetActive(true);
            correctParticle.transform.position = Camera.main.ScreenToWorldPoint(v);

            AudioManager.instance.Play(goodClip);
            AudioManager.instance.Play(crayonClip);
        }
    }



    #endregion



    #region Overrides

    protected override IEnumerator Start()
    {



        //On setup le score et on active le sboutons des différences
        cross.SetActive(false);
        scoreText.text = differencesFound.ToString();
        finalScoreText.text = $" / {differences.Length}";


        for (int i = 0; i < differences.Length; i++)
        {
            differences[i].gameObject.SetActive(true);
            differences[i].interactable = true;

            yield return null;
        }


        yield return StartCoroutine(base.Start());



    }



    public override void GiveSolutionToPlayer(int index)
    {
        //Pour les deux premières aides, on fait pivoter la différence pour le le joueur la remarque
        if (index < 2)
        {

            for (int i = 0; i < differences.Length; i++)
            {
                if (differences[i].interactable)
                {
                    differences[i].GetComponent<Animator>().Play("show");
                    break;
                }
            }
        }
        //A la troisième, on entoure une différence que le joueur n'a pas déjà remarqué.
        //A la quatrième, on lui entoure les autres
        else
        {
            for (int i = 0; i < differences.Length; i++)
            {
                if (differences[i].interactable)
                {
                    HighlightDifferences(i);
                    ResetHelpTimer();

                    if (index == 2)
                        break;
                }
            }

            CheckVictory(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.WorldToScreenPoint(cross.transform.position).z));
        }
    }


    public override void UpdateScoreUI()
    {
        scoreText.text = differencesFound.ToString();
    }



    protected override void OnVictory()
    {
        base.OnVictory();
        Exit(false);
    }
    protected override void OnDefeat()
    {
        Exit(true);
    }
    #endregion
}

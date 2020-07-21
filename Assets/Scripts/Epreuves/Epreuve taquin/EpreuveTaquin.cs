using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EpreuveTaquin : Epreuve
{

    [Space(10)]
    [Header("Scripts & Components : ")]
    [Space(10)]

    public GameObject statsPanel;
    public GameObject previewBtn, previewImg, victoryImg;
    public TextMeshProUGUI nbMovesText, timeSpentText;


    [Space(10)]
    [Header("Puzzle : ")]
    [Space(10)]

    public DemoSlidingPuzzle puzzle;
    public DemoSlide puzzleStats;

    [Space(10)]

    public AnimationCurve blinkCurve;
    public float blinkSpeed = 3f;
    public int nbBlinkIterations = 2;

    [Space(10)]

    public float delayBeforeSendingHelp = 180f;
    float helpTimer;


    #region Epreuves


    public void DisplayVictory(int moves, float time)
    {
        puzzle.enabled = false;

        statsPanel.SetActive(true);
        victoryImg.SetActive(true);
        previewImg.SetActive(false);
        previewBtn.SetActive(false);

        nbMovesText.text = moves.ToString();
        timeSpentText.text = puzzleStats.DispTime();

        OnEpreuveEnded(true);
    }

    public void TogglePreviewImg()
    {
        previewImg.SetActive(!previewImg.activeSelf);
    }


    public IEnumerator BlinkColor(MeshRenderer m, int iterations, float speed)
    {
        Color @base = m.sharedMaterial.color;
        float t = 0f;

        for (int i = 0; i < iterations; i++)
        {

            while (t < 1f)
            {
                t += Time.deltaTime * speed;
                m.sharedMaterial.color = Color.Lerp(@base, Color.red, blinkCurve.Evaluate(t));
                yield return null;
            }


            while (t > 0f)
            {
                t -= Time.deltaTime * speed;
                m.sharedMaterial.color = Color.Lerp(@base, Color.red, blinkCurve.Evaluate(t));
                yield return null;
            }


            yield return null;
        }
    }



    #endregion






    #region Overrides

    protected override IEnumerator Start()
    {
        yield return StartCoroutine(base.Start());

        statsPanel.SetActive(false);
        victoryImg.SetActive(false);
        previewImg.SetActive(false);
        previewBtn.SetActive(false);

        yield return null;

    }


    protected override void Update()
    {
        if (EpreuveFinished)
            return;


        if (currentHelpIndex < nbHelp)
        {
            if (helpTimer < delayBeforeSendingHelp && !HelpPanelButtons.instance.btnCameleon.gameObject.activeSelf)
            {
                helpTimer += Time.deltaTime;
            }
            else
            {
                helpTimer = 0f;
                HelpPanelButtons.instance.btnCameleon.gameObject.SetActive(true);
            }
        }

    }

    public override void GiveSolutionToPlayer(int index)
    {
        switch (index)
        {
            case 1:
                List<GameObject> pieces = puzzle.GetAllIncorrectPieces();
                for (int i = 0; i < pieces.Count; i++)
                {
                    StartCoroutine(BlinkColor(pieces[i].GetComponent<MeshRenderer>(), nbBlinkIterations, blinkSpeed));
                }
                break;
            case 2:
                previewBtn.SetActive(true);
                break;
            case 3:
                puzzle.PuzzleSolved();
                break;
        }
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

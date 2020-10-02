using UnityEngine;

public class EpreuveTest : Epreuve
{

    public float delayBeforeSendingHelp = 10f;

    bool shouldCount = false;
    float timer, helpTimer = 0f;

    private void OnMouseDown()
    {
        if (EpreuveFinished)
            return;

        shouldCount = true;
    }

    private void OnMouseUp()
    {
        if (EpreuveFinished)
            return;

        shouldCount = false;
        OnEpreuveEnded(false);
    }

    protected override void Update()
    {
        if (shouldCount)
        {
            if(timer < 3f)
            {
                timer += Time.deltaTime;
            }
            else
            {
                timer = 0f;
                shouldCount = false;
                OnEpreuveEnded(true);
            }
        }

        if (currentHelpIndex < nbHelp && !EpreuveFinished)
        {
            if (helpTimer < delayBeforeSendingHelp && !HelpPanelButtons.instance.cameleon.gameObject.activeSelf)
            {
                helpTimer += Time.deltaTime;
            }
            else
            {
                helpTimer = 0f;
                HelpPanelButtons.instance.cameleon.gameObject.SetActive(true);

            }
        }
    }

    public override void GiveSolutionToPlayer(int index)
    {
        shouldCount = false;
        OnEpreuveEnded(true);
    }


    #region Overrides


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

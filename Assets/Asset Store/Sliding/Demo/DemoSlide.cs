using UnityEngine;
using System.Collections;

// this Demo is the main process class for the WyrmTale Jigsaw Puzzle Pack Demo
// this script was added to the Demo scene main Camera
public class DemoSlide : MonoBehaviour {
	
	// we will select all images and game objects that have to be accessed on the linked script
	// so we dant have to do lookups or put stuff in Resources
    public GameObject puzzle;

    DemoSlidingPuzzle slidingPuzzle = null;
    int puzzleImage = 1;
    int sizeMode = 1;

	// Use this for initialization
	void Start () {
        if (puzzle != null)
        {
			// puzzle was set so get linked DemoJigsawPuzzle class
            slidingPuzzle = puzzle.GetComponent("DemoSlidingPuzzle") as DemoSlidingPuzzle;
        }        
	}
	
    // translate Input mouseposition to GUI coordinates using camera viewport
    private Vector2 GuiMousePosition()
    {
        Vector2 mp = Input.mousePosition;
        Vector3 vp = Camera.main.ScreenToViewportPoint(new Vector3(mp.x, mp.y, 0));
        mp = new Vector2(vp.x * Camera.main.pixelWidth, (1 - vp.y) * Camera.main.pixelHeight);
        return mp;
    }
	


    void Restart()
    {
		// set puzzle top left piece so restart is forced
        slidingPuzzle.Restart();
    }
	
	// format time display string
	public string DispTime()
	{
        //Utilisé pour convertir le temps en minutes et secondes avant de l'afficher sur l'UI
        int min = (int)Mathf.Floor(slidingPuzzle.time / 60);
        int sec = (int)(slidingPuzzle.time % 60);

        if (sec == 60)
        {
            sec = 0;
            min++;
        }

        string minutes = min.ToString("0");
        string seconds = sec.ToString("00");

        if(min == 0)
        {
            return string.Format("{0} {1}", seconds, LocalizationManager.instance.GetLocalizedData("secondes"));
        }
        else if(min == 1)
        {
            return string.Format("{0} {1} {2} {3}", minutes, LocalizationManager.instance.GetLocalizedData("minute"), seconds, LocalizationManager.instance.GetLocalizedData("secondes"));
        }
        else
        {
            return string.Format("{0} {1}s {2} {3}", minutes, LocalizationManager.instance.GetLocalizedData("minute"), seconds, LocalizationManager.instance.GetLocalizedData("secondes"));
        }

    }



}

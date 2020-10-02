using System.IO;
using UnityEngine;

public class CameraPhotoScript : MonoBehaviour
{

    bool takeScreenShotOnNextFrame = false;
    string path;

    Texture2D screenShot;



    public void TakeScreenshot(string path)
    {
        takeScreenShotOnNextFrame = true;
        this.path = path;
    }


    private void OnPostRender()
    {
        if (takeScreenShotOnNextFrame)
        {
            TakePhoto();
        }
    }



    //Pour capturer la vue AR
    private void TakePhoto()
    {
        takeScreenShotOnNextFrame = false;

        DestroyImmediate(screenShot);

        //Get Image from screen
        screenShot = new Texture2D(Screen.width, Screen.height);
        screenShot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        screenShot.Apply();



        //Convert to jpeg and save
        byte[] imageBytes = screenShot.EncodeToJPG();
        File.WriteAllBytes(path, imageBytes);
    }
}

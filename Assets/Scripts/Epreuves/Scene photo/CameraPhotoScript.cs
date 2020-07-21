using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CameraPhotoScript : MonoBehaviour
{

    Camera camPhoto;
    bool takeScreenShotOnNextFrame = false;
    string path;

    public void TakeScreenshot(string path)
    {
        this.path = path;
        camPhoto = GetComponent<Camera>();
        camPhoto.targetTexture = RenderTexture.GetTemporary(Screen.width, Screen.height);
        takeScreenShotOnNextFrame = true;
    }


    private void OnPostRender()
    {
        if (takeScreenShotOnNextFrame)
        {
            takeScreenShotOnNextFrame = false;
            RenderTexture renderTexture = camPhoto.targetTexture;
            Texture2D renderResult = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.ARGB32, false);
            Rect rect = new Rect(0, 0, renderTexture.width, renderTexture.height);
            renderResult.ReadPixels(rect, 0, 0);

            byte[] byteArray = renderResult.EncodeToJPG();
            File.WriteAllBytes(path, byteArray);
            print("Screenshot saved");

            RenderTexture.ReleaseTemporary(renderTexture);
            camPhoto.targetTexture = null;
        }
    }
}

using System.Collections;
using NatSuite.Sharing;
using UnityEngine;

public class PrintSender : MonoBehaviour
{

    public static PrintSender instance;
    private void Awake()
    {
        if (instance)
        {
            Destroy(this);
            return;
        }

        instance = this;
    }



#if UNITY_EDITOR || UNITY_STANDALONE

    public IEnumerator PrintImage(string persistentDataPath, string fileName, int nbCopies)
    {

        //print(persistentDataPath);

        string desktop = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);
        SaveTempScreenshotStandalone(desktop, persistentDataPath, fileName);

        for (int i = 0; i < nbCopies; i++)
        {
            System.Diagnostics.Process.Start("mspaint.exe", "/pt " + desktop + "\\" + fileName);
            yield return null;
        }

    }


    public void SaveTempScreenshotStandalone(string desktopPath, string persistentDataPath, string fileName)
    {
        string filePathOnDesktop = desktopPath + "\\" + fileName;
        System.IO.File.WriteAllBytes(filePathOnDesktop, System.IO.File.ReadAllBytes(persistentDataPath));

        //print(filePathOnDesktop);
        //print(System.IO.File.Exists(streamingAssetsPath));
        //print(System.IO.File.Exists(filePathOnDesktop));
    }

#elif UNITY_IOS
    public void PrintImage(Texture2D photo)
    {
        //iosAirprintPlugin.PrintOut ();
        PrintPayload payload = new PrintPayload(true, true);
        payload.AddImage(photo).Commit();
    }
#endif







    }

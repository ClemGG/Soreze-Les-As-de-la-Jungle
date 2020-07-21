using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
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



//#if UNITY_EDITOR || UNITY_STANDALONE_WIN

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
//#elif UNITY_IOS
    public IEnumerator PrintImage(int nbCopies)
    {
        for (int i = 0; i < nbCopies; i++)
        {
		    iosAirprintPlugin.PrintOut ();
            yield return null;
        }


    }
//#endif


//#if UNITY_EDITOR || UNITY_STANDALONE_WIN


    public void SaveTempScreenshotStandalone(string desktopPath, string persistentDataPath, string fileName)
    {
        string filePathOnDesktop = desktopPath + "\\" + fileName;
        System.IO.File.WriteAllBytes(filePathOnDesktop, System.IO.File.ReadAllBytes(persistentDataPath));

        //print(filePathOnDesktop);
        //print(System.IO.File.Exists(streamingAssetsPath));
        //print(System.IO.File.Exists(filePathOnDesktop));
    }


//#elif UNITY_IOS
//    public void SaveTempScreenshotIpad(string persistentDataPath)
//    {


//    }
//#endif






    public void DeleteFile(string path)
    {

#if UNITY_EDITOR || UNITY_STANDALONE

        if (System.IO.File.Exists(path))
        {
            System.IO.File.Delete(path);
        }

#elif UNITY_IOS



#endif

    }
}
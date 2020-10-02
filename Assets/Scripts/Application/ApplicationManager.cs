using UnityEngine;
using UnityEngine.SceneManagement;

public static class ApplicationManager
{

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void OnAppStarted()
    {


        SetFrameRate(30);
        SetResoltuion(2048, 1536);
        Application.lowMemory += Application_lowMemory;
    }


    private static void Application_lowMemory()
    {
        CollectGarbage();
    }





    public static void SetResoltuion(int width, int height)
    {
        Screen.SetResolution(width, height, true);
        
    }

    public static void SetFrameRate(int newFrameRate)
	{
        Application.targetFrameRate = newFrameRate;
	}

    public static void CollectGarbage()
    {
        System.GC.Collect();
        Resources.UnloadUnusedAssets();
    }
}

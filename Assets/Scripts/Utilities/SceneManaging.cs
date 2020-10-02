using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace Clement.Utilities
{
    public static class SceneManaging
    {
        // This methos finds all objects of type T in Scene, excluding Prefabs:

        public static List<T> FindAllObjectsInSceneOfType<T>() where T : class
        {
            List<T> objectsInScene = new List<T>();

            T[] array = GameObject.FindObjectsOfType(typeof(T)) as T[];

            foreach (T @object in array)
            {
                objectsInScene.Add(@object);
            }



            return objectsInScene;
        }





        public static int CurrentLevelIndex()
        {
            return SceneManager.GetActiveScene().buildIndex;
        }

        public static string CurrentLevelName()
        {
            return SceneManager.GetActiveScene().name;
        }






        // This method finds all objects of type T in Scene, excluding Prefabs:
        public static List<T> SearchObjectsInSceneOfTypeIncludingDisabled<T>()
        {
            var ActiveScene = SceneManager.GetActiveScene();
            var RootObjects = ActiveScene.GetRootGameObjects();
            var MatchObjects = new List<T>();

            foreach (var ro in RootObjects)
            {
                var Matches = ro.GetComponentsInChildren<T>(true);
                MatchObjects.AddRange(Matches);
            }

            return MatchObjects;
        }
    }


   
}

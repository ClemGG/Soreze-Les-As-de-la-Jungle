using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartCoroutineTest : MonoBehaviour
{
    // Start is called before the first frame update
    IEnumerator Start()
    {
        int i = 0;
        while (i < 15)
        {
            yield return new WaitForSeconds(1f);
            print(i++);
            yield return new WaitForSeconds(1f);
            print(i++);
            yield return new WaitForSeconds(1f);
            print(i++);
            yield return new WaitForSeconds(1f);
            print(i++);
            yield return new WaitForSeconds(1f);
            print(i++);
            yield return new WaitForSeconds(1f);
            print(i++);
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnScrollBarEnabled : MonoBehaviour
{
    private void OnEnable()
    {
        GetComponent<Scrollbar>().value = 0f;

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Cette classe est appelée pour afficher les écrans de confirmations quand on quitte la scène ppale ou une épreuve

public class OnPanelQuitEnabled : MonoBehaviour
{


    private void OnEnable()
    {
        transform.GetChild(1).gameObject.SetActive(ScreenTransitionImageEffect.CurrentLevelIndex() == 1);
        transform.GetChild(2).gameObject.SetActive(ScreenTransitionImageEffect.CurrentLevelIndex() != 1);

    }
}

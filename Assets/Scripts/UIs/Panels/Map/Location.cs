using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Location : MonoBehaviour
{
    public int ID;   //Correspond à l'index de la scène de l'épreuve correspondante
    public bool unlocked = false;   //Permet de savoir si l'épreuve correspondante a été réussie ou non
    public Image img;    //L'icone de validation d'une oeuvre déjà étudiée par le joueur

}

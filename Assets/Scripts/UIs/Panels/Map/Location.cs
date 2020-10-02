using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Location : MonoBehaviour, IPointerClickHandler
{
    public int ID;   //Correspond à l'index de la scène de l'épreuve correspondante
    public bool unlocked = false;   //Permet de savoir si l'épreuve correspondante a été réussie ou non
    public Image img;    //L'icone de validation d'une oeuvre déjà étudiée par le joueur


    //Quand on clique sur le marqueur, afficher l'oeuvre correspondante
    //pour indiquer au joueur où se rendre
    public void OnPointerClick(PointerEventData eventData)
    {
        MapPanelButtons.instance.DisplayPreview(ID);
    }
}

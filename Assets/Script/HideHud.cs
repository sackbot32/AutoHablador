using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HideHud : MonoBehaviour
{
    public Sprite[] icons;
    //0 open
    //1 closed
    [SerializeField]            
    private GameObject[] toHide;
    private bool isHidden = false;
    public Image icon;
    public void HideUnhide()
    {
        isHidden = !isHidden;
        icon.sprite = isHidden ? icons[1] : icons[0];
        foreach (GameObject go in toHide)
        {
            go.SetActive(!isHidden);
        }
        
    }

}

using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ProjectReferenceForNew : MonoBehaviour
{
    public Image refImage;
    public Text projectName;
    public Button chooseButton;
    
    public void SetInfo(Sprite nRefImage,string nProjectName,UnityAction action)
    {
        refImage.sprite = nRefImage;
        projectName.text = nProjectName;
        chooseButton.onClick.AddListener(action);
    }
}

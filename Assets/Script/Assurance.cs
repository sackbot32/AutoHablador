using System;
using UnityEngine;
using UnityEngine.UI;

public class Assurance : MonoBehaviour
{
    public Text assuranceText;
    public Action acceptAction;

    public void SetUpBox(string textForAssurance, Action newAccept)
    {
        acceptAction = newAccept;
        assuranceText.text = textForAssurance;
    }

    public void Accept()
    {
        acceptAction.Invoke();
        Destroy(gameObject);
    }

    public void Cancel()
    {
        Destroy(gameObject);
    }
}

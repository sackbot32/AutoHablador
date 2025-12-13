using System;
using UnityEngine;
using UnityEngine.UI;

public class MultiTextInput : MonoBehaviour
{
    public Text label;
    public InputField inputField;
    public Action<string> onSubmit;
    
    public void Submit()
    {
        if(inputField.text.Length > 0)
        {
            if(onSubmit != null)
            {
                onSubmit.Invoke(inputField.text);
                Destroy(this.gameObject);
            }
        } else
        {
            //
            Loader.Instance.CreateNotif(Loader.Instance.GetLocalizedMessage("errorNotifLackTextForSubmit"), NotifType.Error);
        }
    }
}

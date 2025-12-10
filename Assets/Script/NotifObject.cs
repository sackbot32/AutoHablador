using System.IO;
using UnityEngine;
using UnityEngine.UI;


public enum NotifType
{
    Error,
    Warning,
    Success
}

public class NotifObject : MonoBehaviour
{
    [SerializeField]
    private Sprite[] icons;
    //0 error
    //1 warning
    //2 success
    public Image notifIcon;
    public Text infoText;
    public Text buttonText;
    public Text title;
    public void UpdateInfo(string titleInfo,string nInfo,string nBtnText,Color iconColor,Sprite nIcon = null)
    {
        if(nIcon != null)
        {
            notifIcon.sprite = nIcon;
        }
        title.text = titleInfo;
        notifIcon.color = iconColor;
        infoText.text = nInfo;
        buttonText.text = nBtnText;
    }

    public void UpdateInfo(string nInfo,string nBtnText,NotifType type)
    {
        int whichIcon = 0;
        switch (type)
        {
            case NotifType.Error:
                whichIcon = 0;
                notifIcon.color = Color.red;
                title.text = "Error";
                break;
            case NotifType.Warning:
                whichIcon = 1;
                notifIcon.color = Color.orange;
                title.text = "Warning";
                break;
            case NotifType.Success:
                whichIcon = 2;
                notifIcon.color = Color.green;
                title.text = "Success";
                break;
        }
        notifIcon.sprite = icons[whichIcon];
        notifIcon.GetComponent<RectTransform>().sizeDelta = new Vector2(icons[whichIcon].texture.width / 4f, icons[whichIcon].texture.height/4f);
        infoText.text = nInfo;
        buttonText.text = nBtnText;
    }
    
    public void CloseNotif()
    {
        Destroy(gameObject);
    }
}

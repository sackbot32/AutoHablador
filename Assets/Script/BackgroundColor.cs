using UnityEngine;
using UnityEngine.UI;

public class BackgroundColor : MonoBehaviour
{
    public Image backgroundImage;
    
    void Start()
    {
        Loader.Instance.bgColorControl = this;
        backgroundImage.color = new Color(PlayerPrefs.GetFloat("bgR"), PlayerPrefs.GetFloat("bgG"),PlayerPrefs.GetFloat("bgB"));
    }

    public void ChangeColor(float r,float g,float b)
    {
        backgroundImage.color = new Color(r,g,b);
    }
}

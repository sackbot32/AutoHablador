using UnityEngine;
using UnityEngine.UI;

public class ExpresionColorEditor : MonoBehaviour
{
    public Image colorShow;
    [SerializeField]
    private Slider[] colorFields;
    public Color colorButtonToSave;

    private void Start()
    {

    }

    public void UpdateColorInfo()
    {
        
        colorShow.color = ReturnColor();
        colorButtonToSave = ReturnColor();
    }

    public void LoadColor(Color newColor)
    {
        colorFields[0].value = newColor.r;
        colorFields[1].value = newColor.g;
        colorFields[2].value = newColor.b;
        UpdateColorInfo();
    }

    public Color ReturnColor()
    {
        return new Color(colorFields[0].value, colorFields[1].value, colorFields[2].value, 1);
    }
}

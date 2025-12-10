using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExpressionAddingButton : MonoBehaviour
{
    public int whichExpresion;
    public Text buttonName;
    public GameObject expresionPoint;
    public Color buttonColor;
    private Image buttonImage;
    private Button editButton;
    public List<ExpresionTimeSlider> sliders = new List<ExpresionTimeSlider>();

    private void Awake()
    {
        buttonName = transform.GetChild(0).GetComponent<Text>();
        buttonImage = GetComponent<Image>();
        editButton = transform.GetChild(1).GetComponent<Button>();
        editButton.onClick.AddListener(() => InfoSingleton.Instance.LoadExpresionForEditor(whichExpresion));
    }
    public void AddExpresion()
    {
        InfoSingleton.Instance.changer.AddExpresion(whichExpresion, InfoSingleton.Instance.currentTime);
        GameObject newPoint = Instantiate(expresionPoint,Vector2.zero,Quaternion.identity);
        newPoint.transform.SetParent(InfoSingleton.Instance.slidersContainer);
        newPoint.transform.localScale = Vector3.one;
        newPoint.GetComponent<RectTransform>().position = InfoSingleton.Instance.playerSlider.GetComponent<RectTransform>().position + new Vector3(0,204f,0);
        newPoint.GetComponent<RectTransform>().sizeDelta = InfoSingleton.Instance.playerSlider.GetComponent<RectTransform>().sizeDelta - new Vector2(0,0);
        newPoint.GetComponent<ExpresionTimeSlider>().UpdateExpresion(InfoSingleton.Instance.changer.timeExpresionList.Count -1, buttonColor, InfoSingleton.Instance.talker.characterExpresions[whichExpresion].expresionName);
       
        sliders.Add(newPoint.GetComponent<ExpresionTimeSlider>());
    }
    public ExpresionTimeSlider AddExpresion(float timeDuration)
    {
        InfoSingleton.Instance.changer.AddExpresion(whichExpresion, timeDuration);
        GameObject newPoint = Instantiate(expresionPoint,Vector2.zero,Quaternion.identity);
        newPoint.transform.SetParent(InfoSingleton.Instance.slidersContainer);
        newPoint.transform.localScale = Vector3.one;
        newPoint.GetComponent<RectTransform>().position = InfoSingleton.Instance.playerSlider.GetComponent<RectTransform>().position + new Vector3(0,204f,0);
        newPoint.GetComponent<RectTransform>().sizeDelta = InfoSingleton.Instance.playerSlider.GetComponent<RectTransform>().sizeDelta - new Vector2(0,0);
        newPoint.GetComponent<ExpresionTimeSlider>().UpdateExpresion(InfoSingleton.Instance.changer.timeExpresionList.Count -1, buttonColor, InfoSingleton.Instance.talker.characterExpresions[whichExpresion].expresionName);
        sliders.Add(newPoint.GetComponent<ExpresionTimeSlider>());
        return newPoint.GetComponent<ExpresionTimeSlider>();
    }

    public void ChangeInfo(int newExpresion, string newButtonName)
    {
        whichExpresion = newExpresion;
        buttonName.text = newButtonName;
    }

    public void CanDelete(bool can)
    {
        transform.GetChild(2).gameObject.SetActive(can);
    }

    public void UpdateColor(Color newColor)
    {
        buttonColor = newColor;
        buttonImage.color = newColor;
        float r = 1f - buttonColor.r;
        float g = 1f - buttonColor.g;
        float b = 1f - buttonColor.b;
        //buttonName.color = new Color(r,g,b);
        buttonName.color = new Color(r,g,b,1);
        foreach(ExpresionTimeSlider slider in sliders)
        {
            if(slider != null)
            {
                slider.UpdateExpresion(slider.expresionInstanceIndex, newColor,slider.expresionName);
            }
        }
    }

    public void CallDeleteExpresion()
    {
        Loader.Instance.CreateAssurance("Are you sure you want to delete this expression? ", DeleteExpression);
    }

    public void DeleteExpression()
    {
        InfoSingleton.Instance.talker.characterExpresions.RemoveAt(whichExpresion);
        if(InfoSingleton.Instance.talker.whichExpresion == whichExpresion)
        {
            InfoSingleton.Instance.talker.whichExpresion = 0;
        }
        transform.parent.gameObject.GetComponent<ExpresionButtonCreator>().CreateButtons();
    }
}

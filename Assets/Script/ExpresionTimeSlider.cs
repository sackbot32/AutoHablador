using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ExpresionTimeSlider : MonoBehaviour
{
    public int expresionInstanceIndex;
    [HideInInspector]
    public Slider expresionSlider;
    public Text expresionIndexText;
    [SerializeField]
    public Image imageKnob;
    public RemoveExpresionPoint pointRemover;
    public string expresionName;
    private void Start()
    {
        expresionSlider = GetComponent<Slider>();
        expresionSlider.maxValue = InfoSingleton.Instance.length;
        InfoSingleton.Instance.timeSliderList.Add(this);
        StartCoroutine(PrepareSlider());
    }

    public void ChangeInstanceTimePoint()
    {
        if(expresionInstanceIndex == 0)
        {
            expresionSlider.value = 0;
        }
        if(InfoSingleton.Instance.changer.timeExpresionList[expresionInstanceIndex] != null)
        {
            InfoSingleton.Instance.changer.timeExpresionList[expresionInstanceIndex].timeToStart = expresionSlider.value;
            InfoSingleton.Instance.ReorderTimeSliderList();
        }
    }
    public void ChangeInstanceTimePoint(float time)
    {
        if (expresionInstanceIndex == 0)
        {
            expresionSlider.value = 0;
        }
        if (InfoSingleton.Instance.changer.timeExpresionList[expresionInstanceIndex] != null)
        {
            InfoSingleton.Instance.changer.timeExpresionList[expresionInstanceIndex].timeToStart = time;
            expresionSlider.value = time;
            InfoSingleton.Instance.ReorderTimeSliderList();
        }
    }

    public void UpdateExpresion(int newIndex, Color newColor, string nName)
    {
        expresionName = nName;
        expresionInstanceIndex = newIndex;
        expresionIndexText.text = expresionInstanceIndex.ToString();
        imageKnob.color = newColor;
    }

    IEnumerator PrepareSlider()
    {
        while (InfoSingleton.Instance.length <= 0)
        {
            yield return null;
        }
        expresionSlider.minValue = 0;
        expresionSlider.maxValue = InfoSingleton.Instance.length;
        expresionSlider.value =  expresionInstanceIndex > 0 ? InfoSingleton.Instance.currentTime : 0;
    }

    public void RemoveExpresionTime()
    {
        pointRemover.RemovePoint();
    }
}

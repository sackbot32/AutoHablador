using UnityEngine;
using UnityEngine.EventSystems;

public class RemoveExpresionPoint : MonoBehaviour, IPointerClickHandler
{

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Right)
        {
            RemovePoint();
        }
    }

    public void RemovePoint()
    {
        InfoSingleton.Instance.deletingNode = true;
        int deletedIndex = transform.parent.transform.parent.gameObject.GetComponent<ExpresionTimeSlider>().expresionInstanceIndex;
        InfoSingleton.Instance.changer.RemoveExpresion(deletedIndex);
        InfoSingleton.Instance.timeSliderList.RemoveAt(deletedIndex);
        Destroy(transform.parent.transform.parent.gameObject);
        int n = 0;
        foreach (ExpresionTimeSlider timeSlide in InfoSingleton.Instance.timeSliderList)
        {
            if (timeSlide.expresionInstanceIndex > 0 && timeSlide.expresionInstanceIndex != deletedIndex)
            {

                int newIndex = timeSlide.expresionInstanceIndex;
                if (timeSlide.expresionInstanceIndex > deletedIndex)
                {
                    newIndex -= 1;
                }
                if (timeSlide != null)
                {
                    timeSlide.UpdateExpresion(newIndex, timeSlide.imageKnob.color);
                }
                if (timeSlide.expresionInstanceIndex == 0)
                {
                    timeSlide.expresionSlider.value = 0;
                }
            }
            else if (timeSlide.expresionInstanceIndex != deletedIndex)
            {
                if (timeSlide != null)
                {
                    timeSlide.UpdateExpresion(0, timeSlide.imageKnob.color);
                }
                timeSlide.expresionSlider.value = 0;
            }
            else
            {
                if (timeSlide != null)
                {
                    timeSlide.UpdateExpresion(timeSlide.expresionInstanceIndex - 1, timeSlide.imageKnob.color);
                }
            }
            n++;
        }
        InfoSingleton.Instance.deletingNode = false;
    }
}

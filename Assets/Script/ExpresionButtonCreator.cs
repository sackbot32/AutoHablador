using UnityEngine;
using UnityEngine.UI;

public class ExpresionButtonCreator : MonoBehaviour
{
    public GameObject buttonExpresionPrefab;

    void Start()
    {
        CreateButtons();
    }

    public void CreateButtons()
    {
        foreach (Transform children in transform)
        {
            foreach(ExpresionTimeSlider slider in children.GetComponent<ExpressionAddingButton>().sliders)
            {
                if(slider != null)
                {
                    CharacterPortraits por = InfoSingleton.Instance.talker.characterExpresions.Find(x => x.expresionName == slider.expresionName);
                    if (por == null)
                    {
                        slider.RemoveExpresionTime();
                    }
                }
            } 
            Destroy(children.gameObject);
        }
        bool canDelete = InfoSingleton.Instance.talker.characterExpresions.Count <= 1 ? false : true;
        for (int i = 0; i < InfoSingleton.Instance.talker.characterExpresions.Count; i++)
        {
            GameObject newButton = Instantiate(buttonExpresionPrefab, transform);
            newButton.GetComponent<ExpressionAddingButton>().ChangeInfo(i, InfoSingleton.Instance.talker.characterExpresions[i].expresionName);
            newButton.GetComponent<ExpressionAddingButton>().CanDelete(canDelete);
            newButton.GetComponent<ExpressionAddingButton>().UpdateColor(InfoSingleton.Instance.talker.characterExpresions[i].buttonColor);
            newButton.GetComponent<Image>().color = InfoSingleton.Instance.talker.characterExpresions[i].buttonColor;
            InfoSingleton.Instance.talker.characterExpresions[i].ownedButton = newButton.GetComponent<ExpressionAddingButton>();
        }
    }
}

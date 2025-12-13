using UnityEngine;

public class ExpresionEditor : MonoBehaviour
{

    public int CurrentExpresionEditing;
    public ExpresionColorEditor colorEditor;
    public ImageUnit shutImage;
    public ImageUnit talkImage;
    public EditExpresionName expresionNameEditor;
    public bool isCreatingNewExpresion;
    public ExpresionButtonCreator creator;
    private void Start()
    {
        InfoSingleton.Instance.editor = this;
    }

    public void LoadExpresion()
    {
        colorEditor.LoadColor(InfoSingleton.Instance.talker.characterExpresions[CurrentExpresionEditing].buttonColor);
        shutImage.LoadForEdit(InfoSingleton.Instance.talker.characterExpresions[CurrentExpresionEditing].shut);
        talkImage.LoadForEdit(InfoSingleton.Instance.talker.characterExpresions[CurrentExpresionEditing].talking);
        expresionNameEditor.LoadNameToEdit(InfoSingleton.Instance.talker.characterExpresions[CurrentExpresionEditing].expresionName);
    }

    public void ClearUpInfo()
    {
        colorEditor.LoadColor(new Color(1, 1, 1, 1));
        shutImage.LoadForEdit(null);
        talkImage.LoadForEdit(null);
        expresionNameEditor.LoadNameToEdit("");
    }

    public void SaveExpresion()
    {
        int whatExpresion = CurrentExpresionEditing;
        if (isCreatingNewExpresion)
        {
            InfoSingleton.Instance.talker.characterExpresions.Add(new CharacterPortraits());
            whatExpresion = InfoSingleton.Instance.talker.characterExpresions.Count - 1;
        }
        if (isAllDataToSave())
        {
            InfoSingleton.Instance.talker.characterExpresions[whatExpresion].buttonColor = colorEditor.ReturnColor();
            InfoSingleton.Instance.talker.characterExpresions[whatExpresion].shut = shutImage.SaveEdit();
            InfoSingleton.Instance.talker.characterExpresions[whatExpresion].talking = talkImage.SaveEdit();
            InfoSingleton.Instance.talker.characterExpresions[whatExpresion].expresionName = expresionNameEditor.SaveEditedName();
            if(InfoSingleton.Instance.talker.characterExpresions[whatExpresion].ownedButton != null)
            {
                InfoSingleton.Instance.talker.characterExpresions[whatExpresion].ownedButton.UpdateColor(colorEditor.ReturnColor());
                InfoSingleton.Instance.talker.characterExpresions[whatExpresion].ownedButton.ChangeInfo(CurrentExpresionEditing, expresionNameEditor.SaveEditedName());
            }
            if (isCreatingNewExpresion)
            {
                creator.CreateButtons();
            }
            gameObject.SetActive(false);
            
        } else
        {
            //
            Loader.Instance.CreateNotif(Loader.Instance.GetLocalizedMessage("errorNotifLackImgNName"), NotifType.Error, "OK");
        }
    }


    private bool isAllDataToSave()
    {
        if(colorEditor.ReturnColor() == null)
            return false;
        if(shutImage.SaveEdit() == null || talkImage.SaveEdit() == null) 
            return false;
        if(expresionNameEditor.SaveEditedName() == null)
            return false;
        return true;
    }
}

using UnityEngine;
using UnityEngine.UI;

public class EditExpresionName : MonoBehaviour
{

    public InputField nameInputField;


    public void LoadNameToEdit(string name)
    {
        nameInputField.text = name;
    }

    public string SaveEditedName()
    {
        return nameInputField.text;
    }
}

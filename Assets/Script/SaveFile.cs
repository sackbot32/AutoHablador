using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class SaveClass
{
    public string name;
    public List<CharacterPortraits> characterPortraits;
    public List<ExpresionTime> expTimes;
    public string audioPath;
    public string jsonPath;
}




public class SaveFile : MonoBehaviour
{

    static string filePath;

    public InputField nameField;
    public ExpresionButtonCreator buttonCreator;
    private void Awake()
    {
        filePath = Application.persistentDataPath;
    }
    public void SaveToJson()
    {
        SaveClass save = new SaveClass();
        save.name = nameField.text;

        Directory.CreateDirectory(filePath + "\\" + save.name);
        save.characterPortraits = InfoSingleton.Instance.talker.characterExpresions;
        foreach (CharacterPortraits invPor in save.characterPortraits)
        {
            if(invPor.shutImagePath.Length <= 0)
            {
                byte[] shutImageData = invPor.shut.texture.EncodeToPNG();
                string path = filePath + "\\" + save.name + "\\" + invPor.expresionName + "-shut.png";
                System.IO.File.WriteAllBytes(path, shutImageData);
                invPor.shutImagePath = path;

            }
            if (invPor.talkImagePath.Length <= 0)
            {
                byte[] talkImageData = invPor.talking.texture.EncodeToPNG();
                string path = filePath + "\\" + save.name + "\\" + invPor.expresionName + "-talk.png";
                System.IO.File.WriteAllBytes(path, talkImageData);
                invPor.talkImagePath = path;
            }
            
        }
        save.expTimes = InfoSingleton.Instance.changer.timeExpresionList;
        string audioPath = filePath + "\\" + save.name + "\\currentAudio.wav";
        SavWav.Save(audioPath, InfoSingleton.Instance.talker.source.clip);
        save.audioPath = audioPath;
        save.jsonPath = filePath + "\\" + save.name + "\\" + "saveOf" + save.name + ".json";
        string data = JsonUtility.ToJson(save);
        System.IO.File.WriteAllText(save.jsonPath, data);
    }


    public void LoadJson(string name)
    {
        if(System.IO.File.Exists(filePath + "\\" + name + "\\" + "saveOf" + name + ".json"))
        {
            //filePath + "\\" + name + "\\" + "saveOf" + name + ".json"
            string json = new StreamReader(filePath + "\\" + name + "\\" + "saveOf" + name + ".json").ReadToEnd();
            SaveClass load = JsonUtility.FromJson<SaveClass>(json);
            LoadNewAudio.instance.LoadAudioFromPath(load.audioPath);
            //Clear expresion for new ones
            InfoSingleton.Instance.talker.characterExpresions = new List<CharacterPortraits>();
            //Get expresions
            foreach (CharacterPortraits characterPortraits in load.characterPortraits)
            {
                Texture2D talk = ImageUnit.LoadImage(characterPortraits.talkImagePath);
                Texture2D shut = ImageUnit.LoadImage(characterPortraits.shutImagePath);
                characterPortraits.talking = Sprite.Create(talk, new Rect(0, 0, talk.width, talk.height), new Vector2(0, 0));
                characterPortraits.shut = Sprite.Create(shut, new Rect(0, 0, shut.width, shut.height), new Vector2(0, 0));
                InfoSingleton.Instance.talker.characterExpresions.Add(characterPortraits);
            }

            buttonCreator.CreateButtons();
            //Clear sliders for new ones

            foreach (CharacterPortraits toGetSliders in InfoSingleton.Instance.talker.characterExpresions)
            {
                foreach (ExpresionTimeSlider slider in toGetSliders.ownedButton.sliders)
                {
                    slider.RemoveExpresionTime();
                }
            }

            //And now we create new ones;
            StartCoroutine(PutSliders(load.expTimes));

        } else
        {
            //TODO create popup
            print("Doesnt exist ERROR CREATE POPUP at " + filePath + "\\" + name + "\\" + "saveOf" + name + ".json");
        }
    }

    IEnumerator PutSliders(List<ExpresionTime> loaded)
    {
        foreach (ExpresionTime time in loaded)
        {
            ExpresionTimeSlider slider = InfoSingleton.Instance.talker.characterExpresions[time.whichExpresion].ownedButton.AddExpresion(time.timeToStart);
            yield return new WaitForSeconds(0.01f);
            slider.ChangeInstanceTimePoint(time.timeToStart);
        }
        InfoSingleton.Instance.changer.timeExpresionList = loaded;
    }
}

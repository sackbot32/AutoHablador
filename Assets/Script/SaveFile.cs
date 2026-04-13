using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using System;

[Serializable]
public class SaveClass
{
    public string name;
    public float volumeSensitivity;
    public float[] volumeRange;
    //0 min 1 max
    public List<CharacterPortraits> characterPortraits;
    public List<ExpresionTime> expTimes;
    public string audioPath;
    public string jsonPath;
}




public class SaveFile : MonoBehaviour
{


    public string projectName;
    public ExpresionButtonCreator buttonCreator;
    public List<CharacterPortraits> defaultCharacterPortraits;

    private void Awake()
    {

    }
    public void SaveToJson()
    {
        SaveClass save = new SaveClass();
        save.name = projectName;

        Directory.CreateDirectory(Loader.Instance.saveFilePath + "\\" + save.name);
        save.characterPortraits = InfoSingleton.Instance.talker.characterExpresions;
        print($"MIN {InfoSingleton.Instance.minVol} MAX {InfoSingleton.Instance.maxVol} SENS {InfoSingleton.Instance.talker.volumeThreshold}");
        save.volumeSensitivity = InfoSingleton.Instance.talker.volumeThreshold;
        save.volumeRange = new float[2];
        save.volumeRange[0] = InfoSingleton.Instance.minVol;
        save.volumeRange[1] = InfoSingleton.Instance.maxVol;
        foreach (CharacterPortraits invPor in save.characterPortraits)
        {
            if(invPor != null)
            {
                string expresionPath = Loader.Instance.saveFilePath + "\\" + save.name + "\\" + invPor.expresionName;
                if (!System.IO.File.Exists(expresionPath))
                {
                    Directory.CreateDirectory(expresionPath);
                }
                if (invPor.shutImagePath == null)
                {
                    invPor.shutImagePath = "";
                }
                if (invPor.shutImagePath.Length <= 0)
                {
                    byte[] shutImageData = invPor.shut.texture.EncodeToPNG();
                    string path = expresionPath + "\\" + invPor.expresionName + "-shut.png";
                    System.IO.File.WriteAllBytes(path, shutImageData);
                    invPor.shutImagePath = path;

                }
                if (invPor.talkImagePath == null)
                {
                    invPor.talkImagePath = "";
                }
                if (invPor.talkImagePath.Length <= 0)
                {
                    byte[] talkImageData = invPor.talking.texture.EncodeToPNG();
                    string path = expresionPath + "\\" + invPor.expresionName + "-talk.png";
                    System.IO.File.WriteAllBytes(path, talkImageData);
                    invPor.talkImagePath = path;
                }
            }
            
            
        }
        save.expTimes = InfoSingleton.Instance.changer.timeExpresionList;
        //string audioPath = filePath + "\\" + save.name + "\\currentAudio.wav";
        save.audioPath = InfoSingleton.Instance.audioPath;
        save.jsonPath = Loader.Instance.saveFilePath + "\\" + save.name + "\\" + "saveOf" + save.name + ".json";
        string data = JsonUtility.ToJson(save);
        try
        {
            System.IO.File.WriteAllText(save.jsonPath, data);
        } catch (IOException ex)
        {
            print(ex);
        }
    }

    public void SavePortraitImages()
    {
        foreach (CharacterPortraits invPor in InfoSingleton.Instance.talker.characterExpresions)
        {
            if (invPor != null)
            {
                string expresionPath = Loader.Instance.saveFilePath + "\\" + projectName + "\\" + invPor.expresionName;
                if (!System.IO.File.Exists(expresionPath))
                {
                    Directory.CreateDirectory(expresionPath);
                }
                if (invPor.shutImagePath == null)
                {
                    invPor.shutImagePath = "";
                }
                if (invPor.shutImagePath.Length <= 0)
                {
                    byte[] shutImageData = invPor.shut.texture.EncodeToPNG();
                    string path = expresionPath + "\\" + invPor.expresionName + "-shut.png";
                    System.IO.File.WriteAllBytes(path, shutImageData);
                    invPor.shutImagePath = path;

                }
                if (invPor.talkImagePath == null)
                {
                    invPor.talkImagePath = "";
                }
                if (invPor.talkImagePath.Length <= 0)
                {
                    byte[] talkImageData = invPor.talking.texture.EncodeToPNG();
                    string path = expresionPath + "\\" + invPor.expresionName + "-talk.png";
                    System.IO.File.WriteAllBytes(path, talkImageData);
                    invPor.talkImagePath = path;
                }
            }
        }
    }
    public void LoadJson(string name)
    {
        if(System.IO.File.Exists(Loader.Instance.saveFilePath + "\\" + name + "\\" + "saveOf" + name + ".json"))
        {
            
            //filePath + "\\" + name + "\\" + "saveOf" + name + ".json"
            string json = new StreamReader(Loader.Instance.saveFilePath + "\\" + name + "\\" + "saveOf" + name + ".json").ReadToEnd();
            SaveClass load = JsonUtility.FromJson<SaveClass>(json);
            print($"MIN {load.volumeRange[0]} MAX {load.volumeRange[1]} SENS {load.volumeSensitivity}");

            InfoSingleton.Instance.talker.volumeThreshold = load.volumeSensitivity;
            InfoSingleton.Instance.talker.rangeLimits.UpdateFields(load.volumeRange[0], load.volumeRange[1]);
            InfoSingleton.Instance.talker.UpdateAudioSliders(load.volumeRange[0],load.volumeRange[1]);
            InfoSingleton.Instance.talker.volumeThresholdSlider.value = load.volumeSensitivity;
            //Loader.Instance.tasksForLoading.Add("loadAudio");
            InfoSingleton.Instance.audioLoader.LoadAudioFromPath(load.audioPath);
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
            if(load.expTimes.Count > 0)
            {
                StartCoroutine(PutSliders(load.expTimes));
            }
            projectName = name;
            Loader.Instance.tasksForLoading.Remove("loadJson");


        } else
        {
            //Loader.Instance.GetLocalizedMessage("errorNotifSaveFileNotFound")
            
            Loader.Instance.CreateNotif(Loader.Instance.GetLocalizedMessage("errorNotifSaveFileNotFound", new object[] { Loader.Instance.saveFilePath + "\\" + name + "\\" + "saveOf" + name + ".json" + " not found" }), NotifType.Error, "OK");
        }
    }

    public void NewProject(string name, string audioPath = "", string expresionJsonPath = "")
    {
        //Loader.Instance.tasksForLoading.Add("loadAudio");
        if (audioPath.Length <= 0)
        {
            
            InfoSingleton.Instance.audioLoader.LoadAudioFromPath(Loader.Instance.filePath + "/" + Loader.Instance.GetLocalizedMessage("defaultAudioFilePathEnd") + ".wav" );
            InfoSingleton.Instance.audioPath = Loader.Instance.filePath + "/" + Loader.Instance.GetLocalizedMessage("defaultAudioFilePathEnd") + ".wav";
        } else
        {
            InfoSingleton.Instance.audioLoader.LoadAudioFromPath(audioPath);
            InfoSingleton.Instance.audioPath = audioPath;
        }
            //Clear expresion for new ones
        InfoSingleton.Instance.talker.characterExpresions = new List<CharacterPortraits>();
        if(expresionJsonPath.Length <= 0)
        {
            //Get expresions
            foreach (CharacterPortraits characterPortraits in defaultCharacterPortraits)
            {
                InfoSingleton.Instance.talker.characterExpresions.Add(characterPortraits);
            }
        } else
        {
            string json = new StreamReader(expresionJsonPath).ReadToEnd();
            SaveClass load = JsonUtility.FromJson<SaveClass>(json);
            foreach (CharacterPortraits characterPortraits in load.characterPortraits)
            {
                Texture2D talk = ImageUnit.LoadImage(characterPortraits.talkImagePath);
                Texture2D shut = ImageUnit.LoadImage(characterPortraits.shutImagePath);
                characterPortraits.talking = Sprite.Create(talk, new Rect(0, 0, talk.width, talk.height), new Vector2(0, 0));
                characterPortraits.shut = Sprite.Create(shut, new Rect(0, 0, shut.width, shut.height), new Vector2(0, 0));
                InfoSingleton.Instance.talker.characterExpresions.Add(characterPortraits);
            }
        }
        InfoSingleton.Instance.talker.rangeLimits.UpdateFields(0, 0.1f);
        InfoSingleton.Instance.talker.UpdateAudioSliders(0, 0.1f);
        InfoSingleton.Instance.talker.volumeThreshold = 0.05f;
        InfoSingleton.Instance.talker.volumeThresholdSlider.value = 0.05f;
        buttonCreator.CreateButtons();
        projectName = name;
        Loader.Instance.tasksForLoading.Remove("newProject");
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
        InfoSingleton.Instance.timeSliderList[0].transform.position = InfoSingleton.Instance.timeSliderList[InfoSingleton.Instance.timeSliderList.Count - 1].transform.position;
    }

    

    public Task AsyncSave()
    {
        return Task.Run(() => SaveToJson());
    }

    public async void SaveAndQuitAsync()
    {
        await AsyncSave();
        QuitToMenu();
    }

    public void CallQuitToMenu()
    {
        //Loader.Instance.GetLocalizedMessage("assurQuitNoSave")
        Loader.Instance.CreateAssurance(Loader.Instance.GetLocalizedMessage("assurQuitNoSave"), QuitToMenu);
    }
    public void QuitToMenu()
    {
        StartCoroutine(QuitToMenuCoroutine());
    }
    public IEnumerator QuitToMenuCoroutine()
    {
        InfoSingleton.Instance = null;
        Loader.Instance.LoadingAnim(true,Loader.Instance.loadImageAnimDuration);
        yield return new WaitForSeconds(Loader.Instance.loadImageAnimDuration);
        AsyncOperation sceneLoad = SceneManager.LoadSceneAsync(0);

        while (sceneLoad.progress < 0.9f)
        {
            yield return null;
        }
        while (InfoSingleton.Instance != null)
        {
            print("LOADING");
            yield return null;
        }
        Loader.Instance.LoadingAnim(false);
    }
}

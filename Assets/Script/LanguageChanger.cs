using System.Collections;
using UnityEngine;
using UnityEngine.Identifiers;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class LanguageChanger : MonoBehaviour
{
    private Coroutine languageCor;
    private Dropdown ownDrop;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ownDrop = GetComponent<Dropdown>();
        if (PlayerPrefs.HasKey("currentLang"))
        {
            ownDrop.value = PlayerPrefs.GetInt("currentLang");
            ChangeLocale(PlayerPrefs.GetInt("currentLang"));
        } else
        {
            ownDrop.value = 0;
            ChangeLocale(0);
        }
    }

    public void ChangeLocale(int identifier)
    {
        if (languageCor == null)
        {
            ownDrop.interactable = false;
            languageCor = StartCoroutine(SetLocale(identifier));
        }
    }

    IEnumerator SetLocale(int identifier)
    {
        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[identifier];
        //TODO hacer que se creen los archivos de audio en todos los idiomas con sus respectivos nombres
        //Loader.Instance.filePath + "\\" + Loader.Instance.GetLocalizedMessage("defaultAudioFilePathEnd") + ".wav";
        string path = Loader.Instance.filePath + "\\" + Loader.Instance.GetLocalizedMessage("defaultAudioFilePathEnd") + ".wav";
        if (!System.IO.File.Exists(path))
        {
            //"DefaultAudioTable", "DefaultAudio"
            AudioClip clip = null;
            clip = new LocalizedAudioClip { TableReference = "DefaultAudioTable",TableEntryReference = "DefaultAudio", }.LoadAsset() ;
            SavWav.Save(path, clip);
        }
        PlayerPrefs.SetInt("currentLang", identifier);
        languageCor = null;
        ownDrop.interactable = true;
    }
}

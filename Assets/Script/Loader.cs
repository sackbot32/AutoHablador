using System;
using System.IO;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class Loader : MonoBehaviour
{
    static public Loader Instance;
    public bool debug = false;
    public bool isLoading = false;
    public GameObject multiTextPrefab;
    public GameObject assurancePrefab;
    public GameObject notifPrefab;
    public string filePath;
    public string saveFilePath;
    public Image loaderImage;
    public float loadImageAnimDuration = 0.05f;
    public Color defaultBack = Color.gray;
    public BackgroundColor bgColorControl;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (!PlayerPrefs.HasKey("bgR"))
        {
            PlayerPrefs.SetFloat("bgR", defaultBack.r);
            PlayerPrefs.SetFloat("bgG", defaultBack.g);
            PlayerPrefs.SetFloat("bgB", defaultBack.b);
        }
        if(Loader.Instance == null)
        {
            filePath = Application.persistentDataPath;
            saveFilePath = filePath + "\\Saves";
            if (!System.IO.File.Exists(saveFilePath))
            {
                Directory.CreateDirectory(saveFilePath);
            }
            Loader.Instance = this;
            DontDestroyOnLoad(this);
            LoadingAnim(false);
        } else
        {
            Destroy(this.gameObject);
        }
        //Debug
        if (debug)
        {
            OpenFile("testo");
        }
    }

    public void NewProyect(string name = "default", string audioPath = "", string jsonPath = "")
    {
        StartCoroutine(NewFileCoroutine(name, audioPath, jsonPath));
    }

    public void OpenFile(string whichFile)
    {
        StartCoroutine(OpenFileCoroutine(whichFile));
    }

    IEnumerator OpenFileCoroutine(string whichFile)
    {
        LoadingAnim(true, loadImageAnimDuration);
        yield return new WaitForSeconds(loadImageAnimDuration);
        AsyncOperation sceneLoad = SceneManager.LoadSceneAsync(1);
        isLoading = true;
        //InfoSingleton.Instance = null;
        while (sceneLoad.progress < 0.9f)
        {
            yield return null;
        }
        while(InfoSingleton.Instance == null)
        {
            print("LOADING");
            yield return null;
        }
        print("reach after instance");
        print(InfoSingleton.Instance.saveFile.name);
        InfoSingleton.Instance.saveFile.LoadJson(whichFile);
        isLoading=false;
        LoadingAnim(false);

    }

    IEnumerator NewFileCoroutine(string name,string audioPath,string jsonPath)
    {
        LoadingAnim(true, loadImageAnimDuration);
        yield return new WaitForSeconds(loadImageAnimDuration);
        AsyncOperation sceneLoad = SceneManager.LoadSceneAsync(1);
        isLoading = true;
        //InfoSingleton.Instance = null;
        while (sceneLoad.progress < 0.9f)
        {
            yield return null;
        }
        while (InfoSingleton.Instance == null)
        {
            print("LOADING");
            yield return null;
        }
        InfoSingleton.Instance.saveFile.NewProject(name,audioPath,jsonPath);
        isLoading = false;
        LoadingAnim(false);
    }

    public void CreateTextInput(string label,Action<string> submitAction)
    {
        MultiTextInput multi = Instantiate(multiTextPrefab, gameObject.transform).GetComponent<MultiTextInput>();
        multi.label.text = label;
        multi.onSubmit = submitAction;
    }

    public void CreateAssurance(string newText, Action acceptAction)
    {
        Assurance assu = Instantiate(assurancePrefab,gameObject.transform).GetComponent<Assurance>();
        assu.SetUpBox(newText, acceptAction);
    }

    public void CreateNotif(string boxInfo, NotifType type, string btnInfo = "OK")
    {
        NotifObject assu = Instantiate(notifPrefab, gameObject.transform).GetComponent<NotifObject>();
        assu.UpdateInfo(boxInfo,btnInfo,type);
    }

    public void LoadingAnim(bool on, float duration = 0.05f)
    {
        loaderImage.gameObject.SetActive(true);
        if (on)
        {
            DOTween.To(() => loaderImage.color, x => loaderImage.color = x, new Color(0, 0, 0, 1),duration);
        } else
        {
            DOTween.To(() => loaderImage.color, x => loaderImage.color = x, new Color(0, 0, 0, 0), duration).OnComplete(() => loaderImage.gameObject.SetActive(false));
        }
    }

    public void ChangeColor(float r,float g,float b)
    {
        PlayerPrefs.SetFloat("bgR", r);
        PlayerPrefs.SetFloat("bgG", g);
        PlayerPrefs.SetFloat("bgB", b);
        bgColorControl.ChangeColor(r, g, b);
    }
}

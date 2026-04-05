using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class NewProjectCreator : MonoBehaviour
{
    public InputField projectName;
    public string audioPath;
    public Text audioFileName;
    public string expresionJsonPath;
    public Text projectNameText;
    public Sprite defaultImage;


    [SerializeField]
    private GameObject projectButton;
    [SerializeField]
    private Transform projectListTransform;
    public void CreateNewProject()
    {
        Loader.Instance.NewProyect(projectName.text, audioPath, expresionJsonPath);
    }



    private void OnEnable()
    {
        ClearUpInfo();
        if(projectListTransform.childCount > 0)
        {
            projectListTransform.GetChild(0).gameObject.GetComponent<ProjectReferenceForNew>().SetInfo(defaultImage, Loader.Instance.GetLocalizedMessageFromTable("PrefabStringTable", "defaultProyect"), () => SelectJson("", "Default"));
        }
    }
    public void CallCreateProject()
    {
        if (projectName.text.Length > 0)
        {
            if(Loader.Instance.saveFilePath + "\\" + projectName.text + "\\" + "saveOf" + projectName.text + ".json" != expresionJsonPath){
                if (ProjectSelectorCreator.GetAllProjects().Contains(projectName.text))
                {
                    Loader.Instance.CreateAssurance(Loader.Instance.GetLocalizedMessage("assurOverride",new object[]{ projectName.text }), () =>
                    {
                        if (System.IO.Directory.Exists(Loader.Instance.saveFilePath + "\\" + projectName.text))
                        {
                            DirectoryInfo di = new DirectoryInfo(Loader.Instance.saveFilePath + "\\" + projectName.text);
                            foreach (FileInfo file in di.GetFiles())
                            {
                                file.Delete();
                            }
                            foreach (DirectoryInfo dir in di.GetDirectories())
                            {
                                dir.Delete(true);
                            }
                        }
                        CreateNewProject();
                    });
                }
                else
                {
                    CreateNewProject();
                }
            } else
            {
                Loader.Instance.CreateNotif(Loader.Instance.GetLocalizedMessage("notifOverideWarn"), NotifType.Warning, Loader.Instance.GetLocalizedMessage("under"));
            }
        } else
        {
            Loader.Instance.CreateNotif(Loader.Instance.GetLocalizedMessage("notifNameNeedWarn"), NotifType.Warning, Loader.Instance.GetLocalizedMessage("under"));
        }
    }

    private void Start()
    {
        CreateProjectList();
    }

    public void ClearUpInfo()
    {
        projectName.text = "";
        audioPath = "";

        audioFileName.text = Loader.Instance.GetLocalizedMessageFromTable("MainMenu", "chosenAudioText", new object[] { "Default.wav" });
        expresionJsonPath = "";
        projectNameText.text = Loader.Instance.GetLocalizedMessageFromTable("MainMenu", "chosenProjectReferenceText", new object[] { "Default" });
    }

    public void CreateProjectList()
    {
        List<string> projectNames = ProjectSelectorCreator.GetAllProjects();
        //TODO cambiar idioma
        Instantiate(projectButton, projectListTransform).GetComponent<ProjectReferenceForNew>().SetInfo(defaultImage, Loader.Instance.GetLocalizedMessageFromTable("PrefabStringTable", "defaultProyect"), () => SelectJson("", "Default"));
        foreach (string name in projectNames)
        {
            string path = Loader.Instance.saveFilePath + "\\" + name + "\\" + "saveOf" + name + ".json";
            string json = new StreamReader(path).ReadToEnd();
            SaveClass load = JsonUtility.FromJson<SaveClass>(json);
            Texture2D defaultImg = ImageUnit.LoadImage(load.characterPortraits[0].talkImagePath) ;
            Sprite defaultSpr = Sprite.Create(defaultImg, new Rect(0, 0, defaultImg.width, defaultImg.height), new Vector2(0, 0));
            Instantiate(projectButton, projectListTransform).GetComponent<ProjectReferenceForNew>().SetInfo(defaultSpr,name,() => SelectJson(path,name));
        }
    }

    public void SelectAudioPath()
    {
        //Stream stream;
        //System.Windows.Forms.OpenFileDialog sfd = new System.Windows.Forms.OpenFileDialog();
        //string title = "Select new audio";
        //sfd.Title = title;
        //sfd.Filter = "Audios (*.mp3;*.wav) | *.mp3; *.wav | All files (*.*)|*.*";
        //sfd.FilterIndex = 1;
        //if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        //{
        //    stream = sfd.OpenFile();
        //    if (stream != null)
        //    {
        //        FileStream file = stream as FileStream;
        //        if (file != null)
        //        {
        //            audioPath = file.Name;
        //            audioFileName.text = "Chosen audio:\n" + audioPath.Substring(audioPath.LastIndexOf("\\") +1);

        //        }
        //        stream.Close();
        //    }
        //}
        string openPath = FileBrowserStatic.OpenFile("Select new audio", "Audio (*.wav,*.mp3)|*.wav;*.mp3|All files (*.*)|*.*", 0);
        if(openPath != string.Empty)
        {
            audioPath = openPath;
            
            audioFileName.text = Loader.Instance.GetLocalizedMessageFromTable("MainMenu", "chosenAudioText", new object[] { (audioPath.Substring(audioPath.LastIndexOf("\\") + 1) )});
        }
    }

    public void SelectJson(string jsonPath,string projectName)
    {
        expresionJsonPath = jsonPath;
        //Loader.Instance.GetLocalizedMessage("chosenAudioText")
        
        projectNameText.text = Loader.Instance.GetLocalizedMessageFromTable("MainMenu", "chosenProjectReferenceText", new object[] { projectName });
    }
}

using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditorInternal.ReorderableList;

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

    public void CallCreateProject()
    {
        if (projectName.text.Length > 0)
        {
            if(Loader.Instance.saveFilePath + "\\" + projectName.text + "\\" + "saveOf" + projectName.text + ".json" != expresionJsonPath){
                if (ProjectSelectorCreator.GetAllProjects().Contains(projectName.text))
                {
                    Loader.Instance.CreateAssurance("Do you want to override the project "+ projectName.text + "? It will delete them", () =>
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
                Loader.Instance.CreateNotif("Can't overide reference project, select another or change name", NotifType.Warning, "I understand");
            }
        } else
        {
            Loader.Instance.CreateNotif("You need a name to create a project", NotifType.Warning, "I understand");
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
        audioFileName.text = "Chosen audio:\n" + "Default.wav";
        expresionJsonPath = "";
        projectNameText.text = "Expresion Reference:\n" + "Default";
    }

    public void CreateProjectList()
    {
        List<string> projectNames = ProjectSelectorCreator.GetAllProjects();
        Instantiate(projectButton, projectListTransform).GetComponent<ProjectReferenceForNew>().SetInfo(defaultImage, "Default", () => SelectJson("", "Default"));
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
            audioFileName.text = "Chosen audio:\n" + audioPath.Substring(audioPath.LastIndexOf("\\") + 1);
        }
    }

    public void SelectJson(string jsonPath,string projectName)
    {
        expresionJsonPath = jsonPath;
        projectNameText.text = "Expresion Reference:\n" + projectName;
    }
}

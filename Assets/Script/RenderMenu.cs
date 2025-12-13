using System.Collections;
using System.IO;
using Emgu.CV;
using UnityEngine;
using UnityEngine.UI;

public class RenderMenu : MonoBehaviour
{

    public string pathWithoutEnd;
    public string fileName;
    public InputField fileNameInput;
    public bool greenScreen;
    public bool usesAudio;
    public Toggle[] toggles;
    //0 = transparency
    //1 = audio
    public ExpresionColorEditor backColor;

    public CharRenderer charRenderer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void OnEnable()
    {
        SetGreenScreen(toggles[0].isOn);
        SetAudioRender(toggles[1].isOn);
    }


    public void SetGreenScreen(bool green)
    {
        greenScreen = !green;
    }

    public void SetAudioRender(bool auidi)
    {
        usesAudio = auidi;
    }

    public void SetName(string nName)
    {
        fileName = nName;
    }

    public void SetPath()
    {
        //Stream stream;
        string videoFileExtension = greenScreen ? "mp4" : "mov";
        //System.Windows.Forms.SaveFileDialog sfd = new System.Windows.Forms.SaveFileDialog();
        string title = "Select where to put your rendered gif";
        string defaultFileName = "";
        if (fileNameInput.text.Length <= 0)
        {
            defaultFileName = InfoSingleton.Instance.saveFile.projectName + "." + videoFileExtension;
        }
        else
        {
            defaultFileName = fileNameInput.text + "." + videoFileExtension;
        }
        //sfd.FileName = defaultFileName;
        //sfd.Title = title;
        //sfd.Filter = $"{videoFileExtension} (*.{videoFileExtension}) | *.{videoFileExtension}";
        //sfd.FilterIndex = 1;
        //if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        //{
        //    stream = sfd.OpenFile();
        //    if (stream != null)
        //    {
        //        FileStream file = stream as FileStream;
        //        if (file != null)
        //        {
        //            pathWithoutEnd = file.Name.Substring(0, file.Name.LastIndexOf("\\") + 1);
        //            print(pathWithoutEnd);
        //            string filedotted = file.Name.Substring(file.Name.LastIndexOf("\\") + 1).Substring(0);
        //            fileName = filedotted.Substring(0,filedotted.LastIndexOf('.'));
        //            fileNameInput.text = fileName;
        //        }
        //        stream.Close();
        //    }
        //}
        string savePath = FileBrowserStatic.SaveFile(title, $"{videoFileExtension}(*.{videoFileExtension})|*.{videoFileExtension}", defaultFileName, videoFileExtension);
        if(savePath != string.Empty && savePath != null)
        {
            pathWithoutEnd = savePath.Substring(0, savePath.LastIndexOf("\\") + 1);
            print(pathWithoutEnd);
            string filedotted = savePath.Substring(savePath.LastIndexOf("\\") + 1).Substring(0);
            fileName = filedotted.Substring(0, filedotted.LastIndexOf('.'));
            fileNameInput.text = fileName;
        }
    }

    public void StartRender()
    {
        if(pathWithoutEnd.Length > 0 && fileName.Length > 0)
        {
            string videoFileExtension = greenScreen ? "mp4" : "mov";
            string finalPath = pathWithoutEnd + fileName + "." + videoFileExtension;
            charRenderer.RenderByFiles(finalPath,backColor.colorButtonToSave,greenScreen,usesAudio);
        } else
        {
            
            if (pathWithoutEnd.Length <= 0)
            {
                //Loader.Instance.GetLocalizedMessage("errorNotifNoPath")
                Loader.Instance.CreateNotif(Loader.Instance.GetLocalizedMessage("errorNotifNoPath"), NotifType.Error, "OK");

                
            } else if(fileName.Length <= 0)
            {
                Loader.Instance.CreateNotif(Loader.Instance.GetLocalizedMessage("errorNotifFileName"), NotifType.Error, "OK");
            } else
            {
                Loader.Instance.CreateNotif("This error should not happen", NotifType.Error, "OK");
            }
        }
    }
}

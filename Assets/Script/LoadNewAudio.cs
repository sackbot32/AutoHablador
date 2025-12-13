using UnityEngine;
using System.IO;
using UnityEngine.Networking;
using System.Collections;
using System.Windows;
using System.Threading.Tasks;

public class LoadNewAudio : MonoBehaviour
{
    public static LoadNewAudio instance;
    private void Awake()
    {
        instance = this;
    }
    public void LoadAudio()
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
        //            StartCoroutine(LoadAudioFile(file.Name));

        //        }
        //        stream.Close();
        //    }
        //}
        string openPath = FileBrowserStatic.OpenFile("Select new audio", "Audio (*.wav,*.mp3)|*.wav;*.mp3|All files (*.*)|*.*", 0);
        if (openPath != string.Empty)
        {
            StartCoroutine(LoadAudioFile(openPath));
        }
    }

    public void CallLoadAudio()
    {
        //Loader.Instance.GetLocalizedMessage("assurAudioChange")
        Loader.Instance.CreateAssurance(Loader.Instance.GetLocalizedMessage("assurAudioChange"), LoadAudio);
    }

    public void LoadAudioFromPath(string path) 
    {
        StartCoroutine(LoadAudioFile(path));
    }



    public void SubtitubeAudio(AudioClip clip)
    {
        InfoSingleton.Instance.talker.source.clip = clip;
        
        InfoSingleton.Instance.audioPlayerController.UpdateDurationSlider();
    }

    IEnumerator LoadAudioFile(string path)
    {
        AudioType selectedType = AudioType.MPEG;
        int pointPos = path.LastIndexOf(".");
        print(path.Substring(pointPos));
        if(path.Substring(pointPos).ToLower() == ".wav")
        {
            print("Is wav");
            selectedType = AudioType.WAV;
        }
        UnityWebRequest audioData = UnityWebRequestMultimedia.GetAudioClip(path, selectedType);
        yield return audioData.SendWebRequest();
        if (audioData.result == UnityWebRequest.Result.Success)
        {
            AudioClip audioFile = DownloadHandlerAudioClip.GetContent(audioData);
            if (audioFile != null)
            {
                InfoSingleton.Instance.audioPath = path;
                SubtitubeAudio(audioFile);
            }
        }

        
        
    }
}

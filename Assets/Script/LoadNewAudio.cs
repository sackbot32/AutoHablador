using System.Collections;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Networking;

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

        float[] AudioSamples = new float[1024];
        float min = float.PositiveInfinity;
        float max = float.NegativeInfinity;
        print("ne valu" + clip.samples);
        for (int i = 0; i < clip.samples; i++)
        {
            float newValue = 0;
            clip.GetData(AudioSamples, i);
            foreach (float sample in AudioSamples)
            {
                newValue += Mathf.Abs(sample);
            }
            newValue /= 1024;
            newValue *= InfoSingleton.Instance.talker.valueMultiplier;

            
            if (newValue < 0.001f * InfoSingleton.Instance.talker.valueMultiplier)
            {
                newValue = 0f;
            }

            if (newValue < min)
            {
                min = newValue;
            }
            if(newValue > max)
            {
                max = newValue;
            }
        }

        InfoSingleton.Instance.maxVol = max;
        InfoSingleton.Instance.minVol = min;
        InfoSingleton.Instance.talker.UpdateAudioSliders(InfoSingleton.Instance.minVol, InfoSingleton.Instance.maxVol);
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
        print(audioData.result);
        if (audioData.result == UnityWebRequest.Result.Success)
        {
            print("audio for loading");
            AudioClip audioFile = DownloadHandlerAudioClip.GetContent(audioData);
            if (audioFile != null)
            {
                
                InfoSingleton.Instance.audioPath = path;
                SubtitubeAudio(audioFile);
            }
        }

        
        
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using FFMpegCore.Pipes;
using NUnit.Framework;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.VisualScripting;
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
        string openPath = FileBrowserStatic.OpenFile("Select new audio", "Audio (*.wav,*.mp3)|*.wav;*.mp3|All files (*.*)|*.*", 0);
        if (openPath != string.Empty)
        {
            //int pos = Mathf.Clamp(Loader.Instance.tasksForLoading.Count - 1, 0, Loader.Instance.tasksForLoading.Count - 1);
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
        int pos = Mathf.Clamp(Loader.Instance.tasksForLoading.Count - 1, 0, Loader.Instance.tasksForLoading.Count - 1);
        StartCoroutine(LoadAudioFile(path));
    }



    public void SubtitubeAudio(AudioClip clip)
    {

        //StartCoroutine(getAudioVolumeRange(clip));
        InfoSingleton.Instance.talker.source.clip = clip;
        
        InfoSingleton.Instance.audioPlayerController.UpdateDurationSlider();
    }

    IEnumerator getAudioVolumeRange(AudioClip clip)
    {
        float[] AudioSamples = new float[1024];
        float min = float.PositiveInfinity;
        float max = float.NegativeInfinity;
        float newValue = 0;
        List<float> valueList = new List<float>();
        for (int i = 0; i < clip.samples; i++)
        {
            print("which sample are you on: " + i + " out of " + clip.samples);
            newValue = 0;
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
            valueList.Add(newValue);
            yield return null;
        }
        int n = 0;
        foreach (float value in valueList)
        {
            print("which value are you on: " + n + " out of " + valueList.Count);
            n++;

            if (value < min)
            {
                min = value;
            }
            if (value > max)
            {
                max = value;
            }
            yield return null;
        }

        InfoSingleton.Instance.maxVol = max;
        InfoSingleton.Instance.minVol = min;
        InfoSingleton.Instance.talker.UpdateAudioSliders(InfoSingleton.Instance.minVol, InfoSingleton.Instance.maxVol);
        Loader.Instance.tasksForLoading.Remove("loadAudio");
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

using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class InfoSingleton : MonoBehaviour
{
    public static InfoSingleton Instance;
    public float length;
    public float currentTime;
    public ExpresionChanger changer;
    public Talker talker;
    public Transform timelineTransform;
    public Transform slidersContainer;
    public List<ExpresionTimeSlider> timeSliderList;
    public ExpresionEditor editor;
    public Slider playerSlider;
    public SaveFile saveFile;
    public LoadNewAudio audioLoader;
    public AudioPlayerController audioPlayerController;
    public string audioPath;
    public bool deletingNode = false;
    public float minVol = 0;
    public float maxVol = 0;
    private void Awake()
    {
        Instance = this;
        timeSliderList = new List<ExpresionTimeSlider>();
        editor.gameObject.SetActive(false);
        
    }

    public void LoadExpresionForEditor(int whichExpresionToEdit)
    {
        editor.isCreatingNewExpresion = false;
        editor.gameObject.SetActive(true);
        editor.CurrentExpresionEditing = whichExpresionToEdit;
        editor.LoadExpresion();
    }

    public void OpenMenuForNewExpresion()
    {
        editor.isCreatingNewExpresion = true;
        editor.ClearUpInfo();
        editor.gameObject.SetActive(true);
    }

    public void ReorderTimeSliderList()
    {
        if (!deletingNode)
        {
            List<ExpresionTimeSlider> newTimeSlide = new List<ExpresionTimeSlider>();
            newTimeSlide = timeSliderList.OrderBy(slider => InfoSingleton.Instance.changer.timeExpresionList[slider.expresionInstanceIndex].timeToStart).ToList();
            timeSliderList = newTimeSlide;
            List<ExpresionTime> newTime = new List<ExpresionTime>();
            newTime = InfoSingleton.Instance.changer.timeExpresionList.OrderBy(expTime => expTime.timeToStart).ToList();
            InfoSingleton.Instance.changer.timeExpresionList = newTime;
            int n = 0;
            foreach (ExpresionTimeSlider reorderedSlider in timeSliderList)
            {
                reorderedSlider.UpdateExpresion(n, reorderedSlider.imageKnob.color, reorderedSlider.expresionName);
                n++;
            }
        }
        
        


    }

}

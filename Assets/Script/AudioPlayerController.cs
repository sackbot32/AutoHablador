using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioPlayerController : MonoBehaviour
{
    public AudioSource sourceToControl;
    public Slider durationShow;

    public Image playIcon;

    public List<Sprite> icons;
    //0 Play
    //1 Pause
    //2 Restart

    //dData
    private float currentTime;
    private float currentAudioDuration;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InfoSingleton.Instance.audioPlayerController = this;
        UpdateDurationSlider();

    }

    // Update is called once per frame
    void Update()
    {
        if (sourceToControl.isPlaying) 
        {
            currentTime = sourceToControl.time;
            currentAudioDuration = sourceToControl.clip.length - 0.05f;
            durationShow.value = currentTime;
        }
        if (!sourceToControl.isPlaying)
        {
            currentTime = durationShow.value;
        }
        if (currentTime >= currentAudioDuration && !sourceToControl.isPlaying)
        {
            //print("End reached");
            sourceToControl.Stop();
            playIcon.sprite = icons[2];
        }

        if(!sourceToControl.isPlaying && currentTime < currentAudioDuration)
        {
            playIcon.sprite = icons[0];
        }

        InfoSingleton.Instance.length = currentAudioDuration;
        InfoSingleton.Instance.currentTime = currentTime;

    }

    public void PlayStop()
    {
        if (!sourceToControl.isPlaying)
        {
            sourceToControl.Play();
            playIcon.sprite = icons[1];
            
        } else
        {   
            if (sourceToControl.time != sourceToControl.clip.length)
            {
                sourceToControl.Pause();
                playIcon.sprite = icons[0];
            }
            else if(currentTime >= currentAudioDuration)
            {
                sourceToControl.Stop();
                sourceToControl.Play();
            }
        }
    }

    public void UpdateDurationSlider()
    {
        durationShow.minValue = 0;
        currentAudioDuration = sourceToControl.clip.length - 0.05f;
        InfoSingleton.Instance.length = sourceToControl.clip.length;
        currentTime = 0;
        durationShow.maxValue = currentAudioDuration;
        playIcon.sprite = icons[0];
        if(InfoSingleton.Instance.timeSliderList.Count > 0)
        {
            foreach (ExpresionTimeSlider item in InfoSingleton.Instance.timeSliderList)
            {
                item.expresionSlider.maxValue = currentAudioDuration;
            }
        }
    }
}

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioPlayerController : MonoBehaviour
{
    public AudioSource sourceToControl;
    public Slider durationShow;
    public Text timeText;
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
            SetTime();
        }
        if (!sourceToControl.isPlaying)
        {
            currentTime = durationShow.value;
        }
        if (currentTime >= (currentAudioDuration - 0.05f) && sourceToControl.isPlaying)
        {
            print("End reached");
            playIcon.sprite = icons[2];
            sourceToControl.Stop();
        }
        

        if(!sourceToControl.isPlaying && currentTime < currentAudioDuration)
        {
            playIcon.sprite = icons[0];
        }

        if (currentTime >= (currentAudioDuration - 0.05f) && !sourceToControl.isPlaying)
        {
            playIcon.sprite = icons[2];
        }
        if (InfoSingleton.Instance != null)
        {
            InfoSingleton.Instance.length = currentAudioDuration;
            InfoSingleton.Instance.currentTime = currentTime;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PlayStop();
        }
    }

    public void SetTime()
    {

        int fullLenghtHour = 0;
        int fullLenghtMinute = 0;
        int fullLenghtSecond = 0;
        fullLenghtHour = Mathf.RoundToInt (InfoSingleton.Instance.length / 3600f);
        fullLenghtMinute = ((int)InfoSingleton.Instance.length - fullLenghtHour)/60;
        fullLenghtSecond = ((int)InfoSingleton.Instance.length - fullLenghtHour)%60;


        int currentHour = 0;
        int currentMinute = 0;
        int currentSecond = 0;
        currentHour = Mathf.RoundToInt(InfoSingleton.Instance.currentTime / 3600f);
        currentMinute = ((int)InfoSingleton.Instance.currentTime - currentHour) / 60;
        currentSecond = ((int)InfoSingleton.Instance.currentTime - currentHour) % 60;


        string fullLenghtTimeText = "";
        string currentTimeText = "";
        if(fullLenghtHour > 0)
        {
            fullLenghtTimeText = fullLenghtHour.ToString("00") + ":" + fullLenghtMinute.ToString("00") + ":" + fullLenghtSecond.ToString("00");
            currentTimeText = currentHour.ToString("00") + ":" + currentMinute.ToString("00") + ":" + currentSecond.ToString("00");
        } else
        {
            fullLenghtTimeText = fullLenghtMinute.ToString("00") + ":" + fullLenghtSecond.ToString("00");
            currentTimeText = currentMinute.ToString("00") + ":" + currentSecond.ToString("00");
        }

        timeText.text = currentTimeText + "/" + fullLenghtTimeText;
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
        print("Duracion " + sourceToControl.clip.length);
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
        SetTime();
    }
}

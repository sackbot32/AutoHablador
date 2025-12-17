using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.UI;

[Serializable]
public class CharacterPortraits
{
    public string expresionName;
    public Color buttonColor;
    public Sprite shut;
    public string shutImagePath;
    public Sprite talking;
    public string talkImagePath;
    public ExpressionAddingButton ownedButton;
}
public class Talker : MonoBehaviour
{
    public Image characterShowing;
    public int whichExpresion;
    public Image background;
    public List<CharacterPortraits> characterExpresions;
    [Range(0f, 1f)]
    public float volumeThreshold;
    public Slider volumeThresholdSlider;
    public AudioSource source;
    public Slider showLoudnessSlider;
    public AudioMixer mixer;
    float[] samples;
    public float valueMultiplier = 1;
    public float currentValue;
    public float characterSize = 1f;
    public SetAudioRangeLimits rangeLimits;

    private Color sliderColor;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        samples = new float[1024];
        StartCoroutine(GetInfo());
        InfoSingleton.Instance.talker = this;
    }

    public void SetVolumeThreshold(float nThreshoold)
    {
        volumeThreshold = nThreshoold;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentValue > volumeThreshold)
        {
            characterShowing.sprite = characterExpresions[whichExpresion].talking;
        } else
        {
            characterShowing.sprite = characterExpresions[whichExpresion].shut;
        }

        float factorOfChange = characterShowing.sprite.texture.height / 540f;
        float height = characterShowing.sprite.texture.height/factorOfChange;
        float width = characterShowing.sprite.texture.width/factorOfChange;
        characterShowing.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);

        if (Input.GetKeyDown(KeyCode.P))
        {
            print("Current playback pos " + source.timeSamples);
        }
    }

    private float GetCurrentVolume()
    {
        float newValue = 0;
        source.clip.GetData(samples, source.timeSamples);
        foreach (float sample in samples)
        {
            newValue += Mathf.Abs(sample);
        }
        newValue /= 1024;
        newValue *= valueMultiplier;

        if (newValue < 0.001f * valueMultiplier)
        {
            newValue = 0f;
        }

        return newValue;

    }

    public Texture2D ReturnCurrentCharImage()
    {
        return currentValue > 0 ? characterExpresions[whichExpresion].talking.texture : characterExpresions[whichExpresion].shut.texture;
    }

    public Texture2D ReturnCharImageOnPPos(int pPos)
    {

        float pPosVolume = 0;
        pPosVolume = GetVolumeOnPPos(pPos);
        int expresionOnPPos = InfoSingleton.Instance.changer.ReturnExpresionOnTime(pPos/ (float)source.clip.frequency);
        return pPosVolume > volumeThreshold ? characterExpresions[expresionOnPPos].talking.texture : characterExpresions[expresionOnPPos].shut.texture;
    }

    private float GetVolumeOnPPos(int pPos)
    {
        float newValue = 0;
        source.clip.GetData(samples, pPos);
        foreach (float sample in samples)
        {
            newValue += Mathf.Abs(sample);
        }
        newValue /= 1024;
        newValue *= valueMultiplier;

        if (newValue < 0.001f * valueMultiplier)
        {
            newValue = 0f;
        }

        return newValue;
    }

    public void UpdateAudioSliders(float min,float max)
    {
        showLoudnessSlider.minValue = min;
        showLoudnessSlider.maxValue = max;
        volumeThresholdSlider.minValue = min;
        volumeThresholdSlider.maxValue = max;
        rangeLimits.UpdateFields(min,max);
    }

    IEnumerator GetInfo()
    {
        while (true)
        {
            currentValue = GetCurrentVolume();
            showLoudnessSlider.value = currentValue;
            if(currentValue < volumeThreshold)
            {
                sliderColor = Color.red;
            } else
            {
                sliderColor = Color.green;
            }
            showLoudnessSlider.fillRect.GetComponent<Image>().color = sliderColor;
            //print("Current value " + currentValue);
            yield return new WaitForSeconds(0.1f);
        }
    }
}

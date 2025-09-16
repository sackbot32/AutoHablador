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
    public Sprite shut;
    public Sprite talking;
}
public class Talker : MonoBehaviour
{
    public Image characterShowing;
    public int whichExpresion;
    public List<CharacterPortraits> characterExpresions;
    [Range(0f, 1f)]
    public float volumeThreshold;
    public AudioSource source;
    public Slider testSlider;
    public AudioMixer mixer;
    float[] samples;
    public float valueMultiplier = 1;
    public float currentValue;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        samples = new float[1024];
        StartCoroutine(GetInfo());
    }

    // Update is called once per frame
    void Update()
    {
        if(currentValue > 0)
        {
            characterShowing.sprite = characterExpresions[whichExpresion].talking;
        } else
        {
            characterShowing.sprite = characterExpresions[whichExpresion].shut;
        }
        characterShowing.SetNativeSize();
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

        if(newValue < 0.001f*valueMultiplier)
        {
            newValue = 0f;
        }

        return newValue;

    }

    IEnumerator GetInfo()
    {
        while (true)
        {
            if (source.isPlaying)
            {
                currentValue = GetCurrentVolume();
            }
            testSlider.value = currentValue;
            //print("Current value " + currentValue);
            yield return new WaitForSeconds(0.1f);
        }
    }
}

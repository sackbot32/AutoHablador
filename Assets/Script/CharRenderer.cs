using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using ImageMagick;

public class CharRenderer : MonoBehaviour
{

    [HideInInspector]
    public List<Texture2D> renderedTextures = new List<Texture2D>();
    public int framesPerSecond = 30;


    public void RenderByRecord()
    {

    }
    public void RenderByFiles()
    {
        renderedTextures.Clear();
        StartCoroutine(RenderCouritine());
        
    }

    IEnumerator RenderCouritine()
    {
        List<string> imagePathList = new List<string>();
        int num = 0;
        print("Starting render");
        for (int i = 0; i < InfoSingleton.Instance.talker.source.clip.samples; i = i + InfoSingleton.Instance.talker.source.clip.frequency/framesPerSecond)
        {
            print("Rendering image number " + num);
            renderedTextures.Add(InfoSingleton.Instance.talker.ReturnCharImageOnPPos(i));
            byte[] imageData = InfoSingleton.Instance.talker.ReturnCharImageOnPPos(i).EncodeToPNG();
            string numberStringed = num.ToString("000");
            System.IO.File.WriteAllBytes("F:\\PruebaAutoHablador\\1\\imagenNumero-" + numberStringed + ".png", imageData);
            imagePathList.Add("F:\\PruebaAutoHablador\\1\\imagenNumero-" + numberStringed + ".png");
            num++;
            yield return new WaitForSeconds(0.01f);
        }
        if(imagePathList.Count > 0)
        {
            TurnImagesToVideo(imagePathList);
        }
    }

    private void TurnImagesToVideo(List<string> imagePathList)
    {
        
        MagickImageCollection imageCollection = new MagickImageCollection();
        //int z = 0;
        foreach (string imagePath in imagePathList)
        {
            MagickImage imageToAdd = new MagickImage(imagePath);

            uint delay = 0;

            delay = (uint) Mathf.RoundToInt(100f / (float)framesPerSecond);

            print("El delay es " + delay);

            imageToAdd.AnimationDelay = delay;
            imageToAdd.GifDisposeMethod = GifDisposeMethod.Previous;
            imageCollection.Add(imageToAdd);
        }
        imageCollection.OptimizeTransparency();
        //imageCollection.Optimize();

        print("Turning to video");

        imageCollection.Write("F:\\PruebaAutoHablador\\1\\joinedVideo.gif");


        
        foreach (string path in imagePathList)
        {
            System.IO.File.Delete(path);
        }
        print("Done, video in F:\\PruebaAutoHablador\\1\\joinedVideo.gif");
    }

    

    //Get audio
    //string audioPath = "F:\\PruebaAutoHablador\\1\\extractedAudio.wav";
    //print("Extracting audio");
    //float[] samples = new float[InfoSingleton.Instance.talker.source.clip.samples];
    //InfoSingleton.Instance.talker.source.clip.GetData(samples, 0);
    //byte[] audioData = BitConverter.DoubleToInt64Bits(samples);
    //print("Adding audio");
    //FFMpeg.ReplaceAudio("F:\\PruebaAutoHablador\\1\\joinedVideo.mp4",audioPath, "F:\\PruebaAutoHablador\\1\\joinedVideo.mp4");

}

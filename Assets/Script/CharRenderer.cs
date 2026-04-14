using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Emgu.CV;
using FFMpegCore;
using FFMpegCore.Enums;
using FFMpegCore.Helpers;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class AlreadyGreenedImage 
{
    public Texture2D original;
    public Texture2D greenedImage;

    public AlreadyGreenedImage(Texture2D nOg, Texture2D nGImg)
    {
        original = nOg;
        greenedImage = nGImg;
    }
    public AlreadyGreenedImage()
    {

    }
}


public class CharRenderer : MonoBehaviour
{

    [HideInInspector]
    public List<Texture2D> renderedTextures = new List<Texture2D>();
    public int framesPerSecond = 30;
    public GameObject loadingBarParent;
    public Slider progressBar;
    public Text whatIsLoading;
    private string loadingTextBase = "Currently Loading: \n";
    private Size currentSize;
    public bool greenScreen;
    public bool audioOnRender;
    private List<AlreadyGreenedImage> greenedImages;
    private List<Mat> textureMats;
    List<string> imagePathList;
    string targetPath;
    private string videoFileExtension = "mov";

    private Coroutine currentCoroutine;
    private List<CharacterPortraits> tempGreenPortraits;

    private void Start()
    {
        loadingBarParent.SetActive(false);
    }

    public void SetGreenScreen(bool green)
    {
        greenScreen = green;
    }

    public void SetAudioRender(bool auidi)
    {
        audioOnRender = auidi;
    }

    public void SetFramesPerSecond(string fps)
    {
        framesPerSecond = int.Parse(fps);
    }
    public void SelectFileToSaveRender()
    {
        //Stream stream;
        videoFileExtension = greenScreen ? "mp4" : "mov";
        //System.Windows.Forms.SaveFileDialog sfd = new System.Windows.Forms.SaveFileDialog();
        string title = "Select where to put your rendered gif";
        //sfd.Title = title;
        //sfd.Filter =  $"{videoFileExtension} (*.{videoFileExtension}) | *.{videoFileExtension}";
        //sfd.FilterIndex = 1;
        //if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        //{
        //    stream = sfd.OpenFile();
        //    if (stream != null)
        //    {
        //        FileStream file = stream as FileStream;
        //        if (file != null)
        //        {
        //            RenderByFiles(file.Name);

        //        }
        //        stream.Close();
        //    }
        //}
        string savePath = FileBrowserStatic.SaveFile(title, $"{videoFileExtension}(*.{videoFileExtension})|*.{videoFileExtension}", "default", videoFileExtension);
        if (savePath != string.Empty)
        {
            RenderByFiles(savePath);
        }
    }
    public void RenderByFiles(string path)
    {
        imagePathList = new List<string>();
        targetPath = path;
        renderedTextures.Clear();
        loadingBarParent.SetActive(true);
        loadingTextBase = Loader.Instance.GetLocalizedMessage("loadRenderTextBase") + "\n";
        whatIsLoading.text = loadingTextBase + Loader.Instance.GetLocalizedMessage("loadRenderStartText");
        progressBar.minValue = 0;
        progressBar.value = 0;
        int maxValue = 0;
        maxValue = InfoSingleton.Instance.talker.source.clip.samples / (InfoSingleton.Instance.talker.source.clip.frequency / framesPerSecond);
        maxValue += 2;
        progressBar.maxValue = maxValue;
        textureMats = new List<Mat>();
        greenedImages = new List<AlreadyGreenedImage>();
        currentCoroutine = StartCoroutine(RenderCouritine(path, UnityEngine.Color.green));
        
    }

    public void RenderByFiles(string path, UnityEngine.Color whichColor, bool nGreenScreen = false, bool usesAudio = false)
    {
        imagePathList = new List<string>();
        targetPath = path;
        greenScreen = nGreenScreen;
        audioOnRender = usesAudio;
        renderedTextures.Clear();
        loadingBarParent.SetActive(true);
        loadingTextBase = Loader.Instance.GetLocalizedMessage("loadRenderTextBase") + "\n";
        whatIsLoading.text = loadingTextBase + Loader.Instance.GetLocalizedMessage("loadRenderStartText");
        progressBar.minValue = 0;
        progressBar.value = 0;
        int maxValue = 0;
        maxValue = InfoSingleton.Instance.talker.source.clip.samples / (InfoSingleton.Instance.talker.source.clip.frequency / framesPerSecond);
        maxValue += 2;
        progressBar.maxValue = maxValue;
        textureMats = new List<Mat>();
        greenedImages = new List<AlreadyGreenedImage>();
        currentCoroutine = StartCoroutine(RenderCouritine(path,whichColor));

    }

    public IEnumerator RenderCouritine(string path,UnityEngine.Color colorBackground)
    {

        imagePathList = new List<string>();
        currentSize = new Size(InfoSingleton.Instance.talker.ReturnCharImageOnPPos(0).width, InfoSingleton.Instance.talker.ReturnCharImageOnPPos(0).height);
        InfoSingleton.Instance.saveFile.SavePortraitImages();
        int num = 0;
        int total = (int)progressBar.maxValue - 2;
        string temporalFilesPath = Loader.Instance.saveFilePath + "\\temp\\";
        print("temporalFiles " + temporalFilesPath);
        tempGreenPortraits = new List<CharacterPortraits>();
        for (int i = 0; i < InfoSingleton.Instance.talker.characterExpresions.Count; i++)
        {
            tempGreenPortraits.Add(new CharacterPortraits());
            tempGreenPortraits[i].expresionName = InfoSingleton.Instance.talker.characterExpresions[i].expresionName;
            tempGreenPortraits[i].shutImagePath = "";
            tempGreenPortraits[i].talkImagePath = "";
            tempGreenPortraits[i].shut = null;
            tempGreenPortraits[i].talking = null;
        }

        print("Starting render");;
        
        for (int i = 0; i < InfoSingleton.Instance.talker.source.clip.samples; i = i + InfoSingleton.Instance.talker.source.clip.frequency/framesPerSecond)
        {

            print("Rendering image number " + num);//loadRenderImageNumText
            if(num < total)
            {
                whatIsLoading.text = loadingTextBase + Loader.Instance.GetLocalizedMessage("loadRenderImageNumText") + num + "/" + total;
            } else
            {
                whatIsLoading.text = loadingTextBase + Loader.Instance.GetLocalizedMessage("loadRenderImageToVidText");
            }
            Texture2D starter = InfoSingleton.Instance.talker.ReturnCharImageOnPPos(i);
            
            
            if (!greenScreen)
            {
                string imgPath = InfoSingleton.Instance.talker.ReturnImagePath(i);
                imagePathList.Add(imgPath);
            } else
            {
                Texture2D beingSaved;
                CharacterPortraits currentPorToGreen = tempGreenPortraits.Find(x => x.expresionName == InfoSingleton.Instance.talker.GetExpresionNameOnPpos(i));
                bool isTalking = InfoSingleton.Instance.talker.isTalkingAtPpos(i);
                int indexOfPor = tempGreenPortraits.IndexOf(currentPorToGreen);
                string imgPath = "";
                if (isTalking)
                {
                    if(currentPorToGreen.talkImagePath.Length == 0)
                    {
                        beingSaved = FromTransparentToColor(starter, colorBackground);
                        string resultingPath = temporalFilesPath + currentPorToGreen.expresionName + "-talk.png";
                        System.IO.File.WriteAllBytes(resultingPath, beingSaved.EncodeToPNG());
                        currentPorToGreen.talkImagePath = resultingPath;
                        tempGreenPortraits[indexOfPor] = currentPorToGreen;
                        imgPath = resultingPath;
                    } else
                    {
                        imgPath = currentPorToGreen.talkImagePath;
                    }
                } else
                {
                    if (currentPorToGreen.shutImagePath.Length == 0)
                    {
                        beingSaved = FromTransparentToColor(starter, colorBackground);
                        string resultingPath = temporalFilesPath + currentPorToGreen.expresionName + "-shut.png";
                        System.IO.File.WriteAllBytes(resultingPath, beingSaved.EncodeToPNG());
                        currentPorToGreen.shutImagePath = resultingPath;
                        tempGreenPortraits[indexOfPor] = currentPorToGreen;
                        imgPath = resultingPath;
                    }
                    else
                    {
                        imgPath = currentPorToGreen.shutImagePath;
                    }
                }
                imagePathList.Add(imgPath);
            }

                num++;
            progressBar.value += 1;
            yield return new WaitForSeconds(0.001f);
        }
        if(imagePathList.Count > 0)
        {
            
            progressBar.value += 1;
            string whichCodec = greenScreen ? "libx264" : "prores_ks";
            TurnImagesToFFMpeg(path,imagePathList, whichCodec);
        } else
        {
            //TODO substitute with icon
            Loader.Instance.CreateNotif(Loader.Instance.GetLocalizedMessage("errorNotifLackImage"), NotifType.Error, "OK");
            loadingBarParent.SetActive(false);
        }
    }

    public void CancelRender()
    {
        StopCoroutine(currentCoroutine);
        foreach (CharacterPortraits greenPor in tempGreenPortraits)
        {
            //greenPor.talkImagePath
            if (System.IO.File.Exists(greenPor.talkImagePath))
            {
                System.IO.File.Delete(greenPor.talkImagePath);

            }
            if (System.IO.File.Exists(greenPor.shutImagePath))
            {
                System.IO.File.Delete(greenPor.shutImagePath);

            }
        }
        System.IO.File.Delete(targetPath);
    }

    private void TurnImagesToFFMpeg(string savePath, List<string> imagePathList,string codec)
    {
        
        string onlyVideoPath = savePath.Substring(0, savePath.LastIndexOf("\\")) + "\\tempVid." + videoFileExtension;
        if (audioOnRender)
        {
            var video = JoinImageSequenceWithCodec(onlyVideoPath, codec, framesPerSecond,imagePathList.ToArray());
            //FFMpegArguments.FromFileInput(imagePathList.ToArray(),false,options => options.WithFramerate(framesPerSecond).
            //WithVideoCodec(FFMpeg.GetCodec("apcn"))).OutputToFile(savePath);
            print(InfoSingleton.Instance.audioPath);
            FFMpeg.ReplaceAudio(onlyVideoPath, InfoSingleton.Instance.audioPath,savePath);
            
        } else
        {
            var video = JoinImageSequenceWithCodec(savePath, codec, framesPerSecond, imagePathList.ToArray());
        }
        foreach (CharacterPortraits greenPor in tempGreenPortraits)
        {
            //greenPor.talkImagePath
            if (System.IO.File.Exists(greenPor.talkImagePath))
            {
                System.IO.File.Delete(greenPor.talkImagePath);

            }
            if (System.IO.File.Exists(greenPor.shutImagePath))
            {
                System.IO.File.Delete(greenPor.shutImagePath);

            }
        }
        if (System.IO.File.Exists(onlyVideoPath))
        {
            System.IO.File.Delete(onlyVideoPath);
        }
        progressBar.value += 1;
        //Loader.Instance.GetLocalizedMessage("errorNotifLackImage")
        whatIsLoading.text = loadingTextBase + Loader.Instance.GetLocalizedMessage("loadRenderImageToVidTextDone");
        loadingBarParent.SetActive(false);
        print(savePath.Substring(0, savePath.LastIndexOf("\\")));
        Loader.Instance.CreateAssurance(Loader.Instance.GetLocalizedMessage("succNotifRenderComplete", new object[] { savePath }), () => {
            if(Directory.Exists(savePath.Substring(0, savePath.LastIndexOf("\\"))))
            {
                System.Diagnostics.Process.Start("explorer.exe", @savePath.Substring(0, savePath.LastIndexOf("\\")));
                //System.Diagnostics.Process.Start("explorer.exe", @savePath);
            }
        });
    }

    private Texture2D FromTransparentToColor(Texture2D starter, UnityEngine.Color selectedColor)
    {
        Texture2D toReturn = starter;
        for (int x = 0; x < currentSize.Width; x++)
        {
            for (int y = 0; y < currentSize.Height; y++)
            {
                UnityEngine.Color pixelColor = toReturn.GetPixel(x, y);
                if (pixelColor.a <= 0f)
                {
                    toReturn.SetPixel(x, y, selectedColor);
                } else if(pixelColor.a > 0 && pixelColor.a <= 1f)
                {
                    toReturn.SetPixel(x, y, new UnityEngine.Color(pixelColor.r,pixelColor.g,pixelColor.b,1));
                }
            }
        }

        return toReturn;
    }


    public bool JoinImageSequenceWithCodec(string output, string videoCodec, double frameRate = 30, params string[] images)
    {
        var fileExtensions = images.Select(Path.GetExtension).Distinct().ToArray();
        if (fileExtensions.Length != 1)
        {
            throw new ArgumentException("All images must have the same extension", nameof(images));
        }

        var fileExtension = fileExtensions[0].ToLowerInvariant();
        int? width = null, height = null;

        var tempFolderName = Path.Combine(GlobalFFOptions.Current.TemporaryFilesFolder, Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempFolderName);

        try
        {
            var index = 0;
            foreach (var imagePath in images)
            {
                var analysis = FFProbe.Analyse(imagePath);
                FFMpegHelper.ConversionSizeExceptionCheck(analysis.PrimaryVideoStream!.Width, analysis.PrimaryVideoStream!.Height);
                width ??= analysis.PrimaryVideoStream.Width;
                height ??= analysis.PrimaryVideoStream.Height;

                var destinationPath = Path.Combine(tempFolderName, $"{index++.ToString().PadLeft(9, '0')}{fileExtension}");
                File.Copy(imagePath, destinationPath);
            }

            return FFMpegArguments
                .FromFileInput(Path.Combine(tempFolderName, $"%09d{fileExtension}"), false, options => options
                    .WithFramerate(frameRate))
                .OutputToFile(output, true, options => options
                    .Resize(width!.Value, height!.Value)
                    .WithFramerate(frameRate).WithVideoCodec(videoCodec).WithVariableBitrate(5))
                .ProcessSynchronously();
        }
        finally
        {
            Directory.Delete(tempFolderName, true);
        }
    }


}

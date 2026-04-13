using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Emgu.CV;
using FFMpegCore;
using FFMpegCore.Enums;
using FFMpegCore.Helpers;
using FFMpegCore.Pipes;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using static UnityEngine.UI.Image;

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

    private List<IVideoFrame> videoFrames;
    private ByteFrame currentFrame;
    private List<string> subVideosPath;
    private Coroutine currentCoroutine;

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
        float[] volumesList = new float[InfoSingleton.Instance.talker.source.clip.samples / (InfoSingleton.Instance.talker.source.clip.frequency / framesPerSecond)];
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
        //int n = 0;
        //for (int i = 0; i < InfoSingleton.Instance.talker.source.clip.samples; i = i + InfoSingleton.Instance.talker.source.clip.frequency / framesPerSecond)
        //{
        //    volumesList[n] = InfoSingleton.Instance.talker.GetVolumeOnPPos(i);
        //    n++;
        //}
        //DataToFfmpeg(path, UnityEngine.Color.green, greenScreen ? "libx264" : "prores_ks", volumesList);
    }

    public void RenderByFiles(string path, UnityEngine.Color whichColor, bool nGreenScreen = false, bool usesAudio = false)
    {
        float[] volumesList = new float[InfoSingleton.Instance.talker.source.clip.samples / (InfoSingleton.Instance.talker.source.clip.frequency / framesPerSecond)  +1];
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
        currentCoroutine = StartCoroutine(RenderCouritine(path, whichColor));
        //int n = 0;
        //for (int i = 0; i < InfoSingleton.Instance.talker.source.clip.samples; i = i + InfoSingleton.Instance.talker.source.clip.frequency / framesPerSecond)
        //{
        //    volumesList[n] = InfoSingleton.Instance.talker.GetVolumeOnPPos(i);
        //    n++;
        //}
        //DataToFfmpeg(path, whichColor, greenScreen ? "libx264" : "prores_ks",volumesList);

    }

    public IEnumerator RenderCouritine(string path,UnityEngine.Color colorBackground)
    {;
        subVideosPath = new List<string>();
        byte[] byteList = new byte[0];
        imagePathList = new List<string>();
        currentSize = new Size(InfoSingleton.Instance.talker.ReturnCharImageOnPPos(0).width, InfoSingleton.Instance.talker.ReturnCharImageOnPPos(0).height);
        
        int num = 0;
        int currentSubVideoIndex = 0;
        string subVideoSavePath = Loader.Instance.saveFilePath + "\\temp\\" ;
        if (!System.IO.File.Exists(subVideoSavePath))
        {
            Directory.CreateDirectory(subVideoSavePath);
        }
        //videoFrames = new IVideoFrame[(InfoSingleton.Instance.talker.source.clip.samples / (InfoSingleton.Instance.talker.source.clip.frequency / framesPerSecond)) + 1];
        videoFrames = new List<IVideoFrame>();
        int total = (int)progressBar.maxValue - 2;
        string temporalFilesPath = path.Substring(0,path.LastIndexOf("\\"));
        print("temporalFiles " + temporalFilesPath);
        print("Starting render");
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
            Texture2D beingSaved;
            if (greenScreen)
            {
                AlreadyGreenedImage alreadyGreen = greenedImages.Find(x => x.original == starter);
                if (alreadyGreen != null)
                {
                    beingSaved = alreadyGreen.greenedImage;
                } else
                {
                    beingSaved = FromTransparentToColor(starter, colorBackground);
                }
                    
            } else
            {
                beingSaved = starter;
            }
            //beingSaved = FlipHorizontal(beingSaved);
            
            renderedTextures.Add(beingSaved);
            byteList = beingSaved.GetRawTextureData();
            float startPoint = (float)(beingSaved.height * beingSaved.width) * 4;
            byteList.TryRemoveElementsInRange((int)startPoint, byteList.Length - (int)startPoint,out Exception error);
            //AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAQUI

            byte[] pre = Reorient(byteList, beingSaved.width, beingSaved.height);
            byte[] tempImageData = pre.Reverse().ToArray();
            print("LENGHT " + tempImageData.Length);

            videoFrames.Add(new ByteFrame(tempImageData, beingSaved.format, beingSaved.width, beingSaved.height));
            if(videoFrames.Count > 6000)
            {
                    whatIsLoading.text = loadingTextBase + "TESTING CURRENTLY TURNING FRAMES INTO SUBVIDEO";
                    yield return new WaitForEndOfFrame();
                    currentSubVideoIndex++;
                    RawVideoPipeSource subVideoFrameSource = new RawVideoPipeSource(videoFrames.ToList())
                    {

                        FrameRate = framesPerSecond
                    };
                    var video = FFMpegArguments.FromPipeInput(subVideoFrameSource)
                .OutputToFile(subVideoSavePath + currentSubVideoIndex + "." + videoFileExtension, true, options => options
                    .WithFramerate(framesPerSecond).WithVideoCodec(greenScreen ? "libx264" : "prores_ks").WithVariableBitrate(5))
                .ProcessSynchronously();

                    subVideosPath.Add(subVideoSavePath + currentSubVideoIndex + "." + videoFileExtension);
                videoFrames.Clear();
            }
            //subVideoPath
            string numberStringed = num.ToString("0000");
            //System.IO.File.WriteAllBytes(temporalFilesPath + "\\imagenNumero-" + numberStringed + ".png", tempImageData);
            imagePathList.Add(temporalFilesPath + "\\imagenNumero-" + numberStringed + ".png");
            num++;
            progressBar.value += 1;
            yield return new WaitForSeconds(0.01f);
        }
        if(subVideosPath.Count > 0 || videoFrames.Count > 0)
        {
            if (videoFrames.Count > 0)
            {
                currentSubVideoIndex++;
                whatIsLoading.text = loadingTextBase + "TESTING CURRENTLY TURNING LASTS FRAMES INTO SUBVIDEO";
                yield return new WaitForEndOfFrame();
                RawVideoPipeSource subVideoFrameSource = new RawVideoPipeSource(videoFrames.ToList())
                {

                    FrameRate = framesPerSecond
                };
                var video = FFMpegArguments.FromPipeInput(subVideoFrameSource)
                .OutputToFile(subVideoSavePath + "last." + videoFileExtension, true, options => options
                .WithFramerate(framesPerSecond).WithVideoCodec(greenScreen ? "libx264" : "prores_ks").WithVariableBitrate(5))
                .ProcessSynchronously();

                subVideosPath.Add(subVideoSavePath + "last." + videoFileExtension);
                videoFrames.Clear();
            }
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
        foreach (string path in imagePathList)
        {
            System.IO.File.Delete(path);
        }
        System.IO.File.Delete(targetPath);
    }

    private void TurnImagesToFFMpeg(string savePath, List<string> imagePathList,string codec)
    {
        //RawVideoPipeSource videoFrameSource = new RawVideoPipeSource(videoFrames.ToList())
        //{
            
        //    FrameRate = framesPerSecond
        //};


        string onlyVideoPath = savePath.Substring(0, savePath.LastIndexOf("\\")) + "\\tempVid." + videoFileExtension;
        //JoinImageSequenceWithCodec(onlyVideoPath, codec, framesPerSecond,imagePathList.ToArray());
        var video = FFMpegArguments.FromFileInput(subVideosPath)
            .OutputToFile(onlyVideoPath, true, options => options
               .WithFramerate(framesPerSecond).WithVideoCodec(codec).WithVariableBitrate(5))
            .ProcessSynchronously();
        //FFMpegArguments.FromFileInput(imagePathList.ToArray(),false,options => options.WithFramerate(framesPerSecond).
        //WithVideoCodec(FFMpeg.GetCodec("apcn"))).OutputToFile(savePath);
        print(InfoSingleton.Instance.audioPath);
        string bothPath = savePath.Substring(0, savePath.LastIndexOf("\\")) + "\\tempVidAud." + videoFileExtension;
        FFMpeg.ReplaceAudio(onlyVideoPath, InfoSingleton.Instance.audioPath, audioOnRender ? savePath : bothPath);


        if (!audioOnRender)
        {
            FFMpeg.Mute(bothPath, savePath);
        }
        foreach (string path in imagePathList)
        {
            System.IO.File.Delete(path);
        }
        foreach(string subvidPath in subVideosPath)
        {
            System.IO.File.Delete(subvidPath);
        }
        if (System.IO.File.Exists(onlyVideoPath))
        {
            System.IO.File.Delete(onlyVideoPath);
        }
        if (System.IO.File.Exists(bothPath))
        {
            System.IO.File.Delete(bothPath);
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
        videoFrames = null;
    }

    private IEnumerable<IVideoFrame> DataForVideo(int samples, int frequency,UnityEngine.Color colorBackground,float[] volumes)
    {
        
        int num = 0;
        float time = 0;
        int total = (int)progressBar.maxValue - 2;
        for (int i = 0; i < samples; i = i + 0)
        {
            
            Loader.Instance.dispacher.Enqueue(() => {
                time = Time.realtimeSinceStartup;
                print("Rendering image number " + num);//loadRenderImageNumText
                if (num < total)
                {
                    whatIsLoading.text = loadingTextBase + Loader.Instance.GetLocalizedMessage("loadRenderImageNumText") + num + "/" + total;
                }
                else
                {
                    whatIsLoading.text = loadingTextBase + Loader.Instance.GetLocalizedMessage("loadRenderImageToVidText");
                }
                if(currentFrame == null && currentCoroutine == null)
                {
                    currentCoroutine = StartCoroutine(ReturnFrame(i, volumes[num], colorBackground));
                }
                if(currentFrame != null && currentCoroutine != null)
                {
                    StopCoroutine(currentCoroutine);
                    currentCoroutine = null;
                }
                
            });
            if(num > 2)
            {
                print("TOO LONG AT " + num);
                break;
            }
            if(currentFrame != null)
            {

                yield return currentFrame;
                i = i + frequency / framesPerSecond;
                num++;
                currentFrame = null;
            } 


            
        }
        //.instance.dispatcher.Enqueue
    }

    private IEnumerator ReturnFrame(int pos, float vol, UnityEngine.Color colorBackground)
    {

        byte[] tempImageData = null;
        byte[] byteList = null;
        Texture2D beingSaved = null;
        Loader.Instance.dispacher.Enqueue(() => {
            
            Texture2D starter = InfoSingleton.Instance.talker.ReturnCharImageOnPPos(pos, vol);
            if (greenScreen)
            {
                AlreadyGreenedImage alreadyGreen = greenedImages.Find(x => x.original == starter);
                if (alreadyGreen != null)
                {
                    beingSaved = alreadyGreen.greenedImage;
                }
                else
                {
                    beingSaved = FromTransparentToColor(starter, colorBackground);
                }

            }
            else
            {
                beingSaved = starter;
            }
            renderedTextures.Add(beingSaved);
            byteList = beingSaved.GetRawTextureData();
            float startPoint = (float)(beingSaved.height * beingSaved.width) * 4;
            byteList.TryRemoveElementsInRange((int)startPoint, byteList.Length - (int)startPoint, out Exception error);

            byte[] pre = Reorient(byteList, beingSaved.width, beingSaved.height);
            tempImageData = pre.Reverse().ToArray();
        });

        while(tempImageData == null)
        {
            yield return new WaitForSeconds(0.1f);
        }

        currentFrame = new ByteFrame(tempImageData, beingSaved.format, beingSaved.width, beingSaved.height);

    }

    private async void DataToFfmpeg(string savePath, UnityEngine.Color colorBackground, string codec, float[] volumes)
    {
        RawVideoPipeSource videoFrameSource = new RawVideoPipeSource(DataForVideo(InfoSingleton.Instance.talker.source.clip.samples, InfoSingleton.Instance.talker.source.clip.frequency, colorBackground, volumes))
        {

            FrameRate = framesPerSecond
        };
        print("el path "+savePath);

        string onlyVideoPath = savePath.Substring(0, savePath.LastIndexOf("\\")) + "\\tempVid." + videoFileExtension;
        await FFMpegArguments.FromPipeInput(videoFrameSource)
                .OutputToFile(onlyVideoPath + "last." + videoFileExtension, true, options => options
                .WithFramerate(framesPerSecond).WithVideoCodec(greenScreen ? "libx264" : "prores_ks").WithVariableBitrate(5))
                .ProcessAsynchronously();
        string bothPath = savePath.Substring(0, savePath.LastIndexOf("\\")) + "\\tempVidAud." + videoFileExtension;
        FFMpeg.ReplaceAudio(onlyVideoPath, InfoSingleton.Instance.audioPath, audioOnRender ? savePath : bothPath);
        if (!audioOnRender)
        {
            FFMpeg.Mute(bothPath, savePath);
        }
        whatIsLoading.text = loadingTextBase + Loader.Instance.GetLocalizedMessage("loadRenderImageToVidTextDone");
        loadingBarParent.SetActive(false);
        print(savePath.Substring(0, savePath.LastIndexOf("\\")));
        Loader.Instance.CreateAssurance(Loader.Instance.GetLocalizedMessage("succNotifRenderComplete", new object[] { savePath }), () => {
            if (Directory.Exists(savePath.Substring(0, savePath.LastIndexOf("\\"))))
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

    public static void FlipHorizontal(Texture2D original)
    {
        var originalPixels = original.GetPixels();

        var newPixels = new UnityEngine.Color[originalPixels.Length];

        for (int x = 0; x < original.width; x++)
        {
            for (int y = 0; y < original.height; y++)
            {
                newPixels[x + y * original.width] = originalPixels[(original.width - x - 1) + y * original.width];
            }
        }
        original.SetPixels(newPixels);
        original.Apply();
    }

    public static void FlipVertical(Texture2D original)
    {
        var originalPixels = original.GetPixels();

        var newPixels = new UnityEngine.Color[originalPixels.Length];

        for (int x = 0; x < original.width; x++)
        {
            for (int y = 0; y < original.height; y++)
            {
                newPixels[x + y * original.width] = originalPixels[x + (original.height - y - 1) * original.width];
            }
        }
        original.SetPixels(newPixels);
        original.Apply();
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


    private List<byte> ShuffleTest(List<byte> bytes)
    {
        System.Random random = new System.Random();
        List<byte> result = bytes;
        int n = result.Count;
        while (n > 1)
        {
            n--;
            int k = random.Next(n + 1);
            byte value = result[k];
            result[k] = result[n];
            result[n] = value;

        }
        
       

        return result;

    }

    private byte[] Reorient(byte[] bytes, int width, int height)
    {
        byte[] list = bytes;
        List<byte> result = new List<byte>();
        List<byte> widthToAdd = null;


        for (int i = 0; i < height; i++)
        {
            widthToAdd = new List<byte>();
            int j = 0;
            int startPoint = (width*4) * i;
            int endPoint = (width * 4) * (j+1);
            //print($"first {startPoint + j} last point {(width *4) - j}");
            while(j < (width * 4))
            {
                widthToAdd.Add(bytes[startPoint + j]);

                j++;
            }
            result.AddRange(widthToAdd.Reverse<byte>());
        }
        //print("AMOUNT DONE " + n + " THE AMOUNT USED " + amount);

        return result.ToArray();

    }


}

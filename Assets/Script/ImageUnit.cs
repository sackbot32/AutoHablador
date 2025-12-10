using UnityEngine;
using UnityEngine.UI;
using System;
using System.Windows;
using System.Threading.Tasks;
using System.IO;

public class ImageUnit : MonoBehaviour
{

    public Image image;
    public string typeOf;
    private Image background;

    private void Awake()
    {
        background = image.gameObject.transform.parent.gameObject.GetComponent<Image>();
        print(background.name);
    }
    public void LoadForEdit(Sprite newImage)
    {
        if(newImage != null)
        {
            image.color = new Color(1,1,1,1);
            image.sprite = newImage;
            //image.SetNativeSize();
            if (background == null)
            {
                background = image.gameObject.transform.parent.gameObject.GetComponent<Image>();
            }
            float newHeight = (newImage.rect.height * background.transform.localScale.x) / newImage.rect.width;
            image.transform.localScale = new Vector2(background.transform.localScale.x, newHeight) * 4;
        } else
        {
            image.color = new Color(0,0,0,0);
            image.sprite = null;
        }
        
    }


    public Sprite SaveEdit()
    {
        return image.sprite;
    }

    public void LoadExternalImage()
    {
        print("Loading for " + gameObject.name);
        Stream stream;
        System.Windows.Forms.OpenFileDialog sfd = new System.Windows.Forms.OpenFileDialog();
        string title = "Select image for: " + typeOf;
        sfd.Title = title;
        sfd.Filter = "Images with transparency (*.png) | *.png | All files (*.*)|*.*";
        sfd.FilterIndex = 1;
        if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        {
            stream = sfd.OpenFile();
            if (stream != null)
            {
                FileStream file = stream as FileStream;
                if (file != null)
                {
                    print(file.Name);
                    Texture2D imageChosen = LoadImage(file.Name);
                    if (imageChosen != null)
                    {
                        Sprite newSprite = Sprite.Create(imageChosen, new Rect(0, 0, imageChosen.width, imageChosen.height), new Vector2(0, 0));

                        //image.sprite = newSprite;
                        LoadForEdit(newSprite);
                    }
                    else
                    {
                        Loader.Instance.CreateNotif("No image found in file", NotifType.Error, "OK");
                    }

                }
                stream.Close();
            }
        }
    }

    public static Texture2D LoadImage(string path)
    {
        Texture2D imageTexture = new Texture2D(2, 2);
        byte[] imageData = File.ReadAllBytes(path);
        if (imageTexture.LoadImage(imageData))
        {
            return imageTexture;
        }
        return null;
    }


}

using AnotherFileBrowser.Windows;
using UnityEngine;

public class FileBrowserStatic : MonoBehaviour
{
    public static string OpenFile(string title, string filter, int filterIndex)
    {
        BrowserProperties bp = new BrowserProperties();
        bp.title = title;
        bp.filter = filter;
        bp.filterIndex = 0;
        string toReturn = string.Empty;
        new FileBrowser().OpenFileBrowser(bp, result =>
        {
            toReturn = result;
            Debug.Log(result);
        });

        return toReturn;
    }


    public static string SaveFile(string title,string filter,string defaultName,string extension)
    {
        string toReturn = string.Empty;
        BrowserProperties bp = new BrowserProperties();
        bp.title = title;
        bp.filter = filter;
        bp.filterIndex = 0;
        new FileBrowser().SaveFileBrowser(bp, defaultName, "." + extension, result =>
        {
            toReturn = result;
            Debug.Log(result);
        });
        return toReturn;
    }
}

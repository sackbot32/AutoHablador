using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ProjectSelectorCreator : MonoBehaviour
{
    public GameObject projectButtonPrefab;
    private List<string> projectsName;
    public bool debug;
    public int debugButtonAmountTest;
    public Button newProjectButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        projectsName = GetAllProjects();
        CreateButtons();
        //newProjectButton.onClick.AddListener(() => Loader.Instance.CreateTextInput("Set name",Loader.Instance.NewProyect));
    }

    public void CreateButtons()
    {
        foreach (Transform children in transform)
        {
            Destroy(children.gameObject);
        }

        if (debug)
        {
            for (int i = 0; i < debugButtonAmountTest; i++)
            {
                GameObject newButton = Instantiate(projectButtonPrefab, transform);
            }
        } else
        {
            foreach (string projectName in projectsName)
            {
                GameObject newButton = Instantiate(projectButtonPrefab, transform);
                newButton.GetComponent<Button>().onClick.AddListener(() => Loader.Instance.OpenFile(projectName));
                newButton.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() => CallDeleteProject(projectName));
                newButton.transform.GetChild(0).GetComponent<Text>().text = projectName;
            }
        }
    }

    public void CallDeleteProject(string projectName)
    {
        Loader.Instance.CreateAssurance("Are you sure you want to delete " + projectName, DeleteProject(projectName));
    }

    public Action DeleteProject(string projectName)
    {
        return () =>
        {
            if (System.IO.Directory.Exists(Loader.Instance.saveFilePath + "\\" + projectName))
            {
                DirectoryInfo di = new DirectoryInfo(Loader.Instance.saveFilePath + "\\" + projectName);
                foreach (FileInfo file in di.GetFiles())
                {
                    print(file.FullName);
                    file.Delete();
                }
                foreach (DirectoryInfo dir in di.GetDirectories())
                {
                    print(dir.FullName);
                    dir.Delete(true);
                }
                projectsName = GetAllProjects();
                CreateButtons();
            }
        };
    }
    public static List<string> GetAllProjects()
    {
        List<string> toReturn = new List<string>();
        string[] folders = Directory.GetDirectories(Loader.Instance.saveFilePath);
        foreach (string folder in folders)
        {
            string projectName = folder.Substring(folder.LastIndexOf('\\') + 1);
            if(System.IO.File.Exists(Loader.Instance.saveFilePath + "\\" + projectName + "\\" + "saveOf" + projectName + ".json")){
                toReturn.Add(projectName);
                print(projectName + " correct");
            }
        }
        return toReturn;
    }
}

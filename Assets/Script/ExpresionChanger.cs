using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class ExpresionTime
{
    public int whichExpresion;
    public float timeToStart;
}

public class ExpresionChanger : MonoBehaviour
{
    public List<ExpresionTime> timeExpresionList;
    public Talker talker;

    
    void Update()
    {
        if (timeExpresionList.Count > 0) 
        { 
            for (int i = 0; i < timeExpresionList.Count; i++)
            {
                float aTime = 0;
                float bTime = 0;
                aTime = timeExpresionList[i].timeToStart;
                if(i+1 >= timeExpresionList.Count)
                {
                    bTime = InfoSingleton.Instance.length;
                } else
                {
                    bTime = timeExpresionList [i + 1].timeToStart;
                }
                if(InfoSingleton.Instance.currentTime >= aTime && InfoSingleton.Instance.currentTime < bTime)
                {
                    talker.whichExpresion = timeExpresionList[i].whichExpresion;
                }
            }
        }
    }
}

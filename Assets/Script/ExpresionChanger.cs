using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class ExpresionTime
{
    public int whichExpresion;
    public float timeToStart;
    public ExpresionTime (int newExpresion,  float newTimeToStart)
    {
        whichExpresion = newExpresion;
        timeToStart = newTimeToStart;
    }
}

public class ExpresionChanger : MonoBehaviour
{
    public List<ExpresionTime> timeExpresionList;
    public Talker talker;

    private void Start()
    {
        InfoSingleton.Instance.changer = this;
    }

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

    public int ReturnExpresionOnTime(float time)
    {
        if (timeExpresionList.Count > 0)
        {
            for (int i = 0; i < timeExpresionList.Count; i++)
            {
                float aTime = 0;
                float bTime = 0;
                aTime = timeExpresionList[i].timeToStart;
                if (i + 1 >= timeExpresionList.Count)
                {
                    bTime = InfoSingleton.Instance.length;
                }
                else
                {
                    bTime = timeExpresionList[i + 1].timeToStart;
                }

                if (time >= aTime && time < bTime)
                {
                    print("Reaches actual expresion chosen which is ? " + timeExpresionList[i].whichExpresion);
                    return timeExpresionList[i].whichExpresion;
                }
            }
        }
        print("NOT ACTUAL CHOSEN");
        return 0;
    }

    public void AddExpresion(int expression, float time)
    {
        timeExpresionList.Add(new ExpresionTime(expression, time));
    }

    public void RemoveExpresion(int numberOnList)
    {
        timeExpresionList.RemoveAt(numberOnList);

    }
}

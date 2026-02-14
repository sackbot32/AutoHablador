using UnityEngine;
using UnityEngine.UI;

public class SetAudioRangeLimits : MonoBehaviour
{

    [SerializeField]
    private InputField[] rangeFields;

    public void UpdateFields(float min, float max)
    {
        rangeFields[0].text = min.ToString();
        rangeFields[1].text = max.ToString();
    }

    public void UpdateValues()
    {
        
        bool canEnterMin = false;
        bool canEnterMax = false;
        string strMin = rangeFields[0].text;
        string strMax = rangeFields[1].text;
        print(strMax);
        if(float.TryParse(strMin, out float min))
        {

            if((strMin.Contains(",") || strMin.Contains(".")) && min != 0)
            {
                canEnterMin = true;
            }
            if(!(strMin.Contains(",") && strMin.Contains("."))){
                canEnterMin = true;
            }
            if ((strMin.Contains(",") || strMin.Contains(".")) && min == 0)
            {
                canEnterMin = false;
            }
        }
        if (float.TryParse(strMax, out float max))
        {
            if ((strMax.Contains(",") || strMax.Contains(".")) && max != 0)
            {
                canEnterMax = true;
            }
            if(!(strMax.Contains(",") && strMax.Contains(".")))
            {
                canEnterMax = true;
            }
            if ((strMax.Contains(",") || strMax.Contains(".")) && max == 0)
            {
                canEnterMax = false;
            }
        }
        if (canEnterMax && canEnterMin)
        {
            if(min > max)
            {
                max = min;
            }

            if(max < min)
            {
                min = max;
            }

            InfoSingleton.Instance.talker.UpdateAudioSliders(min, max);
        }
    }
}

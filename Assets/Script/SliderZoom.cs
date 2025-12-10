using UnityEngine;

public class SliderZoom : MonoBehaviour
{

    private RectTransform mainContent;
    private float minSize;

    private void Start()
    {
        mainContent = GetComponent<RectTransform>();
        minSize = mainContent.sizeDelta.x;
    }

    public void Zoom(float zoom)
    {
        if(minSize -1 < mainContent.sizeDelta.x * zoom)
        {
            mainContent.sizeDelta = new Vector2(mainContent.sizeDelta.x * zoom, mainContent.sizeDelta.y);
            foreach (Transform t in transform)
            {
                RectTransform tR = t.GetComponent<RectTransform>();
                tR.sizeDelta = new Vector2(tR.sizeDelta.x * zoom, tR.sizeDelta.y);
            }
        } else
        {
            print("too smol :");
        }
    }
}

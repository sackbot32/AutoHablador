using UnityEngine;

public class InfoSingleton : MonoBehaviour
{
    public static InfoSingleton Instance;
    public float length;
    public float currentTime;

    private void Awake()
    {
        Instance = this;
    }
}

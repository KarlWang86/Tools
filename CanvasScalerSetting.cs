using UnityEngine;
using UnityEngine.UI;

public class CanvasScalerSetting : MonoBehaviour
{
    public CanvasScaler canvasScaler;

    void Awake()
    {
        SetMatch();
    }
    void Start()
    {
        SetMatch();
    }

    public void SetMatch()
    {
        float screenRatio = (float)Screen.width / Screen.height;
        float designRatio = canvasScaler.referenceResolution.x / canvasScaler.referenceResolution.y;
        if (screenRatio > designRatio)
        {
            canvasScaler.matchWidthOrHeight = 1f;
        }
        else
        {
            canvasScaler.matchWidthOrHeight = 0f;
        }
    }
}

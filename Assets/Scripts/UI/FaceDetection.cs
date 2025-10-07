using System.Collections;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;

public class FaceDetection : MonoBehaviour
{

    public ARFaceManager faceManager;
    public FilterManagerIcon filterManagerIcon;

    [Header("Toggle Detection Button")]
    public Image toggleFaceDetectionImage;
    public Sprite onSprite;
    public Sprite offSprite;

    public float delay;
    public Button scanButton;

    public UIStatusManager UIStatus;
    public APIManager APIManager;

    public bool IsFaceDetected()
    {
        return (faceManager.trackables.count > 0);
    }

    public void SetScanButtonInteractable(bool types)
    {
        scanButton.interactable = types;
    }

    void Start()
    {
        if (!UIStatus.IsStatusLabelNull())
            UIStatus.SetStatusText("Arahkan kamera ke wajah...", "SYSTEM");
        if (scanButton != null)
        {
            scanButton.interactable = false;
            scanButton.onClick.AddListener(OnScanButtonPressed);
        }
    }

    public void OnScanButtonPressed()
    {
        UIStatus.SetStatusText("Memindai...", "SYSTEM");
        scanButton.interactable = false;
        StartCoroutine(APIManager.CaptureAndSendImageRoutine());
    }

    // Update is called once per frame
    void Update()
    {
        if (IsFaceDetected())
        {
            scanButton.interactable = true;
            UIStatus.SetStatusText("Wajah Terdeteksi! Silakan klik Scan.", "SYSTEM");
        }
        else
        {
            scanButton.interactable = false;
            UIStatus.SetStatusText("Arahkan kamera ke wajah...", "SYSTEM");
        }
    }
}

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

    private bool isDetecting = false;
    private Coroutine detectionCorountine;
    private string lastMessage = "";

    public bool IsFaceDetected()
    {
        return (faceManager != null && faceManager.trackables.count > 0);
    }

    public void SetScanButtonInteractable(bool types)
    {
        if (scanButton != null)
            scanButton.interactable = types;
    }

    void Start()
    {
        if (UIStatus != null && !UIStatus.IsStatusLabelNull())
            UIStatus.SetStatusText("Arahkan kamera ke wajah...", "SYSTEM");

        if (scanButton != null)
        {
            scanButton.interactable = false;
            scanButton.onClick.AddListener(OnScanButtonPressed);
        }
    }

    public void OnScanButtonPressed()
    {
        if (UIStatus != null)
            UIStatus.SetStatusText("Memindai...", "SYSTEM");

        if (scanButton != null)
            scanButton.interactable = false;

        if (APIManager != null)
            StartCoroutine(APIManager.CaptureAndSendImageRoutine());
        else
            Debug.LogWarning("APIManager belum di-assign di Inspector.");
    }

    void Update()
    {
        // Cegah error jika ada komponen yang belum di-assign
        if (faceManager == null || UIStatus == null || scanButton == null)
            return;

        bool detected = IsFaceDetected();

        // Update UI dan tombol hanya jika status berubah
        if (detected && lastMessage != "detected")
        {
            UIStatus.SetStatusText("Wajah Terdeteksi! Silakan klik Scan.", "SYSTEM");
            scanButton.interactable = true;
            lastMessage = "detected";
        }
        else if (!detected && lastMessage != "not_detected")
        {
            UIStatus.SetStatusText("Arahkan kamera ke wajah...", "SYSTEM");
            scanButton.interactable = false;
            lastMessage = "not_detected";
        }
    }
}

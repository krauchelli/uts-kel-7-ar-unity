using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class FaceTrackingEventHandler : MonoBehaviour
{
    [SerializeField] private ARFaceManager faceManager;
    [SerializeField] private UIManager uiManager;

    private bool lastFaceDetected = false;

    void Update()
    {
        if (faceManager == null || uiManager == null)
            return;

        // Periksa apakah ada wajah yang sedang dilacak
        bool isFaceDetected = faceManager.trackables.count > 0;

        // Hanya update UI jika status berubah
        if (isFaceDetected != lastFaceDetected)
        {
            lastFaceDetected = isFaceDetected;

            if (isFaceDetected)
                uiManager.ShowUIForFaceDetected();
            else
                uiManager.HideAllUI();
        }
    }
}

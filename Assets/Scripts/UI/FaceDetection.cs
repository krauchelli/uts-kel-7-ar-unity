using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class FaceDetection : MonoBehaviour
{
    [Header("Referensi API Predictor Manager")]
    public APIPredictorManager apiPredictorManager;

    [Header("Referensi AR")]
    public ARFaceManager faceManager;

    private bool lastFaceDetected = false;

    void Update()
    {
        if (apiPredictorManager == null || faceManager == null)
            return;

        // Cek apakah ada wajah yang terdeteksi
        bool isFaceDetected = faceManager.trackables.count > 0;

        // Update hanya saat status wajah berubah
        if (isFaceDetected != lastFaceDetected)
        {
            lastFaceDetected = isFaceDetected;

            if (isFaceDetected)
                apiPredictorManager.OnFaceDetected(); // panggil method baru
            else
                apiPredictorManager.OnFaceLost();     // panggil method baru
        }
    }
}
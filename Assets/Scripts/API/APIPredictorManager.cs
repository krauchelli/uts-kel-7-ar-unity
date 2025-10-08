using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

// =========================
// JSON Classes
// =========================
[Serializable]
public class PredictionRequest
{
    public string image_data;
}

[Serializable]
public class PredictionData
{
    public string prediction;
    public float confidence;
}

[Serializable]
public class PredictionResponse
{
    public int statusCode;
    public string message;
    public PredictionData data;
}

// =========================
// API Predictor Manager
// =========================
public class APIPredictorManager : MonoBehaviour
{
    [Header("Konfigurasi API")]
    [SerializeField] private string backendUrl = "https://krauchelli-uts-kel-7-ar-backend.hf.space/api/predict";

    [Header("UI")]
    [SerializeField] private Button scanButton;
    [SerializeField] private TextMeshProUGUI statusText;

    [Header("AR Components")]
    [SerializeField] private ARCameraManager cameraManager;

    private bool isFaceDetected = false;

    // Event agar UIManager / sistem lain bisa subscribe
    public static event Action<string> OnPredictionResult;

    private void Start()
    {
        SetStatusText("Arahkan kamera ke wajah...", false);

        if (scanButton != null)
            scanButton.onClick.AddListener(OnScanButtonPressed);
    }

    // Dipanggil dari FaceTrackingEventHandler saat wajah muncul
    public void OnFaceDetected()
    {
        isFaceDetected = true;
        SetStatusText("Wajah Terdeteksi! Silakan klik Scan.", true);
    }

    // Dipanggil saat wajah hilang
    public void OnFaceLost()
    {
        isFaceDetected = false;
        SetStatusText("Arahkan kamera ke wajah...", false);
    }

    // Tombol Scan ditekan
    public void OnScanButtonPressed()
    {
        SetStatusText("Memindai...", false);
        StartCoroutine(CaptureAndSendImageRoutine());
    }

    // Coroutine utama: ambil gambar, encode, kirim
    private IEnumerator CaptureAndSendImageRoutine()
    {
        if (cameraManager == null)
        {
            SetStatusText("Error: ARCameraManager tidak terhubung.", true);
            yield break;
        }

        yield return new WaitForEndOfFrame();

        if (!cameraManager.TryAcquireLatestCpuImage(out XRCpuImage cpuImage))
        {
            SetStatusText("Error: Gagal mengakses data kamera.", true);
            yield break;
        }

        Texture2D cameraTexture = ConvertCpuImageToTexture(cpuImage);
        cpuImage.Dispose();

        if (cameraTexture == null)
        {
            SetStatusText("Error: Gagal memproses gambar.", true);
            yield break;
        }

        byte[] imageBytes = cameraTexture.EncodeToJPG(75);
        string base64String = Convert.ToBase64String(imageBytes);
        Destroy(cameraTexture);

        // --- Buat request JSON ---
        PredictionRequest requestObject = new PredictionRequest { image_data = base64String };
        string jsonPayload = JsonUtility.ToJson(requestObject);

        using (UnityWebRequest request = new UnityWebRequest(backendUrl, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonPayload);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            SetStatusText("Mengirim data ke server...", false);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                try
                {
                    PredictionResponse response = JsonUtility.FromJson<PredictionResponse>(request.downloadHandler.text);

                    if (response != null && response.data != null)
                    {
                        string resultText = $"{response.data.prediction} ({response.data.confidence:P0})";

                        // Update UI
                        SetStatusText("Hasil: " + resultText, isFaceDetected);

                        // Event untuk subscriber
                        OnPredictionResult?.Invoke(response.data.prediction);

                        // TODO: Terapkan filter sesuai response.data.prediction
                    }
                    else
                    {
                        SetStatusText("Error: Respons server tidak valid.", isFaceDetected);
                        OnPredictionResult?.Invoke("Error: Respons server tidak valid");
                    }
                }
                catch (Exception e)
                {
                    SetStatusText($"Error Parsing: {e.Message}", isFaceDetected);
                    OnPredictionResult?.Invoke($"Error Parsing: {e.Message}");
                }
            }
            else
            {
                SetStatusText($"Server Error: {request.error}", isFaceDetected);
                OnPredictionResult?.Invoke($"Server Error: {request.error}");
            }
        }

        // Aktifkan kembali tombol jika wajah masih terdeteksi
        if (isFaceDetected)
            scanButton.interactable = true;
    }

    // Helper: convert XRCpuImage ke Texture2D
    private unsafe Texture2D ConvertCpuImageToTexture(XRCpuImage cpuImage)
    {
        var format = TextureFormat.RGBA32;
        var conversionParams = new XRCpuImage.ConversionParams
        {
            inputRect = new RectInt(0, 0, cpuImage.width, cpuImage.height),
            outputDimensions = new Vector2Int(cpuImage.width, cpuImage.height),
            outputFormat = format,
            transformation = XRCpuImage.Transformation.MirrorY
        };

        int size = cpuImage.GetConvertedDataSize(conversionParams);
        var buffer = new NativeArray<byte>(size, Allocator.Temp);

        try
        {
            cpuImage.Convert(conversionParams, new IntPtr(buffer.GetUnsafePtr()), buffer.Length);

            var texture = new Texture2D(
                conversionParams.outputDimensions.x,
                conversionParams.outputDimensions.y,
                format,
                false
            );

            texture.LoadRawTextureData(buffer);
            texture.Apply();
            return texture;
        }
        catch (Exception e)
        {
            Debug.LogError($"[AR/API] Gagal konversi CpuImage: {e.Message}");
            return null;
        }
        finally
        {
            if (buffer.IsCreated)
                buffer.Dispose();
        }
    }

    private void SetStatusText(string message, bool buttonInteractable)
    {
        if (statusText != null)
            statusText.text = message;

        if (scanButton != null)
            scanButton.interactable = buttonInteractable;
    }
}
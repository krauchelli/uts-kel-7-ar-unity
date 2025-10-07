using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Unity.Collections;

// =========================
// Data Struktur JSON
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
    public PredictionData data;
}

// =========================
// API Manager
// =========================
public class APIManager : MonoBehaviour
{
    [Header("Dependencies")]
    public ARCameraManager cameraManager;
    public UIStatusManager UIStatus;
    public FilterManagerIcon filterManagerIcon;
    public FaceDetection faceDetection;

    [Header("Server Settings")]
    [Tooltip("URL endpoint API backend kamu")]
    public string backendUrl = "https://krauchelli-uts-kel-7-ar-backend.hf.space/api/predict";

    // =========================
    // Coroutine Utama
    // =========================
    public IEnumerator CaptureAndSendImageRoutine()
    {
        if (cameraManager == null)
        {
            UIStatus.SetStatusText("Error: ARCameraManager belum diset!", "ERROR");
            faceDetection.SetScanButtonInteractable(false);
            yield break;
        }

        yield return new WaitForEndOfFrame(); // biar CPU image valid

        // Ambil frame dari kamera AR
        if (!cameraManager.TryAcquireLatestCpuImage(out XRCpuImage cpuImage))
        {
            UIStatus.SetStatusText("Gagal mengakses kamera. Coba lagi.", "ERROR");
            faceDetection.SetScanButtonInteractable(true);
            yield break;
        }

        // Konversi ke Texture2D
        Texture2D cameraTexture = ConvertCpuImageToTexture(cpuImage);
        cpuImage.Dispose();

        if (cameraTexture == null)
        {
            UIStatus.SetStatusText("Gagal memproses gambar kamera.", "ERROR");
            faceDetection.SetScanButtonInteractable(true);
            yield break;
        }

        // Encode ke Base64
        byte[] imageBytes = cameraTexture.EncodeToJPG(80);
        string base64String = Convert.ToBase64String(imageBytes);
        Destroy(cameraTexture);

        Debug.Log($"[DEBUG] Base64 length: {base64String.Length}");

        if (base64String.Length < 1000)
            Debug.LogWarning("[WARNING] Base64 terlalu pendek, kemungkinan konversi gagal.");

        // Buat JSON payload
        PredictionRequest requestObject = new PredictionRequest { image_data = base64String };
        string jsonPayload = JsonUtility.ToJson(requestObject);

        Debug.Log($"[DEBUG] Payload JSON: {jsonPayload.Substring(0, Mathf.Min(150, jsonPayload.Length))}...");

        // Kirim ke backend
        using (UnityWebRequest request = new UnityWebRequest(backendUrl, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonPayload);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            UIStatus.SetStatusText("Mengirim gambar ke server...", "SYSTEM");

            yield return request.SendWebRequest();

            string responseText = request.downloadHandler.text;
            Debug.Log($"[DEBUG] Server Response: {responseText}");

            if (request.result == UnityWebRequest.Result.Success)
            {
                try
                {
                    PredictionResponse response = JsonUtility.FromJson<PredictionResponse>(responseText);

                    if (response != null && response.data != null)
                    {
                        string prediction = response.data.prediction;
                        float confidence = response.data.confidence;

                        filterManagerIcon.OnGenderDetected(prediction, confidence);
                        UIStatus.SetStatusText($"Prediksi: {prediction} ({confidence:P0})", "SUCCESS");
                    }
                    else
                    {
                        UIStatus.SetStatusText("Error: Respons kosong / tidak valid.", "ERROR");
                        Debug.LogWarning($"[WARNING] Respons tidak lengkap: {responseText}");
                    }
                }
                catch (Exception ex)
                {
                    UIStatus.SetStatusText($"Gagal parsing JSON: {ex.Message}", "ERROR");
                    Debug.LogError($"[EXCEPTION] {ex}");
                }
            }
            else
            {
                UIStatus.SetStatusText($"Server Error: {request.responseCode} ({request.error})", "ERROR");
                Debug.LogError($"[HTTP {request.responseCode}] {responseText}");
            }
        }

        faceDetection.SetScanButtonInteractable(true);
    }

    // =========================
    // Konversi Gambar CPU → Texture2D
    // =========================
    private Texture2D ConvertCpuImageToTexture(XRCpuImage cpuImage)
    {
        try
        {
            var conversionParams = new XRCpuImage.ConversionParams
            {
                inputRect = new RectInt(0, 0, cpuImage.width, cpuImage.height),
                outputDimensions = new Vector2Int(cpuImage.width, cpuImage.height),
                outputFormat = TextureFormat.RGBA32,
                transformation = XRCpuImage.Transformation.MirrorY
            };

            int size = cpuImage.GetConvertedDataSize(conversionParams);
            var buffer = new NativeArray<byte>(size, Allocator.Temp);
            cpuImage.Convert(conversionParams, buffer);

            Texture2D texture = new Texture2D(
                conversionParams.outputDimensions.x,
                conversionParams.outputDimensions.y,
                conversionParams.outputFormat,
                false
            );

            texture.LoadRawTextureData(buffer);
            texture.Apply();
            buffer.Dispose();

            return texture;
        }
        catch (Exception ex)
        {
            Debug.LogError($"[ERROR] Gagal convert CpuImage: {ex.Message}");
            return null;
        }
    }
}

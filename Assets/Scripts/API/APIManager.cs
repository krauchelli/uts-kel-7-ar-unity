using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

// --- Struktur JSON ---
[Serializable]
public class PredictionRequest
{
    public string image_data;
}

[Serializable]
public class PredictionResponse
{
    public int statusCode;
    public string message;
    public PredictionData data;
}

[Serializable]
public class PredictionData
{
    public string prediction;
    public float confidence;
}

// --- Script utama ---
public class APIManager : MonoBehaviour
{
    [Header("Konfigurasi API")]
    [SerializeField] private string backendUrl = "https://krauchelli-uts-kel-7-ar-backend.hf.space/api/predict";

    [Header("UI References")]
    // [SerializeField] private Button scanButton;

    [Header("AR Components")]
    [SerializeField] private ARCameraManager cameraManager;
    [SerializeField] private ARFaceManager faceManager;

    [Header("Script References")]
    public FaceDetection faceDetection;
    public FilterManagerIcon filterManagerIcon;
    public UIStatusManager UIStatus;

    public IEnumerator CaptureAndSendImageRoutine()
    {
        if (cameraManager == null)
        {
            UIStatus.SetStatusText("Error: ARCameraManager tidak diset!", "ERROR");
            faceDetection.SetScanButtonInteractable(false);
            yield break;
        }

        // 1️⃣ Ambil gambar dari kamera
        if (!cameraManager.TryAcquireLatestCpuImage(out XRCpuImage cpuImage))
        {
            UIStatus.SetStatusText("Error: Gagal mengakses data kamera.", "ERROR");
            faceDetection.SetScanButtonInteractable(true);
            yield break;
        }

        // 2️⃣ Konversi ke Texture2D
        Texture2D cameraTexture = ConvertCpuImageToTexture(cpuImage);
        cpuImage.Dispose();

        if (cameraTexture == null)
        {
            UIStatus.SetStatusText("Error: Gagal memproses gambar.", "ERROR");
            faceDetection.SetScanButtonInteractable(true);
            yield break;
        }

        // 3️⃣ Encode ke Base64
        byte[] imageBytes = cameraTexture.EncodeToJPG(75);
        string base64String = Convert.ToBase64String(imageBytes);
        Destroy(cameraTexture);

        // 4️⃣ Siapkan payload JSON
        PredictionRequest requestObject = new PredictionRequest { image_data = base64String };
        string jsonPayload = JsonUtility.ToJson(requestObject);

        using (UnityWebRequest request = new UnityWebRequest(backendUrl, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonPayload);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            // 5️⃣ Tangani respons
            if (request.result == UnityWebRequest.Result.Success)
            {
                string responseJson = request.downloadHandler.text;
                try
                {
                    PredictionResponse response = JsonUtility.FromJson<PredictionResponse>(responseJson);
                    if (response != null && response.data != null)
                    {
                        filterManagerIcon.OnGenderDetected(response.data.prediction, response.data.confidence);
                        // TODO: panggil filter di sini
                    }
                    else
                    {
                        UIStatus.SetStatusText("Error: Respons tidak valid dari server.", "ERROR");
                    }
                }
                catch (Exception ex)
                {
                    UIStatus.SetStatusText($"Error parsing JSON: {ex.Message}", "ERROR");
                }
            }
            else
            {
                UIStatus.SetStatusText($"Error: {request.error}", "ERROR");
            }
        }

        faceDetection.SetScanButtonInteractable(true);
    }

    private unsafe Texture2D ConvertCpuImageToTexture(XRCpuImage cpuImage)
    {
        try
        {
            var format = TextureFormat.RGBA32;
            var conversionParams = new XRCpuImage.ConversionParams
            {
                inputRect = new RectInt(0, 0, cpuImage.width, cpuImage.height),
                outputDimensions = new Vector2Int(cpuImage.width, cpuImage.height),
                outputFormat = format,
                transformation = XRCpuImage.Transformation.MirrorY // Biar hasilnya gak terbalik
            };

            int size = cpuImage.GetConvertedDataSize(conversionParams);
            var buffer = new NativeArray<byte>(size, Allocator.Temp);

            cpuImage.Convert(conversionParams, new IntPtr(buffer.GetUnsafePtr()), buffer.Length);

            var texture = new Texture2D(
                conversionParams.outputDimensions.x,
                conversionParams.outputDimensions.y,
                format,
                false);

            texture.LoadRawTextureData(buffer);
            texture.Apply();

            buffer.Dispose();
            return texture;
        }
        catch (Exception e)
        {
            Debug.LogError($"Gagal konversi gambar CPU: {e.Message}");
            return null;
        }
    }
}

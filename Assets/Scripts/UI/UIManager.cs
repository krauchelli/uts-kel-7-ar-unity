using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private Button scanButton;
    [SerializeField] private TMP_Text statusText;

    [Header("API Manager")]
    [SerializeField] private APIPredictorManager apiPredictor;

    void OnEnable()
    {
        // Mulai mendengarkan event hasil prediksi
        APIPredictorManager.OnPredictionResult += HandlePredictionResult;
    }

    void OnDisable()
    {
        // Berhenti mendengarkan event untuk menghindari error
        APIPredictorManager.OnPredictionResult -= HandlePredictionResult;
    }

    void Start()
    {
        if(scanButton != null)
            scanButton.onClick.AddListener(OnScanButtonPressed);

        HideAllUI();
    }

    private void OnScanButtonPressed()
    {
        if(apiPredictor != null)
        {
            // Set UI sementara
            statusText.text = "Memindai...";
            scanButton.interactable = false;

            // Mulai proses prediksi
            apiPredictor.OnScanButtonPressed();
        }
    }

    // Metode ini otomatis dipanggil saat event hasil prediksi diterima
    private void HandlePredictionResult(string result)
    {
        if (string.IsNullOrEmpty(result)) return;

        // Format: huruf pertama kapital
        string formattedResult = char.ToUpper(result[0]) + result.Substring(1);
        UpdateResultText(formattedResult);
    }

    public void ShowUIForFaceDetected()
    {
        if(statusText != null)
        {
            statusText.gameObject.SetActive(true);
            statusText.text = "Wajah Terdeteksi! Silakan klik Scan.";
        }

        if(scanButton != null)
        {
            scanButton.gameObject.SetActive(true);
            scanButton.interactable = true;
        }
    }

    public void HideAllUI()
    {
        if(statusText != null)
            statusText.gameObject.SetActive(false);

        if(scanButton != null)
            scanButton.gameObject.SetActive(false);
    }

    public void UpdateResultText(string result)
    {
        if(statusText != null)
            statusText.text = $"Hasil Prediksi: {result}";

        if(scanButton != null)
            scanButton.interactable = true; // Aktifkan kembali tombol
    }
}

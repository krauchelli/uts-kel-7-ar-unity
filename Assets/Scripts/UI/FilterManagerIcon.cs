using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

[System.Serializable]
public class FilterButton
{
    public Button button;       // Tombol utamanya
    public Image icon;          // Gambar icon di dalam tombol
    public TextMeshProUGUI label; // Teks di bawah icon
}

public class FilterManagerIcon : MonoBehaviour
{
    // [Header("Filter List Panel")]
    // public GameObject FilterList;

    [Header("Filter Buttons")]
    public FilterButton[] filterButtons;

    [Header("Male Filter Icons & Labels")]
    public Sprite[] maleIcons;
    public string[] maleLabels;

    [Header("Female Filter Icons & Labels")]
    public Sprite[] femaleIcons;
    public string[] femaleLabels;

    [Header("Gender Mode (0=Unknown, 1=Male, 2=Female)")]
    [Range(0, 2)]
    public int genderLevel = 0; // 0 unknown, 1 cowo, 2 cewe

    [Header("Script Reference")]
    public UIStatusManager UIStatus;


    void Start()
    {
        ResetFilterIcon();
    }

    public void UpdateFilterUI(int gender, float percentage)
    {
        Sprite[] targetIcons = null;
        string[] targetLabels = null;
        String textToTampil;

            switch (gender)
            {
                case 1: // Male
                        // FilterList.SetActive(true);
                    targetIcons = maleIcons;
                    targetLabels = maleLabels;
                    textToTampil = percentage > 0 ? "Laki-Laki " + percentage + "%" : "Laki-Laki";
                    UIStatus.SetStatusText(textToTampil, "GENDERMALE");
                    break;
                case 2: // Female
                        // FilterList.SetActive(true);
                    targetIcons = femaleIcons;
                    targetLabels = femaleLabels;
                    textToTampil = percentage > 0 ? "Perempuan " + percentage + "%" : "Laki-Laki";
                    UIStatus.SetStatusText(textToTampil, "GENDERFEMALE");
                    break;
                default: // Unknown
                    targetIcons = null;
                    targetLabels = null;
                    // FilterList.SetActive(false);
                    UIStatus.SetStatusText("Wajah tidak ditemukan", null);
                    break;
            }

        for (int i = 0; i < filterButtons.Length; i++)
        {
            // ICON
            if (targetIcons != null && i < targetIcons.Length && targetIcons[i] != null)
                filterButtons[i].icon.sprite = targetIcons[i];
            else
                filterButtons[i].icon.sprite = null; // none

            // LABEL
            if (targetLabels != null && i < targetLabels.Length)
                filterButtons[i].label.text = targetLabels[i];
            else
                filterButtons[i].label.text = $"Filter{i + 1}";
        }
    }

    // Fungsi yang bisa dipanggil dari script deteksi
    public void OnGenderDetected(string gender, float percentage)
    {
        gender = gender.ToLower();

        if (gender.Contains("male"))
            genderLevel = 1;
        else if (gender.Contains("female"))
            genderLevel = 2;
        else
            genderLevel = 0;

        UpdateFilterUI(genderLevel, percentage);
    }

    public void ResetFilterIcon()
    {
        UpdateFilterUI(0, 0);
    }
}

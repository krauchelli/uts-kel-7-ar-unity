using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class Status
{
    public Image StatusBackground; // background dari statusnya
    public TextMeshProUGUI StatusText; // textnya
}

[System.Serializable]
public class FilterButton
{
    public Button button;       // Tombol utamanya
    public Image icon;          // Gambar icon di dalam tombol
    public TextMeshProUGUI label; // Teks di bawah icon
}

public class FilterManagerIcon : MonoBehaviour
{
    [Header("Filter Buttons")]
    public FilterButton[] filterButtons;

    [Header("Male Filter Icons & Labels")]
    public Sprite[] maleIcons;
    public string[] maleLabels;

    [Header("Female Filter Icons & Labels")]
    public Sprite[] femaleIcons;
    public string[] femaleLabels;

    [Header("Status Text")]
    public Status Status;

    [Header("Gender Mode (0=Unknown, 1=Male, 2=Female)")]
    [Range(0, 2)]
    public int genderLevel = 0; // 0 unknown, 1 cowo, 2 cewe

    private Color unknownColor = new Color32(255, 255, 255, 70);
    private Color maleColor = new Color32(70, 150, 255, 70);
    private Color femaleColor = new Color32(230, 120, 255, 70);


    void Start()
    {
        UpdateFilterUI(genderLevel);
    }

    public void UpdateFilterUI(int gender)
    {
        Sprite[] targetIcons = null;
        string[] targetLabels = null;

        switch (gender)
        {
            case 1: // Male
                targetIcons = maleIcons;
                targetLabels = maleLabels;
                Status.StatusBackground.color = maleColor;
                Status.StatusText.text = "Laki-laki";
                break;
            case 2: // Female
                targetIcons = femaleIcons;
                targetLabels = femaleLabels;
                Status.StatusBackground.color = femaleColor;
                Status.StatusText.text = "Perempuan";

                break;
            default: // Unknown
                targetIcons = null;
                targetLabels = null;
                Status.StatusBackground.color = unknownColor;
                Status.StatusText.text = "Wajah tidak ditemukan";
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
    public void OnGenderDetected(string gender)
    {
        gender = gender.ToLower();

        if (gender.Contains("male"))
            genderLevel = 1;
        else if (gender.Contains("female"))
            genderLevel = 2;
        else
            genderLevel = 0;

        UpdateFilterUI(genderLevel);
    }

    public void ResetFilterIcon()
    {
        UpdateFilterUI(0);
    }
    
}

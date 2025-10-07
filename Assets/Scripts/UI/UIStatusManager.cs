using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class Status
{
    public Image StatusBackground; // background dari statusnya
    public TextMeshProUGUI StatusText; // textnya
}

public class UIStatusManager : MonoBehaviour
{

    [Header("Status Text")]
    public Status Status;

    private Color unknownColor = new Color32(255, 255, 255, 70);
    private Color maleColor = new Color32(70, 150, 255, 70);
    private Color femaleColor = new Color32(230, 120, 255, 70);
    private Color errorColor = new Color32(255, 100, 100, 70);

    public bool IsStatusLabelNull()
    {
        return Status == null;
    }

    public void SetStatusText(string text, string textType)
    {

        if (!IsStatusLabelNull())
        {
            if (textType == "ERROR")
            {
                Status.StatusBackground.color = errorColor;
            }
            else if (textType == "GENDERMALE")
            {
                Status.StatusBackground.color = maleColor;
            }
            else if (textType == "GENDERFEMALE")
            {
                Status.StatusBackground.color = femaleColor;
            }
            else
            {
                Status.StatusBackground.color = unknownColor;
            }
            Status.StatusText.text = text;
        }

    }
}

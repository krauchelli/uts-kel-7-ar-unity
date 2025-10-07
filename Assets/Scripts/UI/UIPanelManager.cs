using UnityEngine;

public class UIPanelManager : MonoBehaviour
{
    public GameObject PanelMain;
    public GameObject PanelFilter;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        OpenMainPanel();
    }

    public void OpenFilterPanel()
    {
        PanelFilter.SetActive(true);
        PanelMain.SetActive(false);
    }

    public void OpenMainPanel()
    {
        PanelMain.SetActive(true);
        PanelFilter.SetActive(false);
    }

}

using System.Collections;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class FaceDetection : MonoBehaviour
{

    public ARFaceManager faceManager;
    public FilterManagerIcon filterManagerIcon;

    [Header("Toggle Detection Button")]
    public UnityEngine.UI.Image toggleFaceDetectionImage;
    public Sprite onSprite;
    public Sprite offSprite;

    public float delay;
    private bool isDetecting = false;
    private Coroutine detectionCorountine;

    public bool IsFaceDetected()
    {
        return (faceManager.trackables.count > 0);
    }

    public void DetectFace()
    {
        if (faceManager == null || filterManagerIcon == null)
        {
            Debug.LogError("Face manager or Filter manager icon not found!");
            return;
        }

        if (IsFaceDetected())
        {
            filterManagerIcon.OnGenderDetected("male");
        }
        else
        {
            filterManagerIcon.OnGenderDetected("unknown");
        }
    }

    public void ToggleFaceDetectionLoop()
    {
        isDetecting = !isDetecting;
        toggleFaceDetectionImage.sprite = isDetecting ? onSprite : offSprite;
        if (isDetecting)
        {
            Debug.Log("Face Detection On");
            detectionCorountine = StartCoroutine(FaceDetectionLoop());
        }
        else
        {
            Debug.Log("Face Detection Off");
            if (detectionCorountine != null) StopCoroutine(detectionCorountine);
        }
    }

    IEnumerator FaceDetectionLoop() {
        while (true)
        {
            DetectFace();
            yield return new WaitForSeconds(delay);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

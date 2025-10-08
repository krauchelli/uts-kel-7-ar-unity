// FaceGender.cs (Revised)
using UnityEngine;

public class FaceGender : MonoBehaviour
{
    // A public property to store the result for this specific face.
    // Other scripts can set this value.
    public string PredictedGender { get; private set; }
    public float Confidence { get; private set; }

    // This public method allows manager scripts to assign the result.
    public void SetPredictionResult(string gender, float confidence)
    {
        PredictedGender = gender;
        Confidence = confidence;
        
        Debug.Log($"Face {this.gameObject.name} result set to: {gender} with {confidence}% confidence.");

        // --- FUTURE LOGIC GOES HERE ---
        // For example, you could call a function to show a 3D model based on the result.
        // ShowAppropriateAccessory(gender);
    }
}
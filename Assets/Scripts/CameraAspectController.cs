using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraAspectController : MonoBehaviour
{
    public float targetAspectRatio = 1.5f; // 15:10 aspect

    void Start()
    {
        UpdateCameraViewport();
    }

    void UpdateCameraViewport()
    {
        Camera cam = GetComponent<Camera>();

        float windowAspect = (float)Screen.width / (float)Screen.height;
        float scaleHeight = windowAspect / targetAspectRatio;

        if (scaleHeight < 1.0f)
        {
            // Add letterbox
            Rect rect = new Rect(0, (1.0f - scaleHeight) / 2.0f, 1, scaleHeight);
            cam.rect = rect;
        }
        else
        {
            // Add pillarbox
            float scaleWidth = 1.0f / scaleHeight;
            Rect rect = new Rect((1.0f - scaleWidth) / 2.0f, 0, scaleWidth, 1);
            cam.rect = rect;
        }
    }

    // Optional: handle resolution changes on the fly
    void OnPreCull()
    {
        GL.Clear(true, true, Color.black); // Fills letterbox areas with black
    }
}


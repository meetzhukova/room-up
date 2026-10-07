using UnityEngine;

[RequireComponent(typeof(Camera))]
[DisallowMultipleComponent]
public class CameraFitter : MonoBehaviour
{
    [SerializeField] private float contentWidth = 21f;
    [SerializeField] private float contentHeight = 32f;

    private Camera targetCamera;
    private int lastScreenWidth;
    private int lastScreenHeight;

    private void Awake()
    {
        targetCamera = GetComponent<Camera>();
        Fit();
    }

    private void LateUpdate()
    {
        if (Screen.width != lastScreenWidth || Screen.height != lastScreenHeight)
        {
            Fit();
        }
    }

    private void Fit()
    {
        lastScreenWidth = Screen.width;
        lastScreenHeight = Screen.height;

        float aspect = (float)Screen.width / Screen.height;
        float sizeForHeight = contentHeight / 2f;
        float sizeForWidth = contentWidth / (2f * aspect);

        targetCamera.orthographicSize = Mathf.Max(sizeForHeight, sizeForWidth);
    }
}

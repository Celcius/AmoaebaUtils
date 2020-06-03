using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AmoaebaUtils
{
public class CalculateOrtographicWindowBounds : ScriptableObject
{
    [SerializeField]
    private CameraVar mainCamera;

    [SerializeField]
    private Vector2Var windowBounds;

    private void OnEnable()
    {
        if(mainCamera.Value != null)
        {
            UpdateBounds(null, mainCamera.Value);
        }
        mainCamera.OnChange += UpdateBounds;
    }

    private void OnDisable()
    {
        mainCamera.OnChange -= UpdateBounds;
    }

    private void UpdateBounds(Camera oldCam, Camera newCam)
    {
        if(newCam == null || !newCam.orthographic)
        {
            windowBounds.Value = Vector2.zero;
            return;
        }

        Vector2 bounds = UnityEngineUtils.WorldOrthographicSize(newCam);
        bounds = Input.deviceOrientation == DeviceOrientation.Portrait?
                 new Vector2(Mathf.Min(bounds.x, bounds.y), Mathf.Max(bounds.x, bounds.y)) : 
                 new Vector2(Mathf.Max(bounds.x, bounds.y), Mathf.Min(bounds.x, bounds.y));
        windowBounds.Value = bounds;
    }
}
}
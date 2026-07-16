using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AmoaebaUtils
{
[ExecuteInEditMode]
public class BillboardPlaneFromVar : BillboardPlane
{
    [SerializeField]
    private TransformVar _mainCamera;

    private void Awake()
    {
        _mainCamera.OnChange += OnCameraChanged;    
    }

    private void OnDestroy()
    {
        _mainCamera.OnChange -= OnCameraChanged;
    }

        private void OnCameraChanged(Transform oldValue, Transform newValue)
    {
        _cameraTransform = newValue.transform;    
    }
}
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AmoaebaUtils
{
public class InstantiateTimerOnScreen : InstantiateTimer
{
    [SerializeField]
    private CameraVar camera;

    [SerializeField]
    private float cameraScale = 1.0f;
        
    [SerializeField]
    private Vector2 cameraSizeInc = Vector2.zero;

    [SerializeField]
    private Vector2 cameraOffset = Vector2.zero;

    protected override void Start()
    {
        StopInstantiation();
    }

    private void Update() 
    {
        
        if(!IsRunning && IsOnScreen())
        {
            StartInstantiation();
        }
    }

    private bool IsOnScreen()
    {
        if(camera == null || camera.Value == null)
        {
            return false;
        }
        Camera main = camera.Value;

        Vector2 size = UnityEngineUtils.WorldOrthographicSize(main);
        Bounds bounds = new Bounds((Vector2)main.transform.position + cameraOffset, 
                                   size * cameraScale + cameraSizeInc);
        return bounds.Contains((Vector2)transform.position);
    }

}
}
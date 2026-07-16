using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AmoaebaUtils
{
[ExecuteInEditMode]
public class BillboardPlane : MonoBehaviour
{
    [SerializeField]
    protected Transform _cameraTransform;

    void Update()
    {
        if(_cameraTransform == null)
        {
            return;
        }

        transform.forward = transform.position - _cameraTransform.position;
    }
}
}
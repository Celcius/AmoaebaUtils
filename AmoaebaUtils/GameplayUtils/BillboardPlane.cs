using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AmoaebaUtils
{
public class BillboardPlane : MonoBehaviour
{
    [SerializeField]
    private Transform cameraTransform;

    void Update()
    {
        if(cameraTransform == null)
        {
            return;
        }

        transform.forward = transform.position - cameraTransform.position;
    }
}
}
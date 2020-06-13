using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AmoaebaUtils
{
[RequireComponent(typeof(FollowTransform))]
public class ChangeFollowOffsetOnScreenSize : MonoBehaviour
{    
    [SerializeField]
    private Vector2Var windowBounds;
    
    [SerializeField]
    private Vector2 percentageOffset;
    

    private FollowTransform follow;
    private void Awake()
    {
        follow = GetComponent<FollowTransform>();
         if(windowBounds.Value.x != 0 && windowBounds.Value.y != 0)
         {
             OnBoundsChange(Vector2.zero, windowBounds.Value);
         }

         windowBounds.OnChange += OnBoundsChange;
    }

    private void OnDestroy()
    {
        windowBounds.OnChange -= OnBoundsChange;
    }

    private void OnBoundsChange(Vector2 oldVal, Vector2 newVal)
    {
        follow.offset = (Vector3)Vector2.Scale(percentageOffset, newVal) + follow.offset.z * Vector3.forward;
    }
}
}
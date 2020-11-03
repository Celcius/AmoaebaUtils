using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AmoaebaUtils
{
public class AnimateFramesDestroyInstantiate : AnimateFramesAndDestroy
{
    public Transform ToInstantiate;
    public Vector3 instantiateOffset;
    public Vector3 scale = Vector3.one;
    private bool overrideRotation;
    private Quaternion instanceRotation;

    protected override void FinishedAnimating()
    {
        if(ToInstantiate != null)
        {
            Transform instance = Instantiate(ToInstantiate, 
                                             transform.position + instantiateOffset, 
                                             overrideRotation? instanceRotation : ToInstantiate.rotation);
            instance.localScale = Vector3.Scale(ToInstantiate.localScale, scale);
        }
        base.FinishedAnimating();
    }
    
    public void ClearOverrideRotation()
    {
        overrideRotation = false;
    }

    public void SetOverrideRotation(Quaternion rotation)
    {
        overrideRotation = true;
        instanceRotation = rotation;
    }

}
}
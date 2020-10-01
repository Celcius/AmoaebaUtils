using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace  AmoaebaUtils
{
public abstract class TransformMovement : MonoBehaviour
{

    [SerializeField]
    private bool canMove = true;

    public  bool CanMove 
    { 
        get { return canMove; }
        set { canMove = value; }
    } 

    protected Vector3 axisMultipliers = Vector3.one;
    
    public Vector3 AxisMultipliers
    {
        get { return axisMultipliers;}
        set { SetAxisMultiplier(value); }
    }
    
    public virtual void SetAxisMultiplier(Vector3 axis)
    {
        axisMultipliers = axis;
    }

    private void Update()
    {
        if(!CanMove)
        {
            return;
        }    
        Move();
    }

    protected abstract void Move();
    public abstract void SetElapsedTime(float elapsed);
    public virtual float GetDeltaTime()
    {
         return Time.deltaTime;
    }
}
}
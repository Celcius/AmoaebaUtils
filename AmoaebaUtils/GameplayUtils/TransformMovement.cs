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

    [HideInInspector]
    public Vector3 AxisMultipliers = new Vector3(1,1,1);

    private void Update()
    {
        if(!CanMove)
        {
            return;
        }    
        Move();
    }

    protected abstract void Move();
}
}
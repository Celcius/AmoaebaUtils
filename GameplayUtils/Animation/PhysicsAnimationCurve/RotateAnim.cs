using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;

namespace AmoaebaUtils
{
public class RotateAnim : CurveAnim
{

    [SerializeField]
    private Vector3 speedVec = Vector3.one;

    [SerializeField]
    private bool _randomX = false;

    [SerializeField]
    private Vector2 _randomRangeX;

    [SerializeField]
    private bool _randomY;

    [SerializeField]
    private Vector2 _randomRangeY;

    [SerializeField]
    private bool _randomZ;

    [SerializeField]
    private Vector2 _randomRangeZ;

    [SerializeField]
    private BoolVector3 _randomDir;
    

    protected override void Start()
    {
        base.Start();

        speedVec.x = (_randomDir.x? Mathf.Sign(Random.Range(-10,10)) : 1.0f) * (_randomX? MathUtils.RandomFromVec(_randomRangeX) : speedVec.x);
        speedVec.y = (_randomDir.y? Mathf.Sign(Random.Range(-10,10)) : 1.0f) * (_randomY? MathUtils.RandomFromVec(_randomRangeY) : speedVec.y);
        speedVec.z = (_randomDir.z? Mathf.Sign(Random.Range(-10,10)) : 1.0f) * (_randomZ? MathUtils.RandomFromVec(_randomRangeZ) : speedVec.z);
    }

    protected override void OnChange(float evaluatedVal)
    {
        transform.Rotate(evaluatedVal * speedVec, Space.Self);
    }
}
}
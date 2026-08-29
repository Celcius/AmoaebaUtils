using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private Vector3 _randomizedVector;

    protected override void Start()
    {
        base.Start();

        _randomizedVector = Vector3.zero;
        _randomizedVector.x = _randomX? MathUtils.RandomFromVec(_randomRangeX) : 0;
        _randomizedVector.y = _randomX? MathUtils.RandomFromVec(_randomRangeY) : 0;
        _randomizedVector.z = _randomX? MathUtils.RandomFromVec(_randomRangeZ) : 0;
    }

    protected override void OnChange(float evaluatedVal)
    {
        transform.Rotate(evaluatedVal * (speedVec + _randomizedVector), Space.Self);
    }
}
}
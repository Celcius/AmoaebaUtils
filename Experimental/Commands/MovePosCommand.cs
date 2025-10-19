using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AmoaebaUtils
{

public class MovePosCommand : PooledCommand
{
    private Vector3 offset;
    private Transform entity;

    public MovePosCommand() {}

    public override bool Execute(Action callback = null)
    {
        entity.position += offset;
        return true;
    }
    
    public override bool Undo(Action callback = null)
    {
        entity.position -= offset;
        return true;
    }
    
    public void SetParams(Transform entity, Vector3 posOffset)
    {
        this.entity = entity;
        offset = posOffset;
    }
}
}
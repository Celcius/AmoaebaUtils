using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AmoaebaUtils
{
public static class StructUtils
{
    public static Vector2 ModulateMatrixPosition(float index, Vector2Int layout, BoolVector2 center, BoolVector2 alternate)
    {
        float x = ModulateVectorPosition(index % layout.x, layout.x, center.x,alternate.x);
        float y = ModulateVectorPosition((int)index / layout.x, layout.y , center.y, alternate.y);
        return new Vector2(x,y);
    }

    public static float ModulateVectorPosition(float index, int total, bool center, bool alternate)
    {
        int sign = alternate? index % 2 == 0? -1 : 1 : 1;
        float modulatedIndex = alternate? ((int) Mathf.Ceil(index/2.0f)) : index;

        if(center)
        {
            modulatedIndex -= alternate? sign *(total % 2 == 0? 0.5f : 0) :
                                total/2.0f - 0.5f;
        }
        return sign * modulatedIndex;
    }
}

[Serializable]
public class BooledTypedVector2<V, T> where V : TypedVector2<T>
{
    public BoolVector2 checks;
    public V values;

    public bool HasAnyCheck => checks != null && (checks.x || checks.y);

    public BooledTypedVector2(BoolVector2 checks, V values)
    {
        this.checks = checks;
        this.values = values;
    }

    public TypedVector2<T> Evaluate(TypedVector2<T> onFailChecks)
    {
        return new TypedVector2<T>(EvaluateX(onFailChecks.x),
                                   EvaluateY(onFailChecks.y));
    }
    
    public T EvaluateX(T onFailCheck)
    {
        return checks.x? values.x : onFailCheck;
    }

    public T EvaluateY(T onFailCheck)
    {
        return checks.y? values.y : onFailCheck;
    }
}

[Serializable]
public class BooledTypedVector3<V, T> where V : TypedVector3<T>
{
    public BoolVector3 checks;
    public V values;

    public BooledTypedVector3(BoolVector3 checks, V values)
    {
        this.checks = checks;
        this.values = values;
    }

    public TypedVector3<T> Evaluate(TypedVector3<T> onFailChecks)
    {
        return new TypedVector3<T>(EvaluateX(onFailChecks.x),
                                   EvaluateY(onFailChecks.y),
                                   EvaluateZ(onFailChecks.z));
    }

    public T EvaluateX(T onFailCheck)
    {
        return checks.x? values.x : onFailCheck;
    }

    public T EvaluateY(T onFailCheck)
    {
        return checks.y? values.y : onFailCheck;
    }

    public T EvaluateZ(T onFailCheck)
    {
        return checks.z? values.z : onFailCheck;
    }
}


[Serializable]
public class BooledVector2 : BooledTypedVector2<FloatVector2, float>
{
    public BooledVector2(BoolVector2 checks, FloatVector2 values)  : base(checks, values) {}

    public BooledVector2(BoolVector2 checks, Vector2 values)  : base(checks, new FloatVector2(values)) {}

    public Vector2 EvaluateVec(Vector2 onFailChecks)
    {
        return new Vector2(EvaluateX(onFailChecks.x),
                           EvaluateY(onFailChecks.y));
    }
}

[Serializable]
public class BooledVector3 : BooledTypedVector3<FloatVector3, float>
{
    public BooledVector3(BoolVector3 checks, FloatVector3 values)  : base(checks, values) {}
    public BooledVector3(BoolVector3 checks, Vector3 values)  : base(checks, new FloatVector3(values)) {}
    

    public Vector3 EvaluateVec(Vector3 onFailChecks)
    {
        return new Vector3(EvaluateX(onFailChecks.x),
                           EvaluateY(onFailChecks.y),
                           EvaluateY(onFailChecks.z));
    }
}



[Serializable]
public class TypedVector2<T> 
{
    public T x;
    public T y;

    public TypedVector2(T x, T y)
    {
        this.x = x;
        this.y = y;
    }
}

[Serializable]
public class TypedVector3<T> : TypedVector2<T>
{
    public T z;

    public TypedVector3(T x, T y, T z) : base(x,y)
    {
        this.z = z;
    }
}

[Serializable]
public class BoolVector2 : TypedVector2<bool> 
{
    public BoolVector2(bool x, bool y): base(x,y) { }
}

[Serializable]
public class BoolVector3 : TypedVector3<bool> 
{
    public BoolVector3(bool x, bool y, bool z): base(x,y,z) { }
}

[Serializable]
public class FloatVector2 : TypedVector2<float> 
{
    public FloatVector2(float x, float y): base(x,y) { }
    public FloatVector2(Vector2 values): base(values.x, values.y) { }
}

[Serializable]
public class FloatVector3 : TypedVector3<float> 
{
    public FloatVector3(float x, float y, float z): base(x,y,z) { }
    public FloatVector3(Vector3 values): base(values.x, values.y, values.z) { }
}

[Serializable]
public class IntVector2 : TypedVector2<int> 
{
    public IntVector2(int x, int y): base(x,y) { }
}

[Serializable]
public class IntVector3 : TypedVector3<int> 
{
    public IntVector3(int x, int y, int z): base(x,y,z) { }
}
}

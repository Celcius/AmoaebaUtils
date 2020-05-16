using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AmoaebaUtils
{
public class UndrawableGraphic : Graphic
{
    protected override void OnPopulateMesh(VertexHelper vh)
    {
    }
    protected override void UpdateGeometry()
    {
    }

    protected override void UpdateMaterial()
    {
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(UndrawableGraphic))]
public class UndrawableGraphicEditor : Editor
{
    public override void OnInspectorGUI()
    {
    }
}
#endif
}
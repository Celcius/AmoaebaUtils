using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using AmoaebaUtils;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UI;
#endif

namespace AmoaebaUtils
{
[System.Serializable]
public class FloatVarToSlider : Slider
{
    [SerializeField]
    private FloatVar slideVar;
    public FloatVar SliderVar
    {
        get { return slideVar; }    
        set 
        { 
            if(slideVar != null)
            {
                slideVar.OnChange -= OnVarChanged;
            }
            slideVar = value; 
        }
    }
    
    [SerializeField]
    private bool storeValue = true;
    protected override void OnEnable()
    {
        base.OnEnable();
        if(slideVar != null)
        {
            slideVar.OnChange += OnVarChanged;
            OnVarChanged(0, slideVar.Value);
        }
        onValueChanged.AddListener(OnValueChanged);
    }

    protected void OnVarChanged(float oldVal, float newVal)
    {
        this.value =  GetClampedVal(newVal); 
    }

    protected override void OnDestroy()
    {
        if(slideVar != null)
        {
            slideVar.OnChange -= OnVarChanged;
        }

        if(storeValue)
        {
            slideVar.SetupValue = slideVar.Value;
        }
        
        onValueChanged.RemoveListener(OnValueChanged);
        base.OnDestroy();
    }

    private void OnValueChanged(float val)
    {
        if(slideVar != null)
        {
            slideVar.Value = GetClampedVal(val); 
            if(storeValue)
            {
                slideVar.SetupValue = slideVar.Value;
            }      
        }
    }

    private float GetClampedVal(float val)
    {
        return Mathf.Clamp(val, minValue, maxValue);
    }
}



#if UNITY_EDITOR
[CustomEditor(typeof(FloatVarToSlider))]
public class FloatVarToSliderEditor : SliderEditor 
{
        FloatVarToSlider sliderTarget;

        protected override void OnEnable()
        {
            base.OnEnable();
            sliderTarget = (FloatVarToSlider)target;
        }

        public override void OnInspectorGUI ()
        {
           base.OnInspectorGUI();
          
           EditorGUILayout.Space();

           FloatVar var = EditorGUILayout.ObjectField("Slider Variable", sliderTarget.SliderVar, typeof(FloatVar), false) as FloatVar;
            
           if(var != sliderTarget.SliderVar)
           {
               sliderTarget.SliderVar = var;
               EditorUtility.SetDirty(sliderTarget);  

           }
        }
}
#endif 
}
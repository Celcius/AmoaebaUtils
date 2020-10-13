using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UI;
#endif

namespace AmoaebaUtils
{

[System.Serializable]
public class BoolVarToggle : Toggle
{
    [SerializeField]
    private BoolVar toggleVar;
    public BoolVar ToggleVar
    {
        get { return toggleVar; }    
        set 
        { 
            if(toggleVar != null)
            {
                toggleVar.OnChange -= OnVarChanged;
            }
            toggleVar = value; 
        }
    }
    
    protected override void OnEnable()
    {
        base.OnEnable();
        if(toggleVar != null)
        {
            toggleVar.OnChange += OnVarChanged;
            OnVarChanged(false, toggleVar.Value);
        }
        onValueChanged.AddListener(OnValueChanged);
        
    }

    protected void OnVarChanged(bool oldVal, bool newVal)
    {
        this.isOn = toggleVar.Value;
    }
    protected override void OnDestroy()
    {
        if(toggleVar != null)
        {
            toggleVar.OnChange -= OnVarChanged;
        }
        onValueChanged.RemoveListener(OnValueChanged);
        base.OnDestroy();
    }

    private void OnValueChanged(bool value)
    {
        if(toggleVar != null)
        {
            toggleVar.Value = this.isOn;            
        }
    }
}


#if UNITY_EDITOR
[CustomEditor(typeof(BoolVarToggle))]
public class BoolVarToggleEditor : ToggleEditor 
{
        BoolVarToggle toggleTarget;

        protected override void OnEnable()
        {
            base.OnEnable();
            toggleTarget = (BoolVarToggle)target;
        }

        public override void OnInspectorGUI ()
        {
           base.OnInspectorGUI();
          
           EditorGUILayout.Space();

           BoolVar var = EditorGUILayout.ObjectField("Toggle Variable", toggleTarget.ToggleVar, typeof(BoolVar), false) as BoolVar;
           if(var != toggleTarget.ToggleVar)
           {
               toggleTarget.ToggleVar = var;
               EditorUtility.SetDirty(toggleTarget);  

           }
        }
}
#endif 
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace AmoaebaUtils
{
public class UpdateTextOnVarChange<T, V> : MonoBehaviour 
    where T : ScriptVar<V>
{
    [SerializeField]
    protected T var;

    [SerializeField]
    protected Text label;

    [SerializeField]
    protected TextMeshProUGUI TMPLabel;
    
    [SerializeField]
    protected string format = "{0}";

    protected virtual void Start()
    {
        if(label == null && TMPLabel == null)
        {
            return;
        }
        var.OnChange += UpdateLabel;
        UpdateLabel(var.Value, var.Value);
    }

    // Update is called once per frame
    private void OnDestroy() 
    {
        var.OnChange -= UpdateLabel;
    }

    private void UpdateLabel(V oldVal, V newVal)
    {
        string newText = GetText(oldVal, newVal);
        
        if(label != null)
        {
            label.text = newText;
        }

        if(TMPLabel != null)
        {
            TMPLabel.text = newText;
        }
    }

    protected virtual string GetText(V oldVal, V newVal)
    {
        return string.Format(format,newVal);
    }
}
}
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
    
    [SerializeField]
    protected bool replaceNewLine = true;

    [SerializeField]
    private bool animate = false;

    [SerializeField]
    private AnimationCurve animationCurve;

    [SerializeField]
    private Vector3 animationScaleTo;
    
    [SerializeField]
    private Color animationColorTo;
    private Color startColor;
    private Vector3 startScale;

    [SerializeField]
    private bool animateOnStart;
    private List<Graphic>[] graphics;
    
    private IEnumerator animationRoutine = null;

    protected virtual void Start()
    {
        if(label == null && TMPLabel == null)
        {
            return;
        }
       

        if(label != null)
        {
            startColor = label.color;
            startScale = label.transform.localScale;
        }
        else
        {
            startColor = TMPLabel.color;
            startScale = TMPLabel.transform.localScale;
        }

        var.OnChange += UpdateLabel;
        UpdateLabel(var.Value, var.Value, !animateOnStart);
    }

    // Update is called once per frame
    private void OnDestroy() 
    {
        var.OnChange -= UpdateLabel;
    }

    protected virtual void UpdateLabel(V oldVal, V newVal)
    {
        UpdateLabel(oldVal, newVal, false);
    }

    protected virtual void UpdateLabel(V oldVal, V newVal, bool ignoreAnimation)
    {
        string newText = GetText(oldVal, newVal);
        if(replaceNewLine)
        {
            newText = newText.Replace("\\n", "\n");
        }        
        
        if(label != null)
        {
            label.text = newText;
        }

        if(TMPLabel != null)
        {
            TMPLabel.text = newText;
        }

        if(animate && !ignoreAnimation)
        {
            if(animationRoutine != null)
            {
                StopCoroutine(animationRoutine);
            }
            animationRoutine = AnimateRoutine(GetAnimationColorTo(oldVal, newVal));
            StartCoroutine(animationRoutine);
        }
    }

    protected virtual Color GetAnimationColorTo(V oldval, V newVal)
    {
        return animationColorTo;
    }
    
    private IEnumerator AnimateRoutine(Color animationColorTo)
    {
        if(animationCurve.keys.Length <= 0)
        {
            animationRoutine = null;
            yield break;
        }

        animationColorTo.a = 1.0f;
        Graphic animationLabel = (label == null)? (Graphic)TMPLabel : (Graphic)label;
        Color colorDelta = animationColorTo - animationLabel.color;
        Vector3 scaleDelta = animationScaleTo - animationLabel.transform.localScale; 
        Keyframe lastKey = animationCurve.keys[animationCurve.keys.Length-1];
        float duration = lastKey.time;
        float toElapse = 0;
        
        while(toElapse < duration)
        {
            float ratio = toElapse/duration;
            float val = animationCurve.Evaluate(ratio);

            animationLabel.color = val * colorDelta + startColor;
            animationLabel.transform.localScale = val * scaleDelta + startScale;

            yield return new WaitForEndOfFrame();
            toElapse += Time.deltaTime;
        }

        animationLabel.color = startColor;
        animationLabel.transform.localScale = startScale;

        animationRoutine = null;
    }

    protected virtual string GetText(V oldVal, V newVal)
    {
        return string.Format(format,newVal);
    }
}
}
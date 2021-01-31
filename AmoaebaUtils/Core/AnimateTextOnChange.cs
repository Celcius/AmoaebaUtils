using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AmoaebaUtils
{
public class AnimateTextOnChange<T,V> : UpdateTextOnVarChange<T, V> where T : ScriptVar<V>
{
    private IEnumerator updateCoroutine = null;

    [SerializeField]
    private float timePerLetter = 0.02f;
    private int textIndex = 0;
    protected override void UpdateLabel(V oldVal, V newVal)
    {
        textIndex = 0;
        SetText("");

        if(updateCoroutine != null)
        {
            StopCoroutine(updateCoroutine);
        }

        updateCoroutine = AnimateText(GetText(oldVal, newVal));
        StartCoroutine(updateCoroutine);
    }

    private IEnumerator AnimateText(string goalText)
    {
        while(textIndex < goalText.Length)
        {
            SetText(goalText.Substring(0,textIndex));
            yield return new WaitForSeconds(timePerLetter);
            textIndex++;
        }
        SetText(goalText);
        updateCoroutine = null;
    }
}
}
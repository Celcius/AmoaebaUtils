﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace AmoaebaUtils
{
public class FadeLabelFromStringVar : MonoBehaviour
{
    protected TextMeshProUGUI label;

    [SerializeField]
    private StringVar stringVar;

    [SerializeField]
    protected float fadeInTime = 2.0f;
    [SerializeField]
    protected float timeInScreen = 5.0f;
    private IEnumerator currentRoutine = null;

    void Awake()
    { 
        label = GetComponent<TextMeshProUGUI>();
        stringVar.OnChange += LabelChanged;
        Hide();
    }

    private void LabelChanged(string oldVal, string newVal)
    {
        if(string.IsNullOrEmpty(newVal))
        {
            if(label.alpha > 0)
            {
                FadeOut();
            }
        }
        else
        {
            label.text = newVal;
            FadeIn();
        }

    }

    private void FadeOut()
    {
        if(currentRoutine != null)
        {
            StopCoroutine(currentRoutine);
        }
        currentRoutine = FadeTo(0.0f);
        StartCoroutine(currentRoutine);
    }

    private void FadeIn()
    {
        if(currentRoutine != null)
        {
            StopCoroutine(currentRoutine);
        }
        currentRoutine = FadeInRoutine();
        StartCoroutine(currentRoutine);
    }

    protected virtual IEnumerator FadeTo(float goal)
    {
        float delta = (goal - label.alpha) / fadeInTime;
        float elapsed = 0;
        float alpha = label.alpha;
        Color c = label.color;
        while(elapsed <= fadeInTime)
        {
            yield return new WaitForEndOfFrame();
            alpha += Time.deltaTime * delta;
            elapsed += Time.deltaTime;
            c.a = alpha;
            label.color = c;
        }
        c.a = goal;
    }

    private IEnumerator FadeInRoutine()
    {
        yield return FadeTo(1.0f);

        yield return new WaitForSeconds(timeInScreen);

        stringVar.Value = "";
    }

    public void Hide()
    {
        Color c = label.color;
        c.a = 0.0f;
        label.color = c;
        stringVar.Value = "";
        if(currentRoutine != null)
        {
            StopCoroutine(currentRoutine);
            currentRoutine = null;
        }
    }
}
}
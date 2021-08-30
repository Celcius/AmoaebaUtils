using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AmoaebaUtils
{
[RequireComponent(typeof(CanvasGroup))]
public class Menu : MonoBehaviour
{

    private CanvasGroup canvasGroup;

    public bool IsEnabled => canvasGroup.interactable;

    protected virtual void Awake() 
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }
    public virtual void EnableButtons()
    {
        canvasGroup.interactable = true;
    }

    public virtual void DisableButtons()
    {
        canvasGroup.interactable = false;
    }
}
}
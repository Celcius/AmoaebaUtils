using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using AmoaebaUtils;

public class BasicWindowOption : MonoBehaviour
{
    [SerializeField]
    protected TextMeshProUGUI TextLabel;

    [SerializeField]
    protected Button Button;

    [SerializeField]
    protected StringEvent NextNodeEvent;

    protected SpeechOption Option;

    public void Awake()
    {
        Button.onClick.AddListener( delegate
        {
            NextNodeEvent?.Invoke(Option.destinationNode);
        });
    }

    public void Destroy()
    {
        
        Button.onClick.RemoveAllListeners();
    }
    public void ShowButton(SpeechOption option)
    {
        ClearButton();
        
        if(TextLabel != null)
        {
            TextLabel.text = option.displayText;    
        }
        Option = option;
        gameObject.SetActive(true);
    }

    public void ClearButton()
    {
        if(TextLabel != null)
        {
            TextLabel.text = "";    
        }
    }

    public void HideButton()
    {
        gameObject.SetActive(false);
    }
}
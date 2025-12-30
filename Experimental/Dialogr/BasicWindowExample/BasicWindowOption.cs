using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using AmoaebaUtils;

public class BasicWindowOption : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI TextLabel;

    [SerializeField]
    private Button Button;

    [SerializeField]
    private StringEvent NextNodeEvent;

    private SpeechOption Option;

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
        TextLabel.text = option.displayText;
        Option = option;
        gameObject.SetActive(true);
    }

    public void ClearButton()
    {
        TextLabel.text = "";
    }

    public void HideButton()
    {
        gameObject.SetActive(false);
    }
}
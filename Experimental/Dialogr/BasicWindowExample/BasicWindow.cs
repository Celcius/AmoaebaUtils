using UnityEngine;
using Dialogr;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using UnityEngine.AI;
using NUnit.Framework;

public class BasicWindow : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI Nameplate;

    [SerializeField]
    private TextMeshProUGUI DialogLabel;

    [SerializeField]
    private BasicWindowOption OptionsPrefab;

    [SerializeField]
    private Transform OptionsHolder;

    private List<BasicWindowOption> PooledOptions = new List<BasicWindowOption>();

    private bool IsOpen = false;

    private const string CHARACTER_TAG = "character";
    private const string SHAKE_TEXT = "shakeText";

    private CharacterNameScriptable NamesInterpreter;

    public void ShowNode(SpeechNode node)
    {
        if(!IsOpen)
        {
            gameObject.SetActive(true); // TODO make cool animation and only do Display Scene after
            IsOpen = true;
        }

        DisplayNode(node);
    }

    void Awake()
    {
        gameObject.SetActive(false);
        IsOpen = false;
    }

    public void SetNamesScriptable(CharacterNameScriptable scriptable)
    {
        this.NamesInterpreter = scriptable;
    }

    private void DisplayNode(SpeechNode node)
    {
        ParseMetaActions(node.MetaActions);
        DialogLabel.text = node.Text;
        CreateButtons(node.Options);
    }

    private void ParseMetaActions(MetaAction[] metas)
    {
        Nameplate.text ="ERROR: Forgot to set character for this node";
        foreach(MetaAction meta in  metas)
        {
            if(meta.Action.CompareTo(CHARACTER_TAG) == 0)
            {
                Assert.True(meta.Values.Length ==1, "Incorrect Name format for name");
                Nameplate.text = NamesInterpreter.GetLabel(meta.Values[0]);
            }
        }
    }

    private void CreateButton(bool startEnabled = true)
    {
        BasicWindowOption newOption = Instantiate(OptionsPrefab, OptionsHolder);
        PooledOptions.Add(newOption);
        newOption.gameObject.SetActive(startEnabled);
    }

    private void CreateButtons(SpeechOption[] options)
    {
        DisableButtons();

        for(int i = 0; i < options.Length; i++)
        {
            if(i>= PooledOptions.Count)
            {
                CreateButton();
            }
            PooledOptions[i].ShowButton(options[i]);
        } 
    }

    private void DisableButtons()
    {
        for(int i = 0; i < PooledOptions.Count; i++)
        {
            PooledOptions[i].HideButton();
        }
    }
}

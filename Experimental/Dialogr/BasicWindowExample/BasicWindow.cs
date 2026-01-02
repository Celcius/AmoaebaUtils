using UnityEngine;
using Dialogr;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using UnityEngine.AI;
using NUnit.Framework;
using System.Collections;

public class BasicWindow : MonoBehaviour
{
    [SerializeField]
    protected TextMeshProUGUI Nameplate;

    [SerializeField]
    protected TextMeshProUGUI DialogLabel;

    [SerializeField]
    protected BasicWindowOption OptionsPrefab;

    [SerializeField]
    protected Transform OptionsHolder;

    [SerializeField]
    protected BasicWindowOption NextDialogOption;

    [SerializeField]
    protected Vector2 ShakeOffset;

    [SerializeField]
    protected Vector2 MaxShakeOffset;
    [SerializeField]
    protected Vector2 ShakeDelay;

    protected List<BasicWindowOption> PooledOptions = new List<BasicWindowOption>();

    protected bool IsOpen = false;

    protected const string CHARACTER_TAG = "character";
    protected const string SHAKE_TAG = "shake";

    protected CharacterNameScriptable NamesInterpreter;

    protected IEnumerator ShakeRoutine = null;
    protected Vector3 OriginalDialogLabelPosition;
    protected Bounds MaxShakeBounds;

    public virtual void ShowNode(SpeechNode node)
    {
        if(!IsOpen)
        {
            gameObject.SetActive(true); // TODO make cool animation and only do Display Scene after
            IsOpen = true;
        }

        DisplayNode(node);
    }

    protected virtual void Awake()
    {
        OriginalDialogLabelPosition = DialogLabel.transform.position;
        MaxShakeBounds = new Bounds(OriginalDialogLabelPosition, MaxShakeOffset);
        
        gameObject.SetActive(false);
        IsOpen = false;
    }

    public virtual void SetNamesScriptable(CharacterNameScriptable scriptable)
    {
        this.NamesInterpreter = scriptable;
    }

    protected virtual void DisplayNode(SpeechNode node)
    {
        ResetDialog();
        StopActions();
        ParseMetaActions(node);
        DialogLabel.text = node.Text;
        CreateButtons(node.Options);
    }

    protected virtual void ParseMetaActions(SpeechNode node)
    {
        MetaAction[] metas = node.MetaActions;
        Nameplate.text ="ERROR: Forgot to set character for this node";
        foreach(MetaAction meta in  metas)
        {
            if(meta.Action.CompareTo(CHARACTER_TAG) == 0)
            {
                Assert.True(meta.Values.Length ==1, "Incorrect Name format for name");
                Nameplate.text = NamesInterpreter.GetLabel(meta.Values[0]);
            }
            else if(meta.Action.CompareTo(SHAKE_TAG) == 0)
            {
                Assert.True(meta.Values.Length == 2, "Incorrect Name format for shake, expected intensity [0-1] and duration [-1,+]");
                try
                {
                    float intensity = float.Parse(meta.Values[0]);
                    float duration = float.Parse(meta.Values[1]);
                    ShakeRoutine = ShakeEvent(intensity, duration);
                    StartCoroutine(ShakeRoutine);    
                } 
                catch (Exception e)
                {
                    Debug.LogError("Unable to parse Shake values for " + node.Title);
                }

            }
        }
    }

    protected virtual void CreateButton(bool startEnabled = true)
    {
        BasicWindowOption newOption = Instantiate(OptionsPrefab, OptionsHolder);
        PooledOptions.Add(newOption);
        newOption.gameObject.SetActive(startEnabled);
    }

    protected virtual void CreateButtons(SpeechOption[] options)
    {
        DisableButtons();

        if(options.Length == 1 && string.IsNullOrEmpty(options[0].displayText))
        {
            NextDialogOption.ShowButton(options[0]);
            return;
        }

        // For multiple options
        for(int i = 0; i < options.Length; i++)
        {
            if(i>= PooledOptions.Count)
            {
                CreateButton();
            }
            PooledOptions[i].ShowButton(options[i]);
        } 
    }

    protected virtual void DisableButtons()
    {
        NextDialogOption.HideButton();
        for(int i = 0; i < PooledOptions.Count; i++)
        {
            PooledOptions[i].HideButton();
        }
    }

    protected virtual IEnumerator ShakeEvent(float intensity, float duration)
    {
        float elapsed = 0;
        while(elapsed < duration || duration < 0)
        {
            intensity = Mathf.Clamp01(intensity);
            Vector2 offset = new Vector2(UnityEngine.Random.Range(-1.0f,1.0f), UnityEngine.Random.Range(-1.0f,1.0f)) * intensity;
            offset.Scale(ShakeOffset);
            Vector3 finalPos = DialogLabel.transform.position + (Vector3)offset;
            finalPos.x = Mathf.Clamp(finalPos.x, MaxShakeBounds.min.x, MaxShakeBounds.max.x);
            finalPos.y = Mathf.Clamp(finalPos.y, MaxShakeBounds.min.y, MaxShakeBounds.max.y);
            
            DialogLabel.transform.position = finalPos;
            float waitSecs = Mathf.Lerp(ShakeDelay.x, ShakeDelay.y, intensity);
            yield return new WaitForSeconds(waitSecs);
            elapsed += waitSecs;
        }
        ResetDialog();
    }

    protected virtual void ResetDialog()
    {
        DialogLabel.transform.position = OriginalDialogLabelPosition;
    }

    protected virtual void StopActions()
    {
        if(ShakeRoutine != null)
        {
            StopCoroutine(ShakeRoutine);
        }
    }
}

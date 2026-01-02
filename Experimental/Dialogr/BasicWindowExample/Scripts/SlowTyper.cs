using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
 
[RequireComponent(typeof(TMPro.TextMeshProUGUI))]
public class SlowTyper : MonoBehaviour
{
    TMPro.TextMeshProUGUI DialogLabel;
    int numShowing;
 
    [SerializeField]
    float delay = 0.018f; // bigger number is slower typing
    [SerializeField]
    private float PunctuationDelay = 0.3f;
 
    string TextToFill;
    string SizeText ="";
    Action EndCallback;    bool init = false;
    bool openTag = false;
    int numLinesNeeded = 0;
    bool pauses;
 
    [SerializeField]
    float SoundDelay = 0.1f;
    
    private float curSoundDelay = 0;
 
    public string GetDisplayText()
    {
        return TextToFill;
    }
 
    private void Awake()
    {
        DialogLabel = GetComponent<TMPro.TextMeshProUGUI>();
    }
 
    public void Begin(bool pauses = true, string textToFill = null, Action endCallback = null)
    {
        numShowing = 0;
        this.TextToFill = textToFill.Trim();
        this.EndCallback = endCallback;
        this.pauses = pauses;
 
        DialogLabel.enableAutoSizing = true;
        DialogLabel.text = textToFill;
        numLinesNeeded = CountNumLines();
        float desiredSize = DialogLabel.fontSize;
        DialogLabel.enableAutoSizing = false;
        DialogLabel.fontSize = desiredSize;
 
        DialogLabel.text = "";
        DialogLabel.ForceMeshUpdate();
 
        if (numLinesNeeded == -1)
            SizeText = "";
        else if (numLinesNeeded <= 1)
            SizeText = "<size=130%><alpha=#00>x<alpha=#FF></size>" + "\n";
        else if (numLinesNeeded == 2)
        {
            SizeText = "<size=80%><alpha=#00>x<alpha=#FF></size>" + "\n";
            numLinesNeeded = 0;
        }
        else if (numLinesNeeded >= 3)
        {
            SizeText= "<size=30%><alpha=#00>x<alpha=#FF></size>" + "\n";
            numLinesNeeded = 0;
        }

        DialogLabel.text += SizeText;
 
        InvokeRepeating("ShowOneMore", 0, delay);
    }
 
    private int CountNumLines()
    {
        DialogLabel.ForceMeshUpdate();
        int lineCount = 1;
        if (null != DialogLabel.textInfo)
        {
            lineCount = DialogLabel.textInfo.lineCount;
        }
 
        // This is buggy for low numbers, manually override
        if (lineCount <= 1)
        {
            lineCount = (int)(DialogLabel.text.Length / 55.0f);
            //Debug.Log( "Overode to : " + lineCount );
        }
 
        return lineCount;
    }
 
    public void Clear()
    {
        CancelInvoke();
        TextToFill = "";
        DialogLabel.text = "";
    }
 
    internal bool IsDone()
    {
        if (string.IsNullOrEmpty(TextToFill))
            Clear();
        return numShowing >= TextToFill.Length;
    }
 
    private void Update()
    {
        curSoundDelay -= Time.deltaTime;
    }

    public void ForceEnd()
    {
        SpeedUp();
    }
 
    private void ShowOneMore()
    {
        if (IsDone())
        {
            CancelInvoke();
            EndCallback?.Invoke();
            EndCallback = null;
        }
        else
        {
            //// Remove line endings we added
            //if ( numLinesNeeded > 1 )
            //    text.text = text.text.Substring( 0, numShowing );
 
            char nextChar = TextToFill[numShowing];
            DialogLabel.text += nextChar;
            numShowing++;
 
            if (curSoundDelay < 0)
            {
                curSoundDelay = SoundDelay;
            }
 
            SkipThroughTags(nextChar);
 
            //// Add line endings
            //if ( numLinesNeeded > 1 ) {
            //    int lineEndingsPresent = CountNumLines();
            //    int numToAdd = numLinesNeeded - lineEndingsPresent;
            //    for ( int i = 0; i < numToAdd; i++ )
            //        text.text += '\n' + "<alpha=#00>x";
            //    // Add actual content so TMP respects me
            //    //text.text += ;
            //}
 
            if (pauses && (nextChar == '.' || nextChar == '!' || nextChar == '?' || nextChar == '>'))
            {
                CancelInvoke();
                if (gameObject.activeInHierarchy)
                    StartCoroutine(WaitAfterPunctuation());
            }
        }
    }
 
    IEnumerator WaitAfterPunctuation()
    {
        yield return new WaitForSeconds(PunctuationDelay);
        InvokeRepeating("ShowOneMore", 0, delay);
    }
 
    private void SkipThroughTags(char lastCharTyped)
    {
        if (lastCharTyped == '<')
            openTag = true;
 
        if (lastCharTyped == '>')
            openTag = false;
 
        if (openTag)
            ShowOneMore();
    }
 
    internal void SpeedUp()
    {
        StopAllCoroutines();
        CancelInvoke();
 
        while (!IsDone())
        {
            char nextChar = TextToFill[numShowing];
            DialogLabel.text += nextChar;
            numShowing++;
        }
 
        EndCallback?.Invoke();
        EndCallback = null;
    }
}
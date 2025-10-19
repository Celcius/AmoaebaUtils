using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AmoaebaUtils
{
    
public class AnimateFrames : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer spriteRenderer;
    [SerializeField]
    private Image image;
    
    [SerializeField]
    private Sprite[] frames;

    [SerializeField]
    private float frameTime = 0.1f;
    private float elapsed = 0;
    int index = -1;
    [SerializeField]
    private bool randomizeStart = false;

    [SerializeField]
    private bool loop = true;

    [SerializeField]
    private bool playOnEnable = false;

    private IEnumerator playRoutine = null;
    public bool IsPlaying => (playRoutine != null);

    public Sprite Sprite
    {
        get 
        { 
            return spriteRenderer == null? 
                    (image == null?
                    null :
                    image.sprite) :
                    spriteRenderer.sprite; 
        }
        
        set 
        { 
            if(spriteRenderer != null)
            {
                spriteRenderer.sprite = value; 
            }
            if(image != null)
            {
                image.sprite = value; 
            }
        }
    }

    private void Start()
    {
        StartPlaying();
    }

    public void StartPlaying()
    {
        StartPlaying(frames, frameTime, randomizeStart, loop);
    }
    public void StartPlaying(Sprite[] newFrames)
    {
        StartPlaying(newFrames, frameTime, randomizeStart, loop);
    }

    public void StartPlaying(Sprite[] newFrames, float newTime)
    {
        StartPlaying(newFrames, newTime, randomizeStart, loop);
    }

    public void StartPlaying(Sprite[] newFrames, float newTime, bool isRandomStart, bool isLooping)
    {
        StopPlaying();

        frames = newFrames;
        frameTime = newTime;
        randomizeStart = isRandomStart;
        loop = isLooping;

        if(randomizeStart && !(frames == null || 
                               (spriteRenderer == null && image == null) || 
                               frames.Length == 0))
        {
            index = UnityEngine.Random.Range(0, frames.Length-1);
        }
        else
        {
            index = 0;
        }

        playRoutine = PlayRoutine();
        StartCoroutine(playRoutine);
    }

    public void StopPlaying()
    {
        if(playRoutine != null)
        {
            StopCoroutine(playRoutine);
        }
        elapsed = 0.0f;
        playRoutine = null;
        
        if(spriteRenderer != null)
        {
            spriteRenderer.sprite = frames.Length > 0 ? frames[0] : spriteRenderer.sprite;
        }

        if(image != null)
        {
            image.sprite = frames.Length > 0 ? frames[0] : image.sprite;
        }   
    }

    // Update is called once per frame
    private IEnumerator PlayRoutine()
    {
        if(frames == null || (spriteRenderer == null && image == null) || frames.Length == 0)
        {
            yield break;
        }

        while (index < frames.Length)
        {
            elapsed -= Time.deltaTime;
            if(elapsed <= 0)
            {
                this.Sprite = frames[index];
                elapsed = frameTime;
                index = loop? (index +1) % frames.Length : (index +1);
            }
            
            if(frames.Length == 1)
            {
                if(!loop)
                {
                    FinishedAnimating();
                }
                yield break;
            }
            yield return new WaitForEndOfFrame();
        }

        FinishedAnimating();
        playRoutine = null;
    }

    protected virtual void FinishedAnimating() {}

    private void OnEnable()
    {
        if(playOnEnable)
        {
            StartPlaying();
        }
    }


}

#if UNITY_EDITOR

[CustomEditor(typeof(AnimateFrames))]
public class AnimateFramesEditor : Editor
{/*
    private AnimateFrames framesAnimator;

    private void OnEnable()
    {
        framesAnimator = (AnimateFrames)target;
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if(framesAnimator == null)
        {
            return;
        }

        EditorGUILayout.Space();
                
        if(GUILayout.Button(framesAnimator.IsPlaying? "Stop Animation" : "Test Animation"))
        {
            if(framesAnimator.IsPlaying)
            {
                framesAnimator.StopPlaying();    
            }
            else
            {
                framesAnimator.StartPlaying();
            }
        }
    }*/
}
#endif
}

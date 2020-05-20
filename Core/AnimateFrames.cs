using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateFrames : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer spriteRenderer;

    
    [SerializeField]
    private Sprite[] frames;

    [SerializeField]
    private float frameTime = 0.1f;
    private float elapsed = 0;
    int index = -1;
    [SerializeField]
    private bool randomizeStart = false;

    
    void Start()
    {
        if(randomizeStart && !(frames == null || spriteRenderer == null || frames.Length == 0))
        {
            index = Random.Range(0, frames.Length-1);
        }
    }
    // Update is called once per frame
    void Update()
    {
        if(frames == null || spriteRenderer == null || frames.Length == 0)
        {
            return;
        }
        
        elapsed -= Time.deltaTime;
        if(elapsed <= 0)
        {
            index = (index +1) % frames.Length;
            spriteRenderer.sprite = frames[index];
            elapsed = frameTime;
        }
    }
}

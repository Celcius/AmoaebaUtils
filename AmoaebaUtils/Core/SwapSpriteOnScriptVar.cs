using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwapSpriteOnScriptVar<V, T> : MonoBehaviour where V : ScriptVar<T>
{
    [System.Serializable]
    public struct SwapSpriteEntry
    {
        public T key;
        public Sprite value;
    }

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    [SerializeField]
    private Image image;
    
    [SerializeField]
    private V scriptVar;

    [SerializeField]
    private SwapSpriteEntry[] entries;

    private Dictionary<T, Sprite> entriesDict = new Dictionary<T, Sprite>();
    private void Start()
    {
        scriptVar.OnChange += OnVarChange;
        foreach(SwapSpriteEntry entry in entries)
        {
            entriesDict[entry.key] = entry.value;
        }
        OnVarChange(default(T), scriptVar.Value);
    }

    private void OnVarChange(T oldVal, T newVal)
    {
        Sprite newSprite = null;
        if(entriesDict.ContainsKey(newVal))
        {
            newSprite = entriesDict[newVal];
        }
        
        if(spriteRenderer != null)
        {
            spriteRenderer.sprite = newSprite;
        }

        if(image != null)
        {
            image.sprite = newSprite;
        }
    }

    private void OnDestroy() 
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

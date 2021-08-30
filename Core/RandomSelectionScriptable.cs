using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSelectionScriptable<T> : ScriptableObject
{
    [System.Serializable]
    public class WeightStruct
    {
        public T entity;
        public int weight = 1;
    }

    [SerializeField]
    private WeightStruct[] objectsToSelect;

    private int computedWeight = 0;

    private float[] computedPercents;

    private void OnEnable() 
    {
        if(objectsToSelect == null || objectsToSelect.Length == 0)
        {
            return;
        }

        computedWeight = 0;
        computedPercents = new float[objectsToSelect.Length];
        foreach(WeightStruct weight in objectsToSelect)
        {
            computedWeight += weight.weight;
        }        

        int curWeight = 0;
        for(int i = 0; i < objectsToSelect.Length; i++)
        {
            curWeight += objectsToSelect[i].weight;
            computedPercents[i] = Mathf.Clamp01((float)curWeight / (float)computedWeight);
        }


    }

    public T GetRandomSelection()
    {
        if(objectsToSelect == null || objectsToSelect.Length == 0)
        {
            return default(T);
        }

        float roll = Random.Range(0.0f, 1.0f);
        for(int i = 0; i < computedPercents.Length; i++)
        {
            if(roll <= computedPercents[i])
            {
                return objectsToSelect[i].entity;
            }
        }
        Debug.LogError("Unexpectedly returning unweighted Selection");
        return objectsToSelect[Random.Range(0, objectsToSelect.Length)].entity;
    }

    
    
}

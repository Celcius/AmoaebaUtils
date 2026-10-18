using System;
using System.Collections.Generic;
using UnityEngine;

public class MaterialRandomizer : MonoBehaviour
{
    [System.Serializable]
    private struct WeightedMaterialStruct
    {
        public Material ChosenMaterial;
        public float Weight;
    }

    [SerializeField]
    private WeightedMaterialStruct[] _materialsToChoose;

    [SerializeField]
    private MeshRenderer _renderer;

    void Start()
    {
        
        float weight = 0;
        foreach(WeightedMaterialStruct wMat in _materialsToChoose)
        {
            weight += wMat.Weight;
        }

        float roll = UnityEngine.Random.Range(0, weight);

        Debug.Log(roll);

        weight = 0;
        foreach(WeightedMaterialStruct wMat in _materialsToChoose)
        {
            weight += wMat.Weight;
            if(roll <= weight)
            {
                _renderer.material = wMat.ChosenMaterial;
                return;
            }
        }

        _renderer.material = null;
    }
}

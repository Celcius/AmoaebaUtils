using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AmoaebaUtils;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ParallaxController : MonoBehaviour
{
    [Header("Parallax")]

    [SerializeField]
    private Vector2 parallaxRatio = Vector2.one;

    [SerializeField]
    private bool instantiateOnAwake = true;

    [SerializeField]
    private BoolVector2 wrap = new BoolVector2(true, true);
    
    [Space]

    [Header("Instances")]

    [SerializeField]
    private Parallaxable parallaxPrefab;

    [SerializeField]
    public Vector2Int copyLayout = new Vector2Int(1,1);

    [SerializeField]
    private BoolVector2 alternate = new BoolVector2(true, true);

    [SerializeField]
    private BoolVector2 center = new BoolVector2(true, true);

    [SerializeField]
    private Vector2 offset = Vector2.zero;
    
    [SerializeField]
    private Vector2 padding = Vector2.zero;

    private List<Parallaxable> instantiatedObjects = new List<Parallaxable>();

    public bool NeedsToInstantiate => (instantiatedObjects.Count != CopyAmount);
    public int CopyAmount => copyLayout.x*copyLayout.y;
    private Bounds parallaxBounds;

    [SerializeField]
    private Vector2 parallaxScroll;
    public Vector2 ParallaxScroll
    {
        get { return parallaxScroll; }
        set 
        { 
            parallaxScroll = value;
            UpdatePositions();
        }
    }

    public Bounds InstanceBounds => parallaxPrefab.GetBounds();

    private void Awake() 
    {
        parallaxScroll = Vector2.zero;
        if(instantiateOnAwake)
        {
            InstantiateObjects();
        }
    }

    public void InstantiateObjects()
    {
        CleanInstances();
        if(parallaxPrefab == null)
        {
            return;
        }

        Vector2 min = new Vector2(float.MaxValue, float.MaxValue);
        Vector2 max = new Vector2(float.MinValue, float.MinValue);
        Vector3 instanceSize = Vector2.zero;
        for(int i = 0; i < CopyAmount; i++)
        {
            Vector3 position = GetPositionFromMatrix(GetModulatedIndex(i));
            Parallaxable instance = Instantiate<Parallaxable>(parallaxPrefab, position,
                                                              Quaternion.identity,
                                                              transform);
            instantiatedObjects.Add(instance); 

            instance.gameObject.SetActive(true);                                                             
            
            Bounds instanceBounds = instance.GetBounds();
            min = Vector2.Min(min, instanceBounds.min);
            max = Vector2.Max(max, instanceBounds.max);
            instanceSize = instanceBounds.size;
        }

        parallaxBounds = new Bounds((Vector3)offset, 
                                    (Vector3)(max-min) - instanceSize/2.0f);
        UpdatePositions();
    }

    public void UpdatePositions()
    {
        for(int i = 0; i < instantiatedObjects.Count; i++)
        {
            Parallaxable paralaxable = instantiatedObjects[i];
            Vector2 modulatedIndex =  GetModulatedIndex(i);
            Vector3 position = GetPositionFromMatrix(modulatedIndex) + (Vector3)(Vector2.Scale(parallaxRatio, parallaxScroll));
                    
            if(wrap.x || wrap.y)
            {
                Vector2 paralaxableSize = paralaxable.GetBounds().size;
                Vector2 size = (Vector2)parallaxBounds.size + paralaxableSize/2.0f;
                
                position = new Vector3(wrap.x? position.x-parallaxBounds.min.x : position.x,
                                       wrap.y? position.y-parallaxBounds.min.y : position.y,
                                       position.z);
                

                if(wrap.x)
                {
                    position.x = position.x < 0? size.x + position.x % size.x  : position.x;
                    position.x = (position.x) %  size.x + parallaxBounds.min.x;
                    position.x += transform.position.x;
                }

                if(wrap.y)
                {
                    position.y = position.y < 0? size.y + position.y % size.y  : position.y;
                    position.y = (position.y) %  size.y + parallaxBounds.min.y;
                    position.y += transform.position.y;

                    Func<float, float> maxRet = arg => arg + (modulatedIndex.y-1) * paralaxableSize.y;
                    Func<float, float> minRet = arg => copyLayout.y * paralaxableSize.y + maxRet(arg);
                }
            }
            else
            {
                position += transform.position;
            }
            paralaxable.transform.position = position;
        }
    }

    private Vector2 GetModulatedIndex(int i)
    {
        return StructUtils.Modulate2DMatrixIndex(i, copyLayout, center, alternate);
    
    }
    private Vector3 GetPositionFromMatrix(Vector2 matrixPos)
    {
        Vector3 position = (Vector3)offset + (Vector3) Vector2.Scale(padding,matrixPos) +
            new Vector3(parallaxPrefab.GetBounds().extents.x*2.0f * matrixPos.x,
                        parallaxPrefab.GetBounds().extents.y*2.0f * matrixPos.y,
                        0);

        return position;
    }
    
    private void OnDestroy()
    {
        CleanInstances();
    }

    private void CleanInstances() 
    {
        foreach(Parallaxable p in instantiatedObjects)
        {
            if(p == null)
            {
                continue;
            }
#if UNITY_EDITOR
            DestroyImmediate(p.gameObject);
#else                        
            Destroy(p.gameObject);
#endif            
        }
        instantiatedObjects.Clear();
    }

       void OnDrawGizmos()
    {
        Color prevColor = Gizmos.color;
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position+parallaxBounds.min, transform.position+parallaxBounds.max);
        Gizmos.color = prevColor;
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(ParallaxController))]
public class ParallaxControllerEditor : Editor
{
    private ParallaxController controller;
    private void OnAwake()
    {
        controller = (ParallaxController)target;
    }

    private void OnEnable()
    {
        controller = (ParallaxController)target;
    }

    public override void OnInspectorGUI()
    {
        if(Application.isPlaying || EditorApplication.isPlayingOrWillChangePlaymode)
        {
            base.OnInspectorGUI();
            return;
        }

        base.OnInspectorGUI();
        
        if (GUILayout.Button("Preview On Editor"))
        {
            controller.InstantiateObjects();
        }
    }
}
#endif


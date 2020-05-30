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
    private bool wrap = true;

    [Space]

    [Header("Instances")]

    [SerializeField]
    private Parallaxable parallaxPrefab;

    [SerializeField]
    private int copyAmount = 2;

    [SerializeField]
    private bool alternate = true;

    [SerializeField]
    private bool center = true;

    [SerializeField]
    private Vector2 offset = Vector2.zero;
    
    [SerializeField]
    private Vector2 padding = Vector2.zero;

    private List<Parallaxable> instantiatedObjects = new List<Parallaxable>();

    public bool NeedsToInstantiate => (instantiatedObjects.Count != copyAmount);
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

    private void Awake() 
    {
        parallaxScroll = Vector2.zero;
        InstantiateObjects();   
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
        for(int i = 0; i < copyAmount; i++)
        {
            Vector3 position = GetModulatedPosition(i);
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
            Vector3 position = GetModulatedPosition(i) + (Vector3)(Vector2.Scale(parallaxRatio, parallaxScroll));
                    
            if(wrap)
            {
                Vector2 size = (parallaxBounds.size + paralaxable.GetBounds().size/2.0f);
                position = position-parallaxBounds.min;

                position.x = position.x < 0? size.x + position.x % size.x  : position.x;
                position.y = position.y < 0? size.y + position.y % size.y  : position.y;
                
                position.x = (position.x) %  size.x + parallaxBounds.min.x;
                position.y = (position.y) %  size.y + parallaxBounds.min.y;
            }
            paralaxable.transform.position = transform.position+position;
        }
    }

    private Vector3 GetModulatedPosition(int i)
    {
        float index = StructUtils.ModulateIndex(i, copyAmount, center, alternate);
        Vector3 position = (Vector3)offset + (Vector3)padding * index +
            new Vector3(parallaxPrefab.GetBounds().extents.x*2.0f * index,
            0,
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

        EditorGUI.BeginChangeCheck();
        base.OnInspectorGUI();
        
        if (EditorGUI.EndChangeCheck() && controller != null)
        {
            controller.InstantiateObjects();
        }
    }
}
#endif


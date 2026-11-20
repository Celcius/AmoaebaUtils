using AmoaebaUtils;
using UnityEngine;

namespace AmoaebaUtils
{

public class ShadowProjector : MonoBehaviour
{
    [SerializeField] 
    protected Transform ShadowPrefab;

    [SerializeField] 
    LayerMask ReceiveMask;
    [SerializeField] 
    Vector3 ProjectDirection = Vector3.down;

    [SerializeField] 
    float MaxDistance = 10f;
    
    [SerializeField]
    private AnimationCurve OpacityDecay;

    [SerializeField]
    private AnimationCurve SizeDecay;

    [SerializeField] 
    Vector3 Offset;

    [SerializeField] 
    protected Vector3 ShadowScale;

    protected Transform ShadowParent;
    protected SpriteRenderer ShadowRenderer;

    protected virtual void Awake()
    {
        ShadowParent = Instantiate(ShadowPrefab);
        ShadowRenderer = ShadowParent.GetComponentInChildren<SpriteRenderer>();
    }

    void OnEnable()  
    { 
        if (ShadowParent != null) 
        {
            ShadowParent.gameObject.SetActive(true); 
        }
    }

    void OnDisable() 
    { 
        if (ShadowParent != null) 
        {
            ShadowParent.gameObject.SetActive(false);    
        }
    }

    void OnDestroy()
    {
         if (ShadowParent != null) 
         {
            Destroy(ShadowParent.gameObject); 
         }
    }

    void LateUpdate()
    {
        if (Physics.Raycast(transform.position, ProjectDirection, out var hit, MaxDistance, ReceiveMask))
        {
            ShadowParent.gameObject.SetActive(true);
            ShadowParent.transform.SetPositionAndRotation(hit.point + Quaternion.FromToRotation(Vector3.up, hit.normal) * Offset,
                                                          Quaternion.FromToRotation(Vector3.back, hit.normal));

            float delta = 1f - hit.distance / MaxDistance;
            ShadowRenderer.transform.localScale = ShadowScale * SizeDecay.Evaluate(delta);

            var color = ShadowRenderer.color;
            color.a = OpacityDecay.Evaluate(delta);
            ShadowRenderer.color = color;
        }
        else
        {
            ShadowParent.gameObject.SetActive(false);
        }
    }
}
}
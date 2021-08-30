using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AmoaebaUtils
{

[RequireComponent(typeof(ParallaxController))]
public class ParallaxFromWindowBounds : MonoBehaviour
{
    [SerializeField]
    private BoolVector2 axis;

    [SerializeField]
    private Vector2Var windowBounds;

    private ParallaxController controller;

    // Start is called before the first frame update
    private void Awake()
    {
        controller = GetComponent<ParallaxController>();
         if(windowBounds.Value.x != 0 && windowBounds.Value.y != 0)
         {
             OnBoundsChange(Vector2.zero, windowBounds.Value);
         }

         windowBounds.OnChange += OnBoundsChange;
    }

    private void OnDestroy()
    {
        windowBounds.OnChange -= OnBoundsChange;
    }

    private void OnBoundsChange(Vector2 oldVal, Vector2 newVal)
    {
        Bounds instanceBounds = controller.InstanceBounds;
        Vector2 ratios = new Vector2(newVal.x / instanceBounds.size.x+1, 
                                     newVal.y / instanceBounds.size.y+1); 

        int x = axis.x? (int)Mathf.Ceil(ratios.x) : controller.copyLayout.x;
        int y = axis.y? (int)Mathf.Ceil(ratios.y) : controller.copyLayout.y;

        controller.copyLayout = new Vector2Int(x,y);
        controller.InstantiateObjects();
    }
}
}
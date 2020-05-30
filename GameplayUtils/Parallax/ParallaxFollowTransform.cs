using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AmoaebaUtils;

[RequireComponent(typeof(ParallaxController))]
public class ParallaxFollowTransform : FollowTransform
{
    [SerializeField]
    private bool ignoreJumps = false;

    [SerializeField]
    private float jumpThreshold = 0;
    Vector2 prevOffset = Vector2.zero;

    ParallaxController controller;
    private void Awake()
    {
        controller = GetComponent<ParallaxController>();
    }

	protected override void MoveTo(Vector3 pos)
	{
        Vector2 offset = (Vector2)(pos - transform.position);
		base.MoveTo(pos);
        
        if(ignoreJumps && offset.magnitude >= jumpThreshold)
        {
            controller.ParallaxScroll -= prevOffset;
        }
        else
        {
            controller.ParallaxScroll -= offset;
            prevOffset = offset;
        }   
	}
}

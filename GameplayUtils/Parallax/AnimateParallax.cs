using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AmoaebaUtils;

[RequireComponent(typeof(ParallaxController))]
public class AnimateParallax : MonoBehaviour
{
    [SerializeField]
    private PhysicsAnimationCurve xVelocityCurve = new PhysicsAnimationCurve();
    
    [SerializeField]
    private PhysicsAnimationCurve yVelocityCurve = new PhysicsAnimationCurve();

    private float maxTime = 0;
    private float elapsedTime = 0;
    
    private ParallaxController controller;

    private void Awake()
    {
        elapsedTime = 0;
        controller = GetComponent<ParallaxController>();
        maxTime = Mathf.Max(xVelocityCurve.Duration, yVelocityCurve.Duration);
    }

    private void Update()
    {
        elapsedTime = (elapsedTime + Time.deltaTime) % maxTime;
        
        Vector2 velVec = new Vector2(xVelocityCurve.Evaluate(0, controller.ParallaxScroll.x, elapsedTime),
                                     yVelocityCurve.Evaluate(0, controller.ParallaxScroll.y, elapsedTime));
        controller.ParallaxScroll = velVec;
    }
}

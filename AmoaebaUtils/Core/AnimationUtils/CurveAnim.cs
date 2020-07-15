using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AmoaebaUtils
{
public abstract class CurveAnim : MonoBehaviour
{
    [SerializeField]
    private PhysicsAnimationCurve animationSpeed;

    [SerializeField]
    private bool randomStart = true;

    private float elapsedTime = 0;
    private float lastInstant = 0;

    protected virtual void Start()
    {
        lastInstant = animationSpeed.Duration;
        if(randomStart && animationSpeed.Duration > 0)
        {
            elapsedTime = Random.Range(0, animationSpeed.Duration);
        }
    }

   protected virtual void Update() 
   {
        if(lastInstant == 0)
        {
            return;
        }

        elapsedTime = (elapsedTime + Time.deltaTime) % lastInstant;

        OnChange(animationSpeed.Evaluate(elapsedTime));
   }

   protected abstract void OnChange(float evaluatedVal);

}
}
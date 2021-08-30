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

        elapsedTime = elapsedTime + GetDeltaTime();

        OnChange(animationSpeed.Evaluate(0, elapsedTime, GetDeltaTime()));
   }

   protected abstract void OnChange(float evaluatedVal);

   public void SetElapsedTime(float elapsed)
   {
       elapsedTime = Mathf.Clamp(elapsed, 0, animationSpeed.Duration);
   }

   public virtual float GetDeltaTime()
   {
       return Time.deltaTime;
   }
}
}
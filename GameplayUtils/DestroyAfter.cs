using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AmoaebaUtils
{
public class DestroyAfter : MonoBehaviour
{
    [SerializeField]
    protected float timeToDestroy;

    private float elapsed = 0;

    void Awake()
    {
        if(timeToDestroy == 0)
        {
            DestroySelf();
        }
    }

        void Update()
    {
        elapsed += GetDeltaTime();
        if(timeToDestroy <= elapsed)
        {
            DestroySelf();
        }
        OnElapsed(elapsed);
    }

    protected virtual void DestroySelf()
    {
        Destroy(this.gameObject);
    }

    protected virtual float GetDeltaTime()
    {
        return Time.deltaTime;
    }

    protected virtual void OnElapsed(float elapsed)
    {

    }
}
}
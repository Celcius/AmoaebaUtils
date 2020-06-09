using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AmoaebaUtils
{
public class InstantiateTimer : MonoBehaviour
{
    [SerializeField]
    private Transform prefab;

    [SerializeField]
    private float timeToStart = 0;

    [SerializeField]
    private float randomTimeToStart = 0;

    [SerializeField]
    private float timeBetween = 0.1f;
    
    [SerializeField]
    private float randomTimeBetween = 0;

    [SerializeField]
    private Vector3 offset;

    [SerializeField]
    private BooledInt amount;

    private IEnumerator instantiateRoutine;
    private int instantiateCount = 0;

    public bool IsRunning => instantiateRoutine != null;

    protected virtual void Start()
    {
        StopInstantiation();
        StartInstantiation();
    }

    public virtual void StartInstantiation()
    {
        instantiateCount = 0;
        instantiateRoutine = InstantiateRoutine();
        StartCoroutine(instantiateRoutine);
    }

    private IEnumerator InstantiateRoutine()
    {
        if(timeToStart > 0)
        {
            yield return new WaitForSeconds(timeToStart + Random.Range(0,randomTimeToStart));
        }

        while(!amount.check || instantiateCount < amount.value)
        {
            Instantiate(prefab, transform.position + offset, Quaternion.identity);
            instantiateCount++;
            yield return new WaitForSeconds(timeBetween + Random.Range(0, randomTimeBetween));
        }
    }

    public virtual void StopInstantiation()
    {
        if(instantiateRoutine != null)
        {
            StopCoroutine(instantiateRoutine);
        }
        instantiateRoutine = null;
    }
}
    
}
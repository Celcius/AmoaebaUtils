using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AmoaebaUtils
{
public class RemoveChildrenOnStart : MonoBehaviour
{
    [SerializeField]
    private bool destroySelf = true;
    private void Start()
    {
        Transform[] children = GetComponentsInChildren<Transform>();
        foreach (Transform item in children)
        {
            item.parent = null;
        }

        if(destroySelf)
        {
            Destroy(this.gameObject);
        }
    }
}
}
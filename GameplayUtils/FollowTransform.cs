using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;

namespace AmoaebaUtils
{
public class FollowTransform : MonoBehaviour {

    [SerializeField]
    private TransformVar transformVariable;

	[SerializeField]
	private bool lockX = false;

	[SerializeField]
	private bool lockY = false;

	[SerializeField]
	private bool lockZ = false;

	private void Awake()
	{
		Assert.IsNotNull(transformVariable, "Variable not assigned to " + this.name);
	}

	private void Update () {
	    if(transformVariable == null || transformVariable.Value == null)
        {
			return;
		}

        transform.position = new Vector3(lockX? transform.position.x : transformVariable.Value.position.x,
										lockY? transform.position.y : transformVariable.Value.position.y,
										lockZ? transform.position.z : transformVariable.Value.position.z);
	}
}
}
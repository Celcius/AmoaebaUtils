using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;

namespace AmoaebaUtils
{
public class FollowTransform : MonoBehaviour {

    [SerializeField]
    private TransformVar transformVariable;

	[SerializeField]
	private BoolVector3  lockPos;

	[SerializeField]
	private BooledVector3  ClampMin = new BooledVector3(new BoolVector3(false, false, false),
                                                    	new Vector3(0,0,0));
	[SerializeField]
	private BooledVector3 ClampMax = new BooledVector3(new BoolVector3(false, false, false),
                                                    	new Vector3(0,0,0));

	private void Awake()
	{
		Assert.IsNotNull(transformVariable, "Variable not assigned to " + this.name);
	}

	private void Update () {
	    
		if(transformVariable == null || transformVariable.Value == null)
        {
			return;
		}

        UpdatePos();
	}

	protected virtual void UpdatePos()
	{
		Vector3 newPos = new Vector3(lockPos.x? transform.position.x : transformVariable.Value.position.x,
								     lockPos.y? transform.position.y : transformVariable.Value.position.y,
									 lockPos.z? transform.position.z : transformVariable.Value.position.z);
		
		newPos = Vector3.Max(ClampMin.EvaluateVec(newPos), newPos);
		newPos = Vector3.Min(ClampMax.EvaluateVec(newPos), newPos);

		MoveTo(newPos);
	}
	
	protected virtual void MoveTo(Vector3 pos)
	{
		transform.position = pos;
	}
}
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace  AmoaebaUtils
{
    public class CurveTransformMove : TransformMovement
    {
      

        [SerializeField]
        private PhysicsAnimationCurve XCurve = new PhysicsAnimationCurve();
        
        [SerializeField]
        private PhysicsAnimationCurve YCurve = new PhysicsAnimationCurve();

        [SerializeField]
        private PhysicsAnimationCurve ZCurve = new PhysicsAnimationCurve();
        
        private float elapsedTime = 0;
        private float maxTime;

        private void Awake()
        {
            maxTime = Mathf.Max(XCurve.Duration,
                                YCurve.Duration,
                                ZCurve.Duration);
                                
        }
        
        protected override void Move()
        {
            elapsedTime = (elapsedTime + Time.deltaTime) % maxTime;
            transform.position = new Vector3(XCurve.Evaluate(transform.position.x,elapsedTime),
                                             YCurve.Evaluate(transform.position.y,elapsedTime),
                                             ZCurve.Evaluate(transform.position.z,elapsedTime));
        }

        public override void SetElapsedTime(float elapsed) 
        {
            elapsedTime = elapsed % maxTime;
        }
    }
}

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

        [SerializeField]
        private BoolVector3 inheritRotations = new BoolVector3(false, false, false);
        
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
            float deltaTime = GetDeltaTime();
            float actualTime = Time.deltaTime;
            
            elapsedTime = (elapsedTime + GetDeltaTime()) % maxTime;
            Vector3 position = new Vector3(XCurve.Evaluate(transform.position.x,elapsedTime, GetDeltaTime()),
                                           YCurve.Evaluate(transform.position.y,elapsedTime, GetDeltaTime()),
                                           ZCurve.Evaluate(transform.position.z,elapsedTime, GetDeltaTime()));
                                           
            Quaternion rotation = Quaternion.Euler(inheritRotations.x? transform.localRotation.eulerAngles.x : 0,
                                                   inheritRotations.y? transform.localRotation.eulerAngles.y : 0,
                                                   inheritRotations.z? transform.localRotation.eulerAngles.z : 0);

            transform.position = transform.position + rotation * (position - transform.position);
        }

        public override void SetElapsedTime(float elapsed) 
        {
            elapsedTime = elapsed % maxTime;
        }
    }
}

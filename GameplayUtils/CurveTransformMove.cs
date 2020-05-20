using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace  AmoaebaUtils
{
    public class CurveTransformMove : TransformMovement
    {
        [Serializable]
        protected struct CurveMoveDefinition
        {
            public enum CurveMoveType
            {
                None,
                Position,
                PositionOffset,
                Velocity,
            }

            public CurveMoveType Type;
            public AnimationCurve Curve;
            public float Mult;
            public float Offset;
            public float TimeMult;
            public float TimeOffset;

            public float Duration
            {
                get {
                    return Curve.keys.Length == 0? 0.0f : Curve.keys[Curve.keys.Length-1].time;
                }
            }
        }

        [SerializeField]
        private CurveMoveDefinition XCurve = EmptyCurveDefinition();
        
        [SerializeField]
        private CurveMoveDefinition YCurve = EmptyCurveDefinition();

        [SerializeField]
        private CurveMoveDefinition ZCurve = EmptyCurveDefinition();
        
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
            transform.position = new Vector3(EvaluateCurve(XCurve, transform.position.x, elapsedTime),
                                             EvaluateCurve(YCurve, transform.position.y, elapsedTime),
                                             EvaluateCurve(ZCurve, transform.position.z, elapsedTime));
        }

        private float EvaluateCurve(CurveMoveDefinition definition, float prevValue, float elapsedTime)
        {
            if(definition.Type == CurveMoveDefinition.CurveMoveType.None)
            {
                return prevValue;
            }

            elapsedTime = (elapsedTime +definition.TimeOffset)% definition.TimeMult;
            float evaluatedValue = definition.Curve.Evaluate(elapsedTime) * definition.Mult + definition.Offset;

            switch (definition.Type)
            {
                case  CurveMoveDefinition.CurveMoveType.None:
                    return prevValue;
                case  CurveMoveDefinition.CurveMoveType.Position:
                    return evaluatedValue;
                case  CurveMoveDefinition.CurveMoveType.PositionOffset:
                    return prevValue + evaluatedValue;
                case  CurveMoveDefinition.CurveMoveType.Velocity:
                    return prevValue + evaluatedValue * Time.deltaTime;
            }

            Debug.LogError("Fell through unexpected switch case at " + this.name);
            return prevValue;
        }

        private static CurveMoveDefinition EmptyCurveDefinition()
        {
            CurveMoveDefinition definition;
            definition.Type = CurveMoveDefinition.CurveMoveType.None;
            definition.Curve = AnimationCurve.Constant(0,1,0);
            definition.Mult = 1.0f;
            definition.Offset = 0.0f;
            definition.TimeMult = 1.0f;
            definition.TimeOffset = 0.0f;
            return definition;
        }
    }
}

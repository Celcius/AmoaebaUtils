using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;
using UnityEngine;
using System;

namespace AmoaebaUtils
{
public class SceneTransitionDefinition : ScriptableObject
{
        [SerializeField]
        private string prevScene;
        public string PrevScene => prevScene;

        [SerializeField]
        private string nextScene;
        public string NextScene => nextScene;
        
        [SerializeField]
        private SceneTransitionOperation[] operations;
        public SceneTransitionOperation[] Operations => operations;

        [NonSerialized]
        private int operationIndex = 0;
        public float TransitionProgress 
        {
            get { return (operations.Length > 0? (operationIndex/operations.Length) : 1.0f); }
        }

        public string TransitionKey
        {
            get
            {
                return CreateKey(prevScene, nextScene);
            } 
        }

        public static string CreateKey(string prev, string next)
        {
            return $"FROM-{prev}-TO-{next}";
        }

        private void OnEnable()
        {
            Assert.IsNotNull(prevScene, "prevScene not assigned at " + this.name);
            Assert.IsNotNull(nextScene, "nextScene not assigned at " + this.name);
            
            foreach (var op in operations)
            {
                Assert.IsNotNull(op, $"Null operation assigned at " + this.name);
            }
        }

        public IEnumerator Load(CoroutineRunner runner)
        {
            int totalOperations = this.operations.Length;

            operationIndex = 0;
            
            foreach (SceneTransitionOperation operation in this.operations)
            {
                operationIndex++;
                yield return runner.StartCoroutine(operation.Run());
            }
        }
}
}
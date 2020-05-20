using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;
using System.Collections;

namespace AmoaebaUtils
{

public class SceneTransitionHandler : BootScriptableObject
{
       [SerializeField]
        private string emptyIdentifier;

        private string currentScene;
        private string previousScene;
    
        [SerializeField]
        private SceneTransitionDefinition[] transitions;

        private CoroutineRunner runner;
         
        private HashSet<string> transitionableScenes;
        private Dictionary<string, SceneTransitionDefinition> transitionsByKey;

        public override void OnBoot()
        {
            Debug.Log("YUP");
            return;
            Assert.IsTrue(emptyIdentifier != string.Empty, $"emptyIdentifier not assigned to " + this.name);

            foreach (SceneTransitionDefinition transition in transitions)
            {
                Assert.IsNotNull(transition, $"SceneTransitionDefinition not assigned to " + this.name);
            }
                
            this.runner = CoroutineRunner.Instantiate(this.name);
            this.transitionableScenes = new HashSet<string>();
            this.transitionsByKey = new Dictionary<string, SceneTransitionDefinition>();

            foreach (SceneTransitionDefinition transition in this.transitions)
            {
                this.transitionableScenes.Add(transition.PrevScene);
                this.transitionableScenes.Add(transition.NextScene);
                this.transitionsByKey[transition.TransitionKey] = transition;                                
            }
            
            SceneManager.sceneLoaded += this.OnSceneLoaded;

            this.EmptyTransition();
        }

        private void EmptyTransition()
        {
            string currentScene = SceneManager.GetActiveScene().name;


            this.previousScene = emptyIdentifier;
            this.currentScene = currentScene;
                
            string emptyKey = SceneTransitionDefinition.CreateKey(this.previousScene, this.currentScene);
            if(!transitionsByKey.ContainsKey(emptyKey))
            {
                Debug.LogError("EmptyTransition not assigned to " + this);
                return;

            }

            this.Transition();
        }
        
        private void Transition()
        {
            IEnumerator routine = this.Transition(SceneTransitionDefinition.CreateKey(this.previousScene, this.currentScene));
            this.runner.StartCoroutine(routine);
        }

        private IEnumerator Transition(string key)
        {
            if (!this.transitionsByKey.ContainsKey(key))
            {
                this.EmptyTransition();
                Debug.LogWarning("Could not find transition definition for " + key);
                yield break;
            }

            SceneTransitionDefinition transition = this.transitionsByKey[key];
            
            yield return this.runner.StartCoroutine(transition.Load(this.runner));
        }
        
        private void OnSceneLoaded(Scene newScene, LoadSceneMode mode)
        {
            string previous = this.currentScene;
            string current = newScene.name;
            
            if (this.transitionableScenes.Contains(newScene.name))
            {
                this.previousScene = previous;
                this.currentScene = current;
                
                this.Transition();
            }
            else
            {
                Debug.LogWarning("Transitioned to unkown scene " + newScene);
            }
        }
}
}

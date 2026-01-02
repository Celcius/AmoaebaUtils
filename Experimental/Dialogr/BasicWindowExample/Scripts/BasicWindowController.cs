using System.Collections;
using System.Collections.Generic;
using AmoaebaUtils;
using UnityEngine;
using static Dialogr.DialogrScene;
 
namespace Dialogr
{
public class BasicWindowController : MonoBehaviour {
 
    [SerializeField] 
    protected DialogrSceneScriptable StartFromScriptable;
    
    [SerializeField]
    protected CharacterNameScriptable DefaultNameScriptable;

    protected DialogrScene CurScene;
    protected SpeechNode CurNode;

    [SerializeField]
    protected BasicWindow BasicWindow;

    [SerializeField]
    protected StringEvent NextNodeEvent;
 
    public delegate void NodeEnteredHandler( SpeechNode node );

    protected virtual void Awake()
    {
        NextNodeEvent.OnEvent += OnNextNode;
        if(StartFromScriptable != null)
        {
            CurScene = StartFromScriptable;
            ShowScene(CurScene);
        }
    }

    public virtual  void ShowScene(DialogrScene scene, CharacterNameScriptable NamesScriptable = null)
    {
        this.CurScene = scene;
        BasicWindow.SetNamesScriptable(NamesScriptable == null? DefaultNameScriptable : NamesScriptable);
        InitializeDialogue();   
    }

    protected virtual void OnDestroy()
    {
          NextNodeEvent.OnEvent -= OnNextNode;
    }

     protected virtual void OnNextNode(string destination)
    {
        SpeechNode nextNode = CurScene.GetNode(destination);
        if(nextNode != null)
        {
            CurNode = nextNode;
            Debug.Log("Entering: " + CurScene.ToString());
            BasicWindow.ShowNode(nextNode);
        }
    }

    public virtual SpeechNode GetCurrentNode() {
        return CurNode;
    }
 
    public virtual void InitializeDialogue() 
    {
        CurNode = CurScene.GetStartNode();
        NextNodeEvent.Invoke(CurNode.Title);
    }
}
}
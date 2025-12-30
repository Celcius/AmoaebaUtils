using System.Collections;
using System.Collections.Generic;
using AmoaebaUtils;
using UnityEngine;
using static Dialogr.DialogrScene;
 
namespace Dialogr
{
public class BasicWindowController : MonoBehaviour {
 
    [SerializeField] 
    DialogrSceneScriptable StartFromScriptable;
    
    [SerializeField]
    CharacterNameScriptable DefaultNameScriptable;

    DialogrScene CurScene;
    SpeechNode CurNode;

    [SerializeField]
    private BasicWindow BasicWindow;

    [SerializeField]
    StringEvent NextNodeEvent;
 
    public delegate void NodeEnteredHandler( SpeechNode node );

    private void Awake()
    {
        NextNodeEvent.OnEvent += OnNextNode;
        if(StartFromScriptable != null)
        {
            CurScene = StartFromScriptable;
            ShowScene(CurScene);
        }
    }

    public void ShowScene(DialogrScene scene, CharacterNameScriptable NamesScriptable = null)
    {
        this.CurScene = scene;
        BasicWindow.SetNamesScriptable(NamesScriptable == null? DefaultNameScriptable : NamesScriptable);
        InitializeDialogue();   
    }

    private void OnDestroy()
    {
          NextNodeEvent.OnEvent -= OnNextNode;
    }

    private void OnNextNode(string destination)
    {
        SpeechNode nextNode = CurScene.GetNode(destination);
        if(nextNode != null)
        {
            CurNode = nextNode;
            Debug.Log("Entering: " + CurScene.ToString());
            BasicWindow.ShowNode(nextNode);
        }
    }

    public SpeechNode GetCurrentNode() {
        return CurNode;
    }
 
    public void InitializeDialogue() 
    {
        CurNode = CurScene.GetStartNode();
        NextNodeEvent.Invoke(CurNode.Title);
    }
}
}
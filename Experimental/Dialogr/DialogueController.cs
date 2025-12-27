using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Dialogr.DialogrScene;
 
namespace Dialogr
{
public class DialogueController : MonoBehaviour {
 
    [SerializeField] TextAsset twineText;
    DialogrSceneObject curDialogue;
    SpeechNode curNode;
 
    public delegate void NodeEnteredHandler( SpeechNode node );
    public event NodeEnteredHandler onEnteredNode;
 
    public SpeechNode GetCurrentNode() {
        return curNode;
    }
 
    public void InitializeDialogue() {
        curDialogue = new DialogrSceneObject( twineText );
        curNode = curDialogue.GetStartNode();
        onEnteredNode?.Invoke( curNode );
        Debug.Log(curDialogue.ToString());
    }
 
    public SpeechOptions[] GetCurrentResponses() {
        return curNode.Options;
    }
 
    public void ChooseResponse( int responseIndex ) {
        string nextNodeID = curNode.Options[responseIndex].destinationNode;
        SpeechNode nextNode = curDialogue.GetNode(nextNodeID);
        curNode = nextNode;
        onEnteredNode( nextNode );
    }
}
}
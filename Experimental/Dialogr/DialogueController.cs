using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static DialogrScene;
 
public class DialogueController : MonoBehaviour {
 
    [SerializeField] TextAsset twineText;
    Dialogue curDialogue;
    SpeechNode curNode;
 
    public delegate void NodeEnteredHandler( SpeechNode node );
    public event NodeEnteredHandler onEnteredNode;
 
    public SpeechNode GetCurrentNode() {
        return curNode;
    }
 
    public void InitializeDialogue() {
        curDialogue = new Dialogue( twineText );
        curNode = curDialogue.GetStartNode();
        onEnteredNode( curNode );
    }
 
    public List<SpeechOptions> GetCurrentResponses() {
        return curNode.options;
    }
 
    public void ChooseResponse( int responseIndex ) {
        string nextNodeID = curNode.options[responseIndex].destinationNode;
        SpeechNode nextNode = curDialogue.GetNode(nextNodeID);
        curNode = nextNode;
        onEnteredNode( nextNode );
    }
}
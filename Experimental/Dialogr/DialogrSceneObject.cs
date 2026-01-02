// Original code from https://www.mrventures.net/all-tutorials/converting-a-twine-story-to-unity

using System.Collections.Generic;
using Codice.Client.Common.TreeGrouper;
using UnityEngine;
 
namespace Dialogr
{
public class DialogrSceneObject : DialogrScene 
{
    protected string DialogTitle;
    protected Dictionary<string, SpeechNode> SpeechNodes;
    protected SpeechNode[] NodesArray;
    protected Dictionary<string,string> DataEntries;
    protected string StartNode;

    public DialogrSceneObject(TextAsset twineText, TwineParseSettings settings) 
    {
        DialogrUtils.ParseTwineText( twineText.text, this, settings);
    }
    
    public void SetTitle(string title)
    {
        DialogTitle = title;
    }

    public void SetStartNodeName(string nodeName)
    {
        StartNode = nodeName;
    }

    public void SetNodes(SpeechNode[] nodes)
    {
        SpeechNodes = new Dictionary<string, SpeechNode>();
        foreach(SpeechNode node in nodes)
        {
            if(DialogrUtils.ValidateNode(node))
            {
                SpeechNodes[node.Title] = node;   
            }
        }
        NodesArray = new List<SpeechNode>(SpeechNodes.Values).ToArray();
    }

    public void SetDataEntries(Dictionary<string, string> entries)
    {
        DataEntries = entries;
    }

    public SpeechNode[] GetNodes()
    {
        return NodesArray;
    }

    public SpeechNode GetNode( string nodeTitle ) {
        return SpeechNodes [ nodeTitle ];
    }

    public SpeechNode GetStartNode() {
        UnityEngine.Assertions.Assert.IsNotNull( StartNode );
        return SpeechNodes [ StartNode ];
    }

    public override string ToString()
    {
        string nodeStr = "";
        foreach(SpeechNode node in NodesArray)
        {
            nodeStr +=  node.ToString() + "\n";
        }

        string entriesStr = "Entries [";
        foreach(string key in DataEntries.Keys)
        {
            entriesStr += "" + key + " => " + DataEntries[key] + ", ";
        }
        entriesStr = entriesStr.Substring(0, entriesStr.Length-2); // remove extra comma and space
        entriesStr += "]\n";

        return  "Node {  Title: '"+ DialogTitle + "'\n  StartNode: '"+StartNode+"'\n  Data: '"+entriesStr+ "' \n Nodes:\n"+nodeStr+"}";
    }
}
}
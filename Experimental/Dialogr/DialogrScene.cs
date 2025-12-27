
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using UnityEngine;

public abstract class DialogrScene
{
    public const string START_NODE_TAG = "START";
    public const string END_NODE_TAG = "END";

    protected string DialogTitle;
    protected Dictionary<string, SpeechNode> SpeechNodes;
    
    [SerializeField]
    protected SpeechNode[] NodesArray;
    protected Dictionary<string,string> DataEntries;
    protected string StartNode;

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
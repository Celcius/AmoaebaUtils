using UnityEngine;
using System.Collections.Generic;
using System;
using NUnit.Framework;
using UnityEngine.UI;

namespace Dialogr
{

[CreateAssetMenu(fileName = "DialogrScene", menuName = "Dialogr/Empty Dialogr Scene Scriptable", order = 100)]
public class DialogrSceneScriptable : ScriptableObject, DialogrScene
{
    [Serializable]
    protected struct DataEntriesStruct
    {
        [SerializeField]
        public string Key;
        [SerializeField]
        public string Value;

        public DataEntriesStruct( string key, string value ) {
            Key = key;
            Value = value;
        }
    }


    [SerializeField]
    protected string DialogTitle;

    [SerializeField]
    protected SpeechNode[] NodesArray;

    [SerializeField]
    protected string StartNode;

    [SerializeField]
    protected DataEntriesStruct[] DataEntries;

    
    [SerializeField, HideInInspector]
    protected UnityEngine.Object LoadedTwineAsset;

    [SerializeField, HideInInspector]
    protected TwineParseSettings LoadedSettings;
    
    private Dictionary<string, SpeechNode> SpeechNodesDict;
    private Dictionary<string,string> DataEntriesDict;

    private void OnEnable()
    {
        
        if(NodesArray != null)
        {
            SpeechNodesDict = new Dictionary<string, SpeechNode>();
            foreach(SpeechNode node in NodesArray)
            {
                if(DialogrUtils.ValidateNode(node))
                {
                    SpeechNodesDict[node.Title] = node;
                }
            }
        }
        
        if(DataEntries != null)
        {
            DataEntriesDict = new Dictionary<string, string>();
            foreach(DataEntriesStruct entry in DataEntries)
            {
                DataEntriesDict[entry.Key] = entry.Value;
            }   
        }
    }

    private void OnDisable()
    {
        SpeechNodesDict?.Clear();
        DataEntriesDict?.Clear();       
    }

    public void ReloadTweeAsset()
    {
        InitializeAsset(LoadedTwineAsset, LoadedSettings);   
    }

    public void InitializeAsset(UnityEngine.Object twineText, TwineParseSettings settings) 
    {
        if (twineText is TextAsset textAsset)
        {
            Assert.True(textAsset != null, "Text Asset is null - " + twineText.name);
            DialogrUtils.ParseTwineText( textAsset.text, this, settings);
            LoadedTwineAsset = twineText;
            LoadedSettings = settings;
        }
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
            SpeechNodesDict = new Dictionary<string, SpeechNode>();
            foreach(SpeechNode node in nodes)
            {
                if(DialogrUtils.ValidateNode(node))
                {
                    SpeechNodesDict[node.Title] = node;   
                }
            }
            NodesArray = new List<SpeechNode>(SpeechNodesDict.Values).ToArray();
    }

    public void SetDataEntries(Dictionary<string, string> entries)
    {
        DataEntriesDict = entries;
        List<DataEntriesStruct> entriesList = new List<DataEntriesStruct>(); 
        foreach(string key in entries.Keys)
        {
            entriesList.Add(new DataEntriesStruct(key, entries[key]));
        }
        DataEntries = entriesList.ToArray();
    }

    public SpeechNode[] GetNodes()
    {
        return NodesArray;
    }

    public SpeechNode GetNode( string nodeTitle ) {
        return SpeechNodesDict [ nodeTitle ];
    }

    public SpeechNode GetStartNode() {
        UnityEngine.Assertions.Assert.IsNotNull( StartNode );
        return SpeechNodesDict [ StartNode ];
    }



        public override string ToString()
    {
        string nodeStr = "";
        foreach(SpeechNode node in NodesArray)
        {
            nodeStr +=  node.ToString() + "\n";
        }

        string entriesStr = "Entries [";
        foreach(string key in DataEntriesDict.Keys)
        {
            entriesStr += "" + key + " => " + DataEntriesDict[key] + ", ";
        }
        entriesStr = entriesStr.Substring(0, entriesStr.Length-2); // remove extra comma and space
        entriesStr += "]\n";

        return  "Node {  Title: '"+ DialogTitle + "'\n  StartNode: '"+StartNode+"'\n  Data: '"+entriesStr+ "' \n Nodes:\n"+nodeStr+"}";
    }
}
}
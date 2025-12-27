using System;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using UnityEngine;


[Serializable]
public class SpeechNode 
{
    [SerializeField]
    private string _title;
    public string Title => _title;

    [SerializeField]
    private string _text;
    public string Text {get;}

    [SerializeField]
    private string[] _tags;
    public string[] Tags => _tags;

    [SerializeField]
    private SpeechOptions[] _options;
    public SpeechOptions[] Options => _options;
    
    [SerializeField]
    private MetaAction[] _metaActions;
    public MetaAction[] MetaActions => _metaActions;

    public SpeechNode(string Title, string Text, string[] Tags, SpeechOptions[] Options, MetaAction[] MetaActions)
    {
        this._title = Title;
        this._text = Title;
        this._tags = Tags;
        this._options = Options;
        this._metaActions = MetaActions;
    }

    public override string ToString() 
    {
        string tagsStr = "Tags [";
        foreach(string tag in Tags)
        {
            tagsStr += tag + ", ";
        }
        tagsStr = tagsStr.Substring(0, tagsStr.Length-2); // remove extra comma and space
        tagsStr += "]";

        string optionsStr = Options.Length > 0? "\nOptions:\n" : "";
        foreach(SpeechOptions option in Options)
        {
            optionsStr += option.ToString() + "\n";
        }

        return "Node {  Title: '" + Title +"' Tag: '" +tagsStr+"'\nText:\n '"+ Text+optionsStr+"'}";
    }
}

[Serializable]
public struct SpeechOptions 
{
    [SerializeField]
    public string displayText;
    [SerializeField]
    public string destinationNode;

    public SpeechOptions( string display, string destination ) {
        displayText = display;
        destinationNode = destination;
    }

    public override string ToString()
    {
        return displayText + " => " + destinationNode;
    }
}

[Serializable]
public struct MetaAction 
{
    [SerializeField]
    public string Action;
    [SerializeField]
    public string[] Values;

    public MetaAction( string action, string[] values ) {
        this.Action = action;
        this.Values = values;
    }

    public override string ToString()
    {
        string valuesStr = "";
        foreach(string value in Values)
        {
            valuesStr += value +",";
        }
        valuesStr = valuesStr.Substring(0, valuesStr.Length-1);
        return Action + " => " + valuesStr;
    }
}

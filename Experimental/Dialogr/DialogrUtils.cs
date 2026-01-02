using System;
using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Codice.Client.Common.TreeGrouper;
using NUnit.Framework;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using UnityEditor.SearchService;

namespace Dialogr
{

[Serializable]
public struct TwineParseSettings
{
    public const string DEFAULT_META_START = "!";
    public const string DEFAULT_META_DIVIDER = ":";
    
    [SerializeField]
    public string MetaActionStart;
    [SerializeField]
    public string MetaActionDivider;

    public TwineParseSettings(string metaActionStart, string metaActionDivider)
    {
        this.MetaActionStart = metaActionStart;
        this.MetaActionDivider = metaActionDivider;
    }
}

public static class DialogrUtils
{
    private const string START_NODE_DATA_TAG = "start";

    // Note: tags and options are optional
    // A null Option allows transitions, only one Null Option is allowed per node
    // A null Option overrides options
    // Normal Format: "NodeTitle [Tags, comma, separated] \n Message Text with any number of paragraphs\n Option One Text[[Option One Destination]] \n Option Two Text [[Option Two]]"
    // No-Tag Format: "NodeTitle \n Message Text with any number of paragraphs\n Option One Text [[Option One Destination]] \n Option Two Text [[Option Two]]"
    // Null Option Format: "NodeTitle \n Message Text with any number of paragraphs \n [[Null Option]] 
    public static void ParseTwineText(string twineText, DialogrScene scene, TwineParseSettings settings)
    {
        Assert.NotNull(scene, "Trying to setup invalid DialogrScene");

        string[] nodeData = twineText.Split(new string[] { "::" }, StringSplitOptions.None);
        RemovePositionData (ref nodeData);

        // Parse Nodes
        List<SpeechNode> nodes = new List<SpeechNode>();
        for (int i = 0; i < nodeData.Length; i++)
        {
            string nodeText = nodeData[i]; 

            if (IsTitleNode(nodeText))
            {
                ParseTitle(nodeText, scene);
            }
            else if(IsDataNode(nodeText))
            {
                ParseMetaDataNode(nodeText, scene);
            }
            else if(nodeText.Length > 0)
            {
                nodes.Add(ParseNode(nodeText, scene, settings));
            }
        }
        scene.SetNodes(nodes.ToArray());
    }

    private static void RemovePositionData(ref string[] NodeData)
    {
            for(int i = 0; i < NodeData.Length; i++)
        {
            string nodeText = NodeData[i];
            // Remove position data
            int posBegin = nodeText.IndexOf("{\"position");
            if ( posBegin != -1 )
            {
                int posEnd = nodeText.IndexOf("}", posBegin);
                nodeText = nodeText.Substring( 0, posBegin ) + nodeText.Substring( posEnd + 1 );
            }
            NodeData[i] = nodeText;
        }
    }

    public static bool IsTitleNode(string nodeText)
    {
        return nodeText.StartsWith(" StoryTitle");
    }

    private static bool IsDataNode(string nodeText)
    {
        return nodeText.StartsWith(" StoryData");
    }

    public static void ParseTitle(string nodeText, DialogrScene scene)
    {
        const string TITLE_START = "\n";
        const string TITLE_END = "\n\n\n";
        scene.SetTitle(TrimNodeFat(nodeText,TITLE_START, TITLE_END));
    }

    public static void ParseMetaDataNode(string nodeText, DialogrScene scene)
    {
        const string DATA_START = "\n{\n";
        const string DATA_END = "\n}\n\n\n";
        nodeText = TrimNodeFat(nodeText, DATA_START, DATA_END);

        // Need to remove zoom end which can be of variable size
        int zoomIndex = nodeText.IndexOf("\n  },\n  \"zoom\":");
        if(zoomIndex < 0)
        {
            zoomIndex = nodeText.IndexOf(",\n  \"zoom\":"); // tagless scene
        }
        nodeText = nodeText.Substring(0, zoomIndex);
        
        // need to add regular ending to use regex for last element
        nodeText += ",\n";

        // string[] dataEntries = nodeText.Split(new string[] { "::" }, StringSplitOptions.None);
        const string ENTRIES_START = "\\\"";
        const string ENTRIES_SEPARATOR = "\\\": \\\"";
        const string ENTRIES_END = "\\\",\n";
        
        Dictionary<string, string> dataEntries = new Dictionary<string, string>();
        Match match = Regex.Match(nodeText, ""+ENTRIES_START + "(.*?)" + ENTRIES_SEPARATOR +"(.*?)" + ENTRIES_END);

        while(match.Success)
        {
            // Always 3 groups - string, key and value
            dataEntries[match.Groups[1].Value] = match.Groups[2].Value;
            match = match.NextMatch();
        }
        scene.SetDataEntries(dataEntries);

        Assert.True(dataEntries.ContainsKey(START_NODE_DATA_TAG), "Could not find start node");
        scene.SetStartNodeName(dataEntries[START_NODE_DATA_TAG]);
    }

    private static string TrimNodeFat(string nodeText, string startTrim, string endTrim)
    {
        int start = nodeText.IndexOf(startTrim) + startTrim.Length;
        int end = nodeText.IndexOf(endTrim);
        return nodeText.Substring(start, end-start);
    }

    private static SpeechNode ParseNode(string nodeText, DialogrScene scene, TwineParseSettings settings)
    {
        const string DIVIDER = " \n";
        int dividerStart = nodeText.IndexOf(DIVIDER);
        string header = nodeText.Substring(0, dividerStart);
        string messageText = nodeText.Substring(dividerStart+DIVIDER.Length, nodeText.Length - (dividerStart+DIVIDER.Length));

        const string TAG_START = "[";
        const string TAG_END = "]";
        bool tagsPresent = nodeText.IndexOf(TAG_START) > 0;

        // Extract Title

        int titleStart = 0;
        int titleEnd = tagsPresent
            ? nodeText.IndexOf(TAG_START)
            : header.Length;
        string title = nodeText.Substring(titleStart, titleEnd - titleStart).Trim();

        // Extract Tags

        string tags = tagsPresent? 
        nodeText.Substring(titleEnd+TAG_START.Length, header.Length - (titleEnd + TAG_START.Length + TAG_END.Length))
            : "";
        List<string> tagsList = new List<string>( tags.Split( new string[] { " " }, StringSplitOptions.None ) );

        // Extract Meta Actions
  
        List<MetaAction> metaActionsList = new List<MetaAction>();
        HashSet<string> metaActionsValidation = new HashSet<string>();
        for(int i = tagsList.Count-1; i >= 0; i--)
        {
            string tag = tagsList[i];
            if(tag.StartsWith(settings.MetaActionStart))
            {
                tag = tag.Substring(settings.MetaActionStart.Length, tag.Length-settings.MetaActionStart.Length);
                tagsList.RemoveAt(i);
                List<string> actionElements = new List<string>(tag.Split(new string[] { settings.MetaActionDivider }, StringSplitOptions.None));
                Assert.True(actionElements.Count > 1, "Invalid amount of actionElements in node " + title);
                
                string key = actionElements[0];
                actionElements.RemoveAt(0);
                Assert.False(metaActionsValidation.Contains(key), "Action already present in node " + title);
                metaActionsList.Add(new MetaAction(key, actionElements.ToArray())); 
            }
        }

        // Extract Text and Options

        List<SpeechOption> options = new List<SpeechOption>();
        string text = "";
        string[] lines = messageText.Split( new string[] { "\n" }, StringSplitOptions.None);
        const string OPTION_START = "[[";
        const string OPTION_END = "]]";
        foreach(string line in lines)
        {
            string trimmed = line.Trim(); // Need to trim to avoid problems with transitions
            int optionIndex = trimmed.IndexOf(OPTION_START);
            if(optionIndex >= 0)
            {
                // Option
                
                string display = optionIndex == 0? "" : trimmed.Substring(0, optionIndex);
                string destination = trimmed.Substring(optionIndex+OPTION_START.Length, trimmed.Length - (optionIndex + OPTION_START.Length+OPTION_END.Length));
                options.Add(new SpeechOption(display, destination));
            }
            else
            {
                // Text
                text += trimmed + "\n";
            }
        }
        text = text.Trim();

        return new SpeechNode(title, text, tagsList.ToArray(), options.ToArray(), metaActionsList.ToArray());
    }

    public static bool ValidateNode(SpeechNode node)
    {
        bool isValid = true;

        // Validate empty node
        int emptyOptions = 0;
        int nonEmptyOptions = 0;
        foreach(SpeechOption option in node.Options)
        {
            bool isEmpty = option.displayText == null || option.displayText.Length == 0;
            emptyOptions += isEmpty? 1 : 0;
            nonEmptyOptions += !isEmpty? 1 : 0;

        }
        
        isValid |= (emptyOptions == 0 && nonEmptyOptions > 0) || (emptyOptions > 0 && nonEmptyOptions == 0);
        Assert.IsTrue(isValid,"Invalid Node (" + node.Title+ ") mismatch option types (empty:" + emptyOptions +", non-empty:" + nonEmptyOptions);
        return isValid;
    }


}
}
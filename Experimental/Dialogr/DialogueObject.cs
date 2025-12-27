// Original code from https://www.mrventures.net/all-tutorials/converting-a-twine-story-to-unity

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using UnityEngine;
 
public class DialogrScene {
    private const string START_NODE_TAG = "START";
    private const string END_NODE_TAG = "END";
 
    public struct SpeechOptions {
        public string displayText;
        public string destinationNode;
 
        public SpeechOptions( string display, string destination ) {
            displayText = display;
            destinationNode = destination;
        }
    }
 
    public class SpeechNode {
        public string title;
        public string text;
        public List<string> tags;
        public List<SpeechOptions> options;
 
        internal bool IsEndNode() {
            return tags.Contains( END_NODE_TAG );
        }

        internal bool IsStartNode() {
            return tags.Contains( START_NODE_TAG );
        }
 
        // TODO proper override
        public string Print() {
            return "";//string.Format( "Node {  Title: '%s',  Tag: '%s',  Text: '%s'}", title, tag, text );
        }
 
    }
 
    public class Dialogue {
        string DialogTitle;
        Dictionary<string, SpeechNode> nodes;
        Dictionary<string,string> dataEntries;
        string StartNode;
        public Dialogue( TextAsset twineText ) {
            nodes = new Dictionary<string, SpeechNode>();
            ParseTwineText( twineText.text );
        }
 
        public SpeechNode GetNode( string nodeTitle ) {
            return nodes [ nodeTitle ];
        }
 
        public SpeechNode GetStartNode() {
            UnityEngine.Assertions.Assert.IsNotNull( StartNode );
            return nodes [ StartNode ];
        }

        // Note: tags and options are optional
        // A null Option allows transitions, only one Null Option is allowed per node
        // A null Option overrides options
        // Normal Format: "NodeTitle [Tags, comma, separated] \n Message Text with any number of paragraphs\n Option One Text[[Option One Destination]] \n Option Two Text [[Option Two]]"
        // No-Tag Format: "NodeTitle \n Message Text with any number of paragraphs\n Option One Text [[Option One Destination]] \n Option Two Text [[Option Two]]"
        // Null Option Format: "NodeTitle \n Message Text with any number of paragraphs \n [[Null Option]] 
        private void ParseTwineText( string twineText )
        {
            string[] nodeData = twineText.Split(new string[] { "::" }, StringSplitOptions.None);
    
            RemovePositionData (ref nodeData);

            // Parse Nodes
            for (int i = 0; i < nodeData.Length; i++)
            {
                string nodeText = nodeData[i]; 

                if (IsTitleNode(nodeText))
                {
                    ParseTitle(nodeText);
                }
                else if(IsDataNode(nodeText))
                {
                    ParseData(nodeText);
                }
                else if(nodeText.Length > 0)
                {
                    ParseNode(nodeText);
                }
            }
        }

        private void RemovePositionData(ref string[] NodeData)
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

        private bool IsTitleNode(string nodeText)
        {
            return nodeText.StartsWith(" StoryTitle");
        }

        private bool IsDataNode(string nodeText)
        {
            return nodeText.StartsWith(" StoryData");
        }

        private void ParseTitle(string nodeText)
        {
            const string TITLE_START = "\n";
            const string TITLE_END = "\n\n\n";
            DialogTitle = TrimNodeFat(nodeText,TITLE_START, TITLE_END);
        }

        private void ParseData(string nodeText)
        {
            const string DATA_START = "\n{\n";
            const string DATA_END = "\n}\n\n\n";
            nodeText = TrimNodeFat(nodeText, DATA_START, DATA_END);

            // Need to remove zoom end which can be of variable size
            int zoomIndex = nodeText.IndexOf("\n  },\n  \"zoom\":");
            nodeText = nodeText.Substring(0, zoomIndex);
            
            // need to add regular ending to use regex for last element
            nodeText += ",\n";

            // string[] dataEntries = nodeText.Split(new string[] { "::" }, StringSplitOptions.None);
            const string ENTRIES_START = "\\\"";
            const string ENTRIES_SEPARATOR = "\\\": \\\"";
            const string ENTRIES_END = "\\\",\n";
            
            dataEntries = new Dictionary<string, string>();
            Match match = Regex.Match(nodeText, ""+ENTRIES_START + "(.*?)" + ENTRIES_SEPARATOR +"(.*?)" + ENTRIES_END);

            while(match.Success)
            {
                // Always 3 groups - string, key and value
                dataEntries[match.Groups[1].Value] = match.Groups[2].Value;
                match = match.NextMatch();
            }

            const string START_NODE = "start";
            StartNode = dataEntries[START_NODE];
        }

        private static string TrimNodeFat(string nodeText, string startTrim, string endTrim)
        {
            int start = nodeText.IndexOf(startTrim) + startTrim.Length;
            int end = nodeText.IndexOf(endTrim);
            return nodeText.Substring(start, end-start);
        }

        private void ParseNode(string nodeText)
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
  

            // Create Node
        
            SpeechNode node = new SpeechNode();
            node.title = title;
            node.options = new List<SpeechOptions>();
            node.tags = new List<string>( tags.Split( new string[] { "," }, StringSplitOptions.None ) );

            // Extract Text and Options
            string text = "";
            string[] lines = messageText.Split( new string[] { "\n" }, StringSplitOptions.None);
            const string OPTION_START = "[[";
            const string OPTION_END = "]]";
            foreach(string line in lines)
            {
                int optionIndex = line.IndexOf(OPTION_START);
                if(line.IndexOf(OPTION_START) > 0)
                {
                    // Option
                    string display = line.Substring(0, optionIndex);
                    string destination = line.Substring(optionIndex+OPTION_START.Length, line.Length - (optionIndex + OPTION_START.Length+OPTION_END.Length));
                    node.options.Add(new SpeechOptions(display, destination));
                }
                else
                {
                    // Text
                    text += line + "\n";
                }
            }

            // Remove unnecessary \n
            node.text = text.Trim();

            if(ValidateNode(node))
            {
                nodes[ node.title ] = node;                
            }
        }

        private bool ValidateNode(SpeechNode node)
        {
            bool isValid = true;

            // Validate empty node
            int emptyOptions = 0;
            int nonEmptyOptions = 0;
            foreach(SpeechOptions option in node.options)
            {
                bool isEmpty = option.displayText == null || option.displayText.Length == 0;
                emptyOptions += isEmpty? 1 : 0;
                nonEmptyOptions += !isEmpty? 1 : 0;

            }
            
            isValid |= (emptyOptions == 0 && nonEmptyOptions > 0) || (emptyOptions > 0 && nonEmptyOptions == 0);
            Assert.IsTrue(isValid,"Invalid Node (" + node.title+ ") mismatch option types (empty:" + emptyOptions +", non-empty:" + nonEmptyOptions);
            return isValid;
        }
    }
}
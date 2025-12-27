
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using UnityEngine;

public interface DialogrScene
{
    public const string START_NODE_TAG = "START";
    public const string END_NODE_TAG = "END";

    public void SetTitle(string title);

    public void SetStartNodeName(string nodeName);

    public void SetNodes(SpeechNode[] nodes);

    public void SetDataEntries(Dictionary<string, string> entries);
    public SpeechNode[] GetNodes();

    public SpeechNode GetNode( string nodeTitle );

    public SpeechNode GetStartNode();

    public string ToString();
}
using UnityEngine;


public class SpeechNode 
{
    public string Title {get;}
    public string Text {get;}
    public string[] Tags {get;}
    public SpeechOptions[] Options {get;}

    public SpeechNode(string Title, string Text, string[] Tags, SpeechOptions[] Options)
    {
        this.Title = Title;
        this.Text = Title;
        this.Tags = Tags;
        this.Options = Options;
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

public struct SpeechOptions 
{
    public string displayText;
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

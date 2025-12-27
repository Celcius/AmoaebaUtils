// Original code from https://www.mrventures.net/all-tutorials/converting-a-twine-story-to-unity

using System.Collections.Generic;
using Codice.Client.Common.TreeGrouper;
using UnityEngine;
 
public class DialogrSceneObject : DialogrScene 
{
    public DialogrSceneObject(TextAsset twineText) 
    {
        DialogrUtils.ParseTwineText( twineText.text, this);
    }
}